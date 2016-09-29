using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver.Core.Bindings;
namespace MongoDriver
{
    public class Person
    {
        [MongoDB.Bson.Serialization.Attributes.BsonId]
        public Guid ID { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Address { get; set; }

        public Occupation Occupation { get; set; }
    }

    public enum Occupation
    {
        SoftwareEngineer = 0,
        Doctor = 1,
        PoliceOfficer = 2,
        PA = 3,
        Nurse = 4
    }
}
