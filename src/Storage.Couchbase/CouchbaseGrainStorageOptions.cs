using Newtonsoft.Json;
using Orleans;
using System;
using System.Collections.Generic;
using System.Text;

namespace Storage.Couchbase
{
    public class CouchbaseGrainStorageOptions
    {
        public List<Uri> Uris { get; set; }

        public string BucketName { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public bool DeleteStateOnClear { get; set; }

        public TypeNameHandling? TypeNameHandling { get; set; }

        public bool IndentJson { get; set; }

        public bool UseFullAssemblyNames { get; set; }

        /// <summary>
        /// Stage of silo lifecycle where storage should be initialized.  Storage must be initialized prior to use.
        /// </summary>
        public int InitStage { get; set; } = DEFAULT_INIT_STAGE;
        public const int DEFAULT_INIT_STAGE = ServiceLifecycleStage.ApplicationServices;
    }
}
