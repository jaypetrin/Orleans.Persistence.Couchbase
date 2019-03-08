using Microsoft.Extensions.Options;
using Orleans;
using Orleans.Configuration;
using Orleans.Runtime;
using Orleans.Storage;
using System;
using System.Threading.Tasks;

namespace Storage.Couchbase
{
    public class CouchbaseGrainStorage : IGrainStorage
    {
        private readonly CouchbaseGrainStorageOptions _options;
        private readonly ClusterOptions _clusterOptions;
        private readonly IGrainFactory _grainFactory;
        private readonly string _name;
        

        public CouchbaseGrainStorage(string name, CouchbaseGrainStorageOptions options, IOptions<ClusterOptions> clusterOptions, IGrainFactory grainFactory )
        {
            _options = options;
            _name = name;
            _clusterOptions = clusterOptions.Value;
            _grainFactory = grainFactory;
        }

        public Task ClearStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            throw new NotImplementedException();
        }

        public Task ReadStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            
        }

        public Task WriteStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            throw new NotImplementedException();
        }
    }
}
