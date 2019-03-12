using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Couchbase;
using Couchbase.Authentication;
using Couchbase.Configuration.Client;
using Couchbase.Core;
using Microsoft.Extensions.Logging;

namespace Storage.Couchbase
{
    public class CouchbaseDataManager
    {
        private readonly ILogger _logger;

        private IBucket _bucket;
        //Couch Interactions

        public List<Uri> ClusterUris { get; set; }
        public CouchbaseDataManager(ILogger logger, string userName, string password, string bucketName)
        {
            _logger = logger;
            this.UserName = userName;
            this.Password = password;
            this.BucketName = bucketName;
        }

        public string UserName { get; set; }
        public string Password { get; set; }
        public string BucketName { get; set; }

        public CouchbaseDataManager(ILogger logger, List<Uri> uris, string bucketName, string userName, string password)
        {
            _logger = logger;
            ClusterUris = uris;
            BucketName = bucketName;
            UserName = userName;
            Password = password;
        }

        public async Task InitConnection()
        {
            //Initialize bucket if it doesn't exist
            ClusterHelper.Initialize(new ClientConfiguration
            {
                Servers = ClusterUris
            }, new PasswordAuthenticator(UserName, Password));

            //Connects to bucket
            _bucket = await ClusterHelper.GetBucketAsync(BucketName);
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

            await _bucket.UpsertAsync(document.Id.ToString(), document.Data);
            //TODO: Add Exception Handling
            return document.Id.ToString();
        }

        public async Task DeleteEntryAsync(string documentId)
        {
            //Delete existing document
            await _bucket.RemoveAsync(documentId);
            //TODO: Add Exception Handling
        }

        public async Task<GrainStateDocument> ReadSingleEntryAsync(string documentId)
        {
            var result = await _bucket.GetAsync<GrainStateDocument>(documentId);

            //TODO: Add Exception Handling
            return result.Value;
        }
    }
}