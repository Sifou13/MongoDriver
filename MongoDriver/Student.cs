using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDriver
{
    public class Student
    {
        [BsonId]
        public int ID { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }
                
        [BsonElement("scores")]
        public ScoreSet[] Scores { get; set; }
    }

    public class ScoreSet
    {
        [BsonElement("type")]
        public string Type { get; set; }

        [BsonElement("score")]
        public double Score { get; set; }
    }
}

