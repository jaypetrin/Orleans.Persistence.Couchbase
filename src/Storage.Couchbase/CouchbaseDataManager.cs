using Couchbase;
using Couchbase.Authentication;
using Couchbase.Configuration.Client;
using Couchbase.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Storage.Couchbase
{
    public class CouchbaseDataManager<T> where T : GrainStateDocument
    {
        //Couch Interactions

        public List<Uri> ClusterUris { get; set; }
        public CouchbaseDataManager(string userName, string password, string bucketName) 
        {
            this.UserName = userName;
                this.Password = password;
                this.BucketName = bucketName;
               
        }
                public string UserName { get; set; }
        public string Password { get; set; }
        public string BucketName { get; set; }

        private IBucket _bucket;

        public CouchbaseDataManager(List<Uri> uris, string bucketName, string userName, string password)
        {
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

        public async Task<string> UpsertEntryAsync(T data)
        {
            //Insert if document doesn't exist otherwise update existing document
            if(!data.Id.HasValue)
            {
                data.Id = Guid.NewGuid();
            }

            await _bucket.UpsertAsync(data.Id.ToString(), data);
            //TODO: Add Exception Handling
            return data.Id.ToString();
        }

        public async Task DeleteEntryAsync(string docId)
        {
            //Delete existing document
            await _bucket.RemoveAsync(docId);
            //TODO: Add Exception Handling
        }

        public async Task<T> ReadSingleEntryAsync(string docId)
        {
            var result = await _bucket.GetAsync<T>(docId);

            //TODO: Add Exception Handling
            return result.Value;
        }
    }
}
