using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Orleans.Hosting;
using Orleans.Providers;
using Orleans.Runtime;
using Orleans.Storage;
using Orleans.Configuration;
using Storage.Couchbase;
using System;

namespace Orleans.Persistence.Couchbase
{
    public static class CouchbaseSiloBuilderExtensions
    {
        public static ISiloHostBuilder AddCouchbaseGrainStorageAsDefault(this ISiloHostBuilder builder, Action<CouchbaseGrainStorageOptions> configureOptions)
        {
            return builder.AddCouchbaseGrainStorage(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME, configureOptions);
        }

        public static ISiloHostBuilder AddCouchbaseGrainStorageAsDefault(this ISiloHostBuilder builder, Action<OptionsBuilder<CouchbaseGrainStorageOptions>> configureOptions = null)
        {
            return builder.AddCouchbaseGrainStorage(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME, configureOptions);
        }

        public static ISiloHostBuilder AddCouchbaseGrainStorage(this ISiloHostBuilder builder, string name, Action<CouchbaseGrainStorageOptions> configureOptions)
        {
            return builder.ConfigureServices(services => services.AddCouchbaseGrainStorage(name, ob=> ob.Configure(configureOptions)));
        }

        public static ISiloHostBuilder AddCouchbaseGrainStorage(this ISiloHostBuilder builder, string name, Action<OptionsBuilder<CouchbaseGrainStorageOptions>> configureOptions = null)
        {
            return builder.ConfigureServices(services => services.AddCouchbaseGrainStorage(name, configureOptions));
        }

        public static ISiloBuilder AddCouchbaseGrainStorage(this ISiloBuilder builder, Action<CouchbaseGrainStorageOptions> configureOptions)
        {
            return builder.AddCouchbaseGrainStorage(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME, configureOptions);
        }

        public static ISiloBuilder AddCouchbaseGrainStorage(this ISiloBuilder builder, string name, Action<CouchbaseGrainStorageOptions> configureOptions)
        {
            return builder.ConfigureServices(services => services.AddCouchbaseGrainStorage(name, ob => ob.Configure(configureOptions)));
        }

        public static ISiloBuilder AddCouchbaseGrainStorageAsDefault(this ISiloBuilder builder, Action<OptionsBuilder<CouchbaseGrainStorageOptions>> configureOptions = null)
        {
            return builder.AddCouchbaseGrainStorage(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME, configureOptions);
        }

        public static ISiloBuilder AddCouchbaseGrainStorage(this ISiloBuilder builder, string name, Action<OptionsBuilder<CouchbaseGrainStorageOptions>> configureOptions = null)
        {
            return builder.ConfigureServices(services => services.AddCouchbaseGrainStorage(name, configureOptions));
        }

        internal static IServiceCollection AddCouchbaseGrainStorage(this IServiceCollection services, string name,
            Action<OptionsBuilder<CouchbaseGrainStorageOptions>> configureOptions = null)
        {
            configureOptions?.Invoke(services.AddOptions<CouchbaseGrainStorageOptions>(name));
            services.AddTransient<IConfigurationValidator>(sp => new CouchbaseGrainStorageOptionsValidator(sp.GetService<IOptionsSnapshot<CouchbaseGrainStorageOptions>>().Get(name), name));
            services.ConfigureNamedOptionForLogging<CouchbaseGrainStorageOptions>(name);
            services.TryAddSingleton<IGrainStorage>(sp => sp.GetServiceByName<IGrainStorage>(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME));
            return services.AddSingletonNamedService<IGrainStorage>(name, CouchbaseStorageFactory.Create)
                           .AddSingletonNamedService<ILifecycleParticipant<ISiloLifecycle>>(name, (s, n) => (ILifecycleParticipant<ISiloLifecycle>)s.GetRequiredServiceByName<IGrainStorage>(n));
        }
    }
}
