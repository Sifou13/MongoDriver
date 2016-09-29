using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDriver
{
    public class Student
    {
        [MongoDB.Bson.Serialization.Attributes.BsonId]
        public int ID { get; set; }

        [MongoDB.Bson.Serialization.Attributes.BsonElement("name")]
        public string Name { get; set; }

        [MongoDB.Bson.Serialization.Attributes.BsonElement("scores")]
        public ScoreSet[] Scores { get; set; }
    }

    public class ScoreSet
    {
        [MongoDB.Bson.Serialization.Attributes.BsonElement("type")]
        public string Type { get; set; }

        [MongoDB.Bson.Serialization.Attributes.BsonElement("score")]
        public float Score { get; set; }
    }
}
}
