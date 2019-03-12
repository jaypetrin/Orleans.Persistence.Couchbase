using Orleans.Runtime;
using System;

namespace Storage.Couchbase
{
    public class CouchbaseGrainStorageException : Exception
    {
        private const string DefaultMessageFormat = "Couchbase storage Exception.  GrainType: {0}, GrainId: {1}, BucketName: {2}";

        public CouchbaseGrainStorageException(string message, string grainType, GrainReference grainId, string bucketName, Exception ex) : base(message, ex)
        {
            GrainType = grainType;
            GrainId = grainId.ToKeyString();
            BucketName = bucketName;
        }

        public CouchbaseGrainStorageException(
            string grainType,
            GrainReference grainId,
            string bucketName,
            Exception couchException)
            : this(CreateDefaultMessage(grainType, grainId, bucketName), grainType, grainId, bucketName, couchException)
        {
        }

        public string GrainId { get; }
        public string GrainType { get; set; }
        public string BucketName { get; set; }

        private static string CreateDefaultMessage(
           string grainType,
           GrainReference grainId,
           string tableName)
        {
            return string.Format(DefaultMessageFormat, grainType, grainId, tableName);
        }
    }
}
