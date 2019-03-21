using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Storage.Couchbase
{
    public class GrainStateDataManager
    {
        private readonly CouchbaseDataManager dataManager;

        public GrainStateDataManager(CouchbaseGrainStorageOptions options)
        {
            dataManager = new CouchbaseDataManager(options);
        }

        public Task InitConnectionAsync()
        {
            return dataManager.InitConnection();
        }

        public async Task<GrainStateDocument> Read(string docId)
        {
            return await dataManager.ReadSingleEntryAsync(docId);
        }

        public async Task Write(GrainStateDocument document)
        {
            await dataManager.UpsertEntryAsync(document);
        }

        public async Task Delete(GrainStateDocument document)
        {
            await dataManager.DeleteEntryAsync(document.Id.ToString());
        }
    }
}
