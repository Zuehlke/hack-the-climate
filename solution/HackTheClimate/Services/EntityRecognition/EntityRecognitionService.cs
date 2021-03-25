using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using HackTheClimate.Data;
using Microsoft.Extensions.Options;

namespace HackTheClimate.Services.Similarity
{
    public class EntityRecognitionService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobContainerClient _blobContainerClient;
        private readonly Dictionary<string, HashSet<string>> _cache = new();

        public EntityRecognitionService(IOptions<AzureBlobEntitiesConfiguration> blobOptions)
        {
            _blobContainerClient = new BlobServiceClient(new Uri(blobOptions.Value.Endpoint))
                .GetBlobContainerClient(blobOptions.Value.BlobContainerName);
        }

        private EntityRecognitionInfo GetEntityFile(string id)
        {
            var matchingBlobs = _blobContainerClient.GetBlobs(prefix: id + "_");

            var blobItems = matchingBlobs.ToList();
            if (blobItems.Count == 0)
            {
                return new EntityRecognitionInfo();
            }

            BlobItem largestBlobItem = blobItems[0];
            long blobItemSize = 0;
            foreach (var item in blobItems)
            {
                if (item.Properties.ContentLength.HasValue && item.Properties.ContentLength.Value > blobItemSize)
                {
                    largestBlobItem = item;
                    blobItemSize = item.Properties.ContentLength.Value;
                }
            }

            var blobClient = _blobContainerClient.GetBlobClient(largestBlobItem.Name);
            var download = blobClient.Download();
            var streamReader = new StreamReader(download.Value.Content, Encoding.UTF8);
            var readToEnd = streamReader.ReadToEnd();
            return JsonSerializer.Deserialize<EntityRecognitionInfo>(readToEnd);
        }

        public HashSet<string> GetEntitiesForCategory(Legislation legislation, string category)
        {
            var key = legislation.Id + "_" + category;

            if (_cache.ContainsKey(key))
            {
                return _cache[key];
            }
            
            var entityFile = GetEntityFile(legislation.Id);

            var entitiesPerCategory = new HashSet<string>();
            if (entityFile.Entities != null)
            {
                foreach (var entity in entityFile.Entities)
                {
                    if (category.Equals(entity.Category))
                    {
                        entitiesPerCategory.Add(entity.Text);
                    }
                }
            }

            _cache.Add(key, entitiesPerCategory);
            return entitiesPerCategory;
        }
    }
}