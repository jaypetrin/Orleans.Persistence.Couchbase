using System;

namespace Storage.Couchbase
{
    public class CouchbaseGrainStorageException : Exception
    {
        public CouchbaseGrainStorageException(string message) : base(message)
        {
        }
    }
}
