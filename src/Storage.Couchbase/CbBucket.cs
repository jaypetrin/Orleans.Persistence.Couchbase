using Couchbase;
using Couchbase.Authentication;
using Couchbase.Configuration.Client;
using Couchbase.Core;
using Storage.Couchbase;
using System;
using System.Threading.Tasks;

namespace Orleans.Persistence.Couchbase
{
    //Bucket Instance needs to be a singleton to prevent connection inefficiency in Couchbase
    public class CbBucket
    {
        private IBucket _bucket;
        private static readonly Lazy<CbBucket> _lazy =
            new Lazy<CbBucket>(() => new CbBucket());

        private static CouchbaseGrainStorageOptions _config;

        private CbBucket()
        {
        }

        public static CbBucket GetInstance(CouchbaseGrainStorageOptions config)
        {
            _config = config;
            return _lazy.Value;
        }

        public IBucket Bucket
        {
            get
            {
                if (_bucket == null)
                    InitConnection().Wait();

                return _bucket;
            }
        }

        private async Task InitConnection()
        {
            //Initialize bucket if it doesn't exist
            ClusterHelper.Initialize(new ClientConfiguration
            {
                Servers = _config.Uris
            }, new PasswordAuthenticator(_config.UserName, _config.Password));

            //Connects to bucket
            _bucket = await ClusterHelper.GetBucketAsync(_config.BucketName);
        }
    }
}
