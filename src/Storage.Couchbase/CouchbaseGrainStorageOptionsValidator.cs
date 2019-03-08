using Orleans;

namespace Storage.Couchbase
{
    public class CouchbaseGrainStorageOptionsValidator : IConfigurationValidator
    {
        private readonly CouchbaseGrainStorageOptions _options;
        private readonly string _name;

        public CouchbaseGrainStorageOptionsValidator(CouchbaseGrainStorageOptions options, string name)
        {
            _name = name;
            _options = options;
        }

        public void ValidateConfiguration()
        {
            //Check for connectionString valid

            //Throw OrleansConfigurationException if its no good
        }
    }
}
