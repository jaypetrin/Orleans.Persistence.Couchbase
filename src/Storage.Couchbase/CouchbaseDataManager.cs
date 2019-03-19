using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Couchbase;
using Microsoft.Extensions.Logging;
using Orleans.Persistence.Couchbase;

namespace Storage.Couchbase
{
    public class CouchbaseDataManager
    {
        private readonly ILogger _logger;
        private readonly CouchbaseGrainStorageOptions _config;

        private CbBucket _bucket;
        //Couch Interactions

        public List<Uri> ClusterUris { get; set; }
        public CouchbaseDataManager(ILogger logger, CouchbaseGrainStorageOptions options)
        {
            _logger = logger;
            _config = options;
        }

        public string UserName { get; set; }
        public string Password { get; set; }
        public string BucketName { get; set; }

        public Task InitConnection()
        {
            _bucket = CbBucket.GetInstance(_config);
            return Task.CompletedTask;
        }

        public void CloseConnection()
        {
            ClusterHelper.Close();
        }

        public async Task<string> UpsertEntryAsync(GrainStateDocument document)
        {
            //Insert if document doesn't exist otherwise update existing document
            if (!String.IsNullOrWhiteSpace(document.Id))
            {
                document.Id = Guid.NewGuid().ToString();
            }

            var bucket = _bucket.Bucket;
            await bucket.UpsertAsync(document.Id.ToString(), document.Data);
            //TODO: Add Exception Handling
            return document.Id.ToString();
        }

        public async Task DeleteEntryAsync(string documentId)
        {
            //Delete existing document
            var bucket = _bucket.Bucket;
            await bucket.RemoveAsync(documentId);
            //TODO: Add Exception Handling
        }

        public async Task<GrainStateDocument> ReadSingleEntryAsync(string documentId)
        {
            var bucket = _bucket.Bucket;
            var result = await bucket.GetAsync<GrainStateDocument>(documentId);

            //TODO: Add Exception Handling
            return result.Value;
        }
    }
}