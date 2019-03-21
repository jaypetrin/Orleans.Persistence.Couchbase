using System;
using System.Collections.Generic;
using System.Text;

namespace GrainInterfaces
{
    public class EnvConfig
    {
        public List<Uri> CouchbaseUris { get; set; }
        public string CouchbaseUserName { get; set; }
        public string CouchbasePassword { get; set; }
        public string CouchbaseBucketName { get; set; }
    }
}
