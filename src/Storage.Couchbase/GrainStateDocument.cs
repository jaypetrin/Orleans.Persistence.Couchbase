namespace Storage.Couchbase
{
    public class GrainStateDocument
    {
        //Combination of GrainId and GrainType
        public string Id { get; set; }

        //Serialized JSON data stored as a binary in Couch
        public byte[] Data { get;set;}
    }
}
