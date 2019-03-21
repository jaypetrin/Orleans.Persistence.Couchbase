using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Orleans;
using Orleans.Configuration;
using Orleans.Runtime;
using Orleans.Serialization;
using Orleans.Storage;

namespace Storage.Couchbase
{
    public class CouchbaseGrainStorage : IGrainStorage, ILifecycleParticipant<ISiloLifecycle>
    {
        private readonly CouchbaseGrainStorageOptions _options;
        private readonly ClusterOptions _clusterOptions;
        private readonly IGrainFactory _grainFactory;
        private readonly ITypeResolver _typeResolver;
        private readonly string _name;
        private readonly SerializationManager _serializationManager;

        private JsonSerializerSettings _jsonSettings;
        private GrainStateDataManager _dataManager;

        public CouchbaseGrainStorage(
            string name,
            CouchbaseGrainStorageOptions options,
            IOptions<ClusterOptions> clusterOptions,
            IGrainFactory grainFactory,
            ITypeResolver typeResolver,
            SerializationManager serializationManager)
        {
            _options = options;
            _name = name;
            _clusterOptions = clusterOptions.Value;
            _grainFactory = grainFactory;
            _typeResolver = typeResolver;
            _serializationManager = serializationManager;
        }

        public async Task ClearStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            if (_dataManager == null)
                throw new ArgumentException("GrainState-Bucket property not initialized");

            string pk = GetKeyString(grainReference);
           
            var entity = new GrainStateDocument { Id = pk };
            string operation = "Clearing";
            try
            {
                if (_options.DeleteStateOnClear)
                {
                    operation = "Deleting";
                    await DoOptimisticUpdateAsync(() => _dataManager.Delete(entity), grainType, grainReference, _options.BucketName).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task ReadStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            if (_dataManager == null)
                throw new ArgumentException("GrainState-Couchbase property not initialized");

            string pk = GetKeyString(grainReference);
            var doc = await _dataManager.Read(pk).ConfigureAwait(false);
            if (doc != null)
            {
                var stateType = grainState.State.GetType();
                var loadedState = ConvertFromStorageFormat(doc.Data);
            }
        }

        public async Task WriteStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            if (_dataManager == null)
                throw new ArgumentException("GrainState-Couchbase property not initialized");

            string pk = GetKeyString(grainReference);
           
            var contents = ConvertToStorageFormat(grainState);
            var document = new GrainStateDocument { Id = pk, Data = contents };

            try
            {
                await DoOptimisticUpdateAsync(() => _dataManager.Write(document), grainType, grainReference, _options.BucketName).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private Task Init(CancellationToken ct)
        {
            try
            {
                _jsonSettings = OrleansJsonSerializer
                    .UpdateSerializerSettings(OrleansJsonSerializer.GetDefaultSerializerSettings(_typeResolver, _grainFactory),
                        _options.UseFullAssemblyNames, _options.IndentJson, _options.TypeNameHandling);
                _dataManager = new GrainStateDataManager(_options);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Task.CompletedTask;
        }

        private string GetKeyString(GrainReference grainReference)
        {
            return String.Format("{0}_{1}", this._clusterOptions.ServiceId, grainReference.ToKeyString());
        }

        private static async Task DoOptimisticUpdateAsync(Func<Task> updateOperation, string grainType, GrainReference grainReference, string bucketName)
        {
            try
            {
                await updateOperation.Invoke().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new CouchbaseGrainStorageException(ex.Message, grainType, grainReference, bucketName, ex);
            }
        }

        //Converts from the State Model to the binary Storage Model
        internal object ConvertFromStorageFormat(byte[] contents)
        {
            object result;
            var str = Encoding.UTF8.GetString(contents);
            try
            {
                result = JsonConvert.DeserializeObject<object>(str, _jsonSettings);
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return result;
        }

        internal byte[] ConvertToStorageFormat(object grainState)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(grainState, _jsonSettings));
        }

        public void Participate(ISiloLifecycle lifecycle)
        {
            lifecycle.Subscribe(OptionFormattingUtilities.Name<CouchbaseGrainStorage>(_name), _options.InitStage, Init);
        }
    }
}