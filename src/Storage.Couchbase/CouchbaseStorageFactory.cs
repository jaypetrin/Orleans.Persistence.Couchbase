using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Orleans.Configuration;
using Orleans.Configuration.Overrides;
using Orleans.Storage;
using Storage.Couchbase;
using System;

namespace Orleans.Persistence.Couchbase
{
    public static class CouchbaseStorageFactory
    {
        public static IGrainStorage Create(IServiceProvider services, string name)
        {
            IOptionsSnapshot<CouchbaseGrainStorageOptions> optionsSnapshot = services.GetRequiredService<IOptionsSnapshot<CouchbaseGrainStorageOptions>>();
            IOptions<ClusterOptions> clusterOptions = services.GetProviderClusterOptions(name);
            return ActivatorUtilities.CreateInstance<CouchbaseGrainStorage>(services, name, optionsSnapshot.Get(name), clusterOptions);
        }
    }
}
