using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDriver
{
    class Program
    {
        static void Main(string[] args)
        {
            MongoDBHelper mongodbHelper = new MongoDBHelper();

            
            mongodbHelper.Find<Student>("test", "people").ForEach(x =>
            {
                IEnumerable<ScoreSet> homeworkScoreSets = x.Scores.Where(score => score.Type == "homework");

                float scoreToRemove = homeworkScoreSets.Min(scoreSet => scoreSet.Score);

                mongodbHelper.
            });

            Console.ReadKey();
        }
        //static void Main(string[] args)
        //{
        //    MongoDBHelper mongodbHelper = new MongoDBHelper();

        //    Person georges = new Person
        //    {
        //        ID = Guid.NewGuid(),
        //        Firstname = "Gheorge",
        //        Lastname = "Suciu",
        //        Address = "Cluj, Romania",
        //        DateOfBirth = new DateTime(1980, 05, 13),
        //        Occupation = Occupation.Nurse
        //    };

        //    mongodbHelper.Insert<Person>("test", "people", georges);

        //    Console.ReadKey();
        //}
    }
}
