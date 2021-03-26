using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using HackTheClimate.Tools.Data;

namespace HackTheClimate.Tools.Service
{
    class Indexer
    {
        private const string IndexName = "climate-docs";
        public async Task CreateIndex(string fileDir, string legislationFile)
        {
            Uri endpoint = new Uri("https://<something>.search.windows.net");
            string key = "<addme>";

            await CreateIndexAsync(key, endpoint, IndexName);
            await IndexDocumentsAsync(key, endpoint, IndexName, PrepareSearchModel(fileDir, legislationFile));
        }

        private static async Task CreateIndexAsync(string key, Uri endpoint, string indexName)
        {

            var credential = new AzureKeyCredential(key);
            SearchIndexClient client = new SearchIndexClient(endpoint, credential);

            var indexResult = await client.GetIndexAsync(indexName);
            if (indexResult.Value != null)
            {
                return;
            }

            var fields = new FieldBuilder().Build(typeof(SearchModel));

            fields.Select(nameof(SearchModel.Id)).IsKey = true;
            fields.Select(nameof(SearchModel.Description)).IsSearchable = true;
            fields.Select(nameof(SearchModel.Description)).IsSearchable = true;
            fields.Select(nameof(SearchModel.DocumentTypes)).IsSearchable = true;
            fields.Select(nameof(SearchModel.Frameworks)).IsSearchable = true;
            fields.Select(nameof(SearchModel.Keywords)).IsSearchable = true;
            fields.Select(nameof(SearchModel.PolicyLanguages)).IsSearchable = true;
            fields.Select(nameof(SearchModel.Geography)).IsSearchable = true;
            fields.Select(nameof(SearchModel.LegislationType)).IsSearchable = true;
            fields.Select(nameof(SearchModel.NaturalHazards)).IsSearchable = true;
            fields.Select(nameof(SearchModel.Responses)).IsSearchable = true;
            fields.Select(nameof(SearchModel.Sector)).IsSearchable = true;
            fields.Select(nameof(SearchModel.PolicyTexts)).IsSearchable = true;
            fields.Select(nameof(SearchModel.PolicyTexts)).IsFilterable = true;

            // Create the index using FieldBuilder.
            SearchIndex index = new SearchIndex(indexName)
            {
                Fields = fields,
                Similarity = new BM25Similarity()
            };

            await client.CreateIndexAsync(index);
        }

        private static async Task IndexDocumentsAsync(string key, Uri endpoint, string indexName,
            List<SearchModel> models)
        {
            var credential = new AzureKeyCredential(key);
            SearchClient client = new SearchClient(endpoint, indexName, credential);

            // too large batches cause issues...
            for (int i = 0; i < models.Count; i += 100)
            {
                Console.WriteLine($"{i}/{models.Count}");
                try
                {
                    var operations = models.Skip(i).Take(100)
                        .Select(x => IndexDocumentsAction.MergeOrUpload<SearchModel>(x)).ToArray();
                    var batch = IndexDocumentsBatch.Create<SearchModel>(operations);

                    IndexDocumentsOptions options = new IndexDocumentsOptions { ThrowOnAnyError = true };
                    var result = await client.IndexDocumentsAsync(batch, options);
                }
                catch (Exception exception)
                {
                    // ignore for now
                }
            }
        }


        private static List<SearchModel> PrepareSearchModel(string fileDirectory, string legislationFile)
        {
            var files = new DirectoryInfo(fileDirectory).GetFiles();

            return DataLoader.LoadLegislationFile(legislationFile)
                .Select(legislationEntry =>
                {
                    var policyTexts = files.Where(file => file.Name.StartsWith(legislationEntry.Id));

                    var texts = policyTexts.Select(x => File.ReadAllText(x.FullName)).ToList();
                    //var policyLanguages = policyTexts.Select(x => languages[x.Name]).ToList();

                    return new SearchModel
                    {
                        Description = legislationEntry.Description,
                        NaturalHazards = legislationEntry.Natural_hazards.SplitAndTrim(","),
                        Keywords = legislationEntry.Keywords.SplitAndTrim("/", ","),
                        DocumentTypes = legislationEntry.Document_types.SplitAndTrim("/", ","),
                        Responses = legislationEntry.Responses.SplitAndTrim("/", ","),
                        Sector = legislationEntry.Sector.SplitAndTrim("/", ","),
                        Frameworks = legislationEntry.Frameworks.SplitAndTrim("/", ","),
                        Geography = legislationEntry.Geography,
                        GeographyIso = legislationEntry.Geography_iso,
                        Id = legislationEntry.Id,
                        LegislationType = legislationEntry.Legislation_type,
                        Title = legislationEntry.Title.Trim(),
                        PolicyTexts = texts.ToArray()
                        //PolicyLanguages = policyLanguages.Select(x=>x.Item1).ToArray()
                    };
                }).ToList();
        }
    }
}
