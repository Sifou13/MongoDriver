using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using DataTypes;
using System.Reflection;
using System.Reflection.Emit;

namespace MongoDriver
{
    class Program
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        static void Main(string[] args)
        {
            List<string> items;
            MongoDBHelper mongodbHelper = new MongoDBHelper();
            Random random = new Random();
            Dictionary<string, KeyValuePair<Type, object>> properties = new Dictionary<string, KeyValuePair<Type, object>>();

            properties.Add("DateOfBirth", new KeyValuePair<Type, object>(typeof(DateTime), new DateTime(1980, 05, 13)));
            properties.Add("JoiningDate", new KeyValuePair<Type, object>(typeof(DateTime), new DateTime(2011, 01, 03)));
            properties.Add("Age", new KeyValuePair<Type, object>(typeof(int), 1547896));
            //properties.Add("NumberOfDependants", new KeyValuePair<Type, object>(typeof(int), 5));

            Type myType = CreateNewtypeAndAddProperties(properties.Values.Select(x => x.Key).ToArray());
           
            mongodbHelper.CreateCollectionOptionsIndex(myType, "Local-TestDBSif", "ReportDataSif_Generic", myType.GetProperties().Select(x => x.Name).ToList());

            object dataRecord;
            
            for (int index = 1; index <= 100; index++)
            {
                int numberOfDataItems = random.Next(100);

                items = new List<string>();

                for (int itemIndex = 0; itemIndex < numberOfDataItems; itemIndex++)
                    items.Add(new string(Enumerable.Repeat(chars, random.Next(1000)).Select(s => s[random.Next(s.Length)]).ToArray()));
                
                dataRecord = Activator.CreateInstance(myType);

                
                //dataRecord as Type 

                //AssignPropertyValue(dataRecord, "SortableProperty1", properties.Values.Select(x => x.Value).ToArray(), properties.Values.Select(x => x.Key).ToArray());
                
                //AssignPropertyValue(dataRecord2, "NumberOfDependants", random.Next(99));
                ((ReportData)dataRecord).Data = items.ToArray();
                ((ReportData)dataRecord).ID = Guid.NewGuid();
                
                mongodbHelper.Insert<object>("Local-TestDBSif", "ReportDataSif_Generic", new object[] { dataRecord });
            }

            dataRecord = Activator.CreateInstance(myType);
            List<object> results = mongodbHelper.Find("Local-TestDBSif", "ReportDataSif", dataRecord);

            foreach(object result in results)
            {
                Convert.ChangeType(result, myType);
                Console.WriteLine(result);
            }
        }

        private static Type CreateNewtypeAndAddProperties(Type[] sortablePropertyTypes)
        {
            return typeof(SortableReportData<,,>).MakeGenericType(sortablePropertyTypes);
        }
        
        //private static void AssignPropertyValue<T>(T instance, string property, object[] values, Type[] propertyTypes)
        //{
        //    MethodInfo method = typeof(SortableReportDataExtensions).GetMethod("SetSortableProperties", instance.GetType().GetGenericArguments());

            
        //    method.Invoke(instance, values);

        //    //PropertyInfo prop = instance.GetType().GetProperty(property);

        //    //prop.SetValue(instance, value);
        //}

        //static void Main(string[] args)
        //{
        //    new Random().Next(1, 2) == 1
        //        HashSet<Int32> imageIds = new HashSet<Int32>();

        //    MongoDBHelper mongodbHelper = new MongoDBHelper();

        //    mongodbHelper.Find<Album>("photodb", "albums");

        //    var test = mongodbHelper.Project<Album, Int32[]>("photodb", "albums", x => true, x => x.ImageIDs);
        //    //List<object[]> images = mongodbHelper.Project<Album, object[]>("photodb", "albums", x => true, x => (object[])x.ImageIDs).Result;
        //    //List<int> images = mongodbHelper.Project<Album, int>("photodb", "albums", x => true, x => x.ImageIDs).Result;

        //    int counter = 0;

        //    foreach (Int32[] array in test.Result)
        //    {
        //        foreach (Int32 imageID in array)
        //        {
        //            imageIds.Add(imageID);
        //            counter++;
        //        }
        //    }

        //    var imageswithsunsetBefore = mongodbHelper.Project<Image, Int32>("photodb", "images", x => x.Tags.Contains("sunrises"), x => x.ID).Result;

        //    var images = mongodbHelper.Project<Image, Int32>("photodb", "images", x => true, x => x.ID).Result;

        //    var imagesTodelete = images.Except(imageIds).ToList();

        //    mongodbHelper.DeleteMany<Image>("photodb", "images", x => imagesTodelete.Contains(x.ID));

        //    var imageswithsunsetPost = mongodbHelper.Project<Image, Int32>("photodb", "images", x => x.Tags.Contains("sunrises"), x => x.ID).Result;

        //    Console.WriteLine("counter: " + counter);
        //    Console.ReadKey();
        //}

        //static void Main(string[] args)
        //{
        //    MongoDBHelper mongodbHelper = new MongoDBHelper();


        //    mongodbHelper.Find<Student>("school", "students").ForEach(x =>
        //    {
        //        List<ScoreSet> studentScores = x.Scores.ToList();

        //        List<ScoreSet> homeworkScoreSets = studentScores.Where(score => score.Type == "homework").ToList();

        //        double scoreToRemove = homeworkScoreSets.Min(scoreSet => scoreSet.Score);

        //        ScoreSet scoreSetToRemove = homeworkScoreSets.First(y => y.Type == "homework" && y.Score == scoreToRemove);

        //        studentScores.Remove(scoreSetToRemove);

        //        mongodbHelper.UpdateDocumentSection<Student, ScoreSet[]>("school", "students", y => y.ID == x.ID, y => y.Scores, studentScores.ToArray());
        //    });

        //    Console.ReadKey();
        //}
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
