using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Storage.Couchbase
{
    public class GrainStateDataManager<T> where T : class
    {
        private readonly CouchbaseDataManager<T> dataManager;

        public GrainStateDataManager(CouchbaseGrainStorageOptions options)
        {
            dataManager = new CouchbaseDataManager<T>(options.Uris, options.BucketName, options.UserName, options.Password);
        }

        public Task InitConnectionAsync()
        {
            return dataManager.InitConnection();
        }

        public async Task<GrainStateDocument> Read(string docId)
        {

        }
    }
}
