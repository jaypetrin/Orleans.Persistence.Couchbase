using System;
using System.Collections.Generic;
using System.Text;

namespace Storage.Couchbase
{
    public class GrainStateDocument
    {
        public Guid? Id { get; set; }
        public string Type => typeof(GrainStateDocument).Name;
    }
}
