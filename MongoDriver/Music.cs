using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDriver
{
    public class Album
    {
        [BsonId]
        public Int32 ID { get; private set; }

        [BsonElement("images")]
        public Int32[] ImageIDs { get; set; }
    }

    
    public class Image
    {
        [BsonId]
        public Int32 ID { get; set; }

        [BsonElement("tags")]
        public string[] Tags { get; set; }

    }
}
