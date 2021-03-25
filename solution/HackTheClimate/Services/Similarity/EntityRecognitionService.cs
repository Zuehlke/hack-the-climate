using System;
using System.IO;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;

namespace HackTheClimate.Services.Similarity
{
    public class EntityRecognitionService
    {
        private readonly IOptions<AzureBlobEntitiesConfiguration> _blobOptions;
        private BlobServiceClient _blobServiceClient;
        private BlobContainerClient _blobContainerClient;

        public EntityRecognitionService(IOptions<AzureBlobEntitiesConfiguration> blobOptions)
        {
            _blobOptions = blobOptions;
            _blobServiceClient = new BlobServiceClient(new Uri(blobOptions.Value.Endpoint));
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(_blobOptions.Value.BlobContainerName);
        }

        public string getEntityFile(string id)
        {
            var blobClient = _blobContainerClient.GetBlobClient("1004_0_entities.json");
            var download = blobClient.Download();
            var streamReader = new StreamReader(download.Value.Content);
            return streamReader.ReadToEnd();
        }
    }
}