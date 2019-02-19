using System;
using System.Collections.Generic;
using System.Text;

namespace NCoreEventServer.SqlStore.Models
{
    public class ObjectEntity
    {
        public string ObjectType { get; set; }

        public string ObjectId { get; set; }

        public string ObjectJson { get; set; }
    }
}
