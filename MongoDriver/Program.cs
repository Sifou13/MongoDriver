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

            Type myType2 = CreateNewtypeAndAddProperties2(properties.Values.Select(x => x.Key).ToArray());
            
            Type mytype = CreateNewTypeAndAddProperties(properties, typeof(ReportData));

            mongodbHelper.CreateCollectionOptionsIndex(mytype, "Local-TestDBSif", "ReportDataSif", properties.Keys.ToList());

            object dataRecord;
            
            for (int index = 1; index <= 100; index++)
            {
                int numberOfDataItems = random.Next(100);

                items = new List<string>();

                for (int itemIndex = 0; itemIndex < numberOfDataItems; itemIndex++)
                    items.Add(new string(Enumerable.Repeat(chars, random.Next(1000)).Select(s => s[random.Next(s.Length)]).ToArray()));

                dataRecord = Activator.CreateInstance(mytype);
                AssignPropertyValue(dataRecord, "DateOfBirth", new DateTime(1940, 01, 01).AddDays(random.Next(2500)));
                AssignPropertyValue(dataRecord, "JoiningDate", new DateTime(1940, 01, 01).AddDays(random.Next(2500)));
                AssignPropertyValue(dataRecord, "Age", random.Next(99));
                //AssignPropertyValue(dataRecord, "NumberOfDependants", random.Next(99));
                AssignPropertyValue(dataRecord, "Data", items.ToArray());
                AssignPropertyValue(dataRecord, "ID", Guid.NewGuid());

                var dataRecord2 = Activator.CreateInstance(myType2);
                AssignPropertyValue(dataRecord2, "SortableProperty1", new DateTime(1940, 01, 01).AddDays(random.Next(2500)));
                AssignPropertyValue(dataRecord2, "SortableProperty2", new DateTime(1940, 01, 01).AddDays(random.Next(2500)));
                AssignPropertyValue(dataRecord2, "SortableProperty3", random.Next(99));
                //AssignPropertyValue(dataRecord2, "NumberOfDependants", random.Next(99));
                AssignPropertyValue(dataRecord2, "Data", items.ToArray());
                AssignPropertyValue(dataRecord2, "ID", Guid.NewGuid());
                
                mongodbHelper.Insert<object>("Local-TestDBSif", "ReportDataSif_Emit", new object[] { dataRecord });
                mongodbHelper.Insert<object>("Local-TestDBSif", "ReportDataSif_Generic", new object[] { dataRecord2 });
            }

            dataRecord = Activator.CreateInstance(mytype);
            List<object> results = mongodbHelper.Find("Local-TestDBSif", "ReportDataSif", dataRecord);

            foreach(object result in results)
            {
                Convert.ChangeType(result, mytype);
                Console.WriteLine(result);
            }

        }

        private static Type CreateNewtypeAndAddProperties2(Type[] sortablePropertyTypes)
        {
            return typeof(SortableReportData<,,>).MakeGenericType(sortablePropertyTypes);
        }

        private static Type CreateNewTypeAndAddProperties(Dictionary<string, KeyValuePair<Type, object>> properties, Type baseType)
        {
            ModuleBuilder moduleBuilder;

            Type type = typeof(ReportData);

            string typeName = string.Concat(type.FullName, "_Runtime");

            var assemblyName = new AssemblyName(string.Concat(type.Namespace, ".", type.Name, "_runtime"));
            
            AssemblyBuilder ab = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            moduleBuilder = ab.DefineDynamicModule(assemblyName.Name, string.Concat(assemblyName.Name, ".dll"));
            
            TypeBuilder typeBuilder = moduleBuilder.DefineType(typeName, TypeAttributes.Public);
            
            typeBuilder.SetParent(baseType);
            
            GenerateConstructor(typeBuilder, baseType);

            foreach (string property in properties.Keys)
            {
                CreatePropertyAndAddToType(typeBuilder, property, properties[property].Key);
            }

            Type newType = typeBuilder.CreateType();

            return newType;
        }

        private static void GenerateConstructor(TypeBuilder typeBuilder, Type baseType)
        {
            ConstructorInfo baseConstructor = baseType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).First();
            ConstructorBuilder constructor = typeBuilder.DefineConstructor(MethodAttributes.Public, baseConstructor.CallingConvention, null);
            ILGenerator ilGenerator = constructor.GetILGenerator();
            ilGenerator.Emit(OpCodes.Nop);
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Call, baseConstructor);
            ilGenerator.Emit(OpCodes.Ret);
        }

        private static void CreatePropertyAndAddToType(TypeBuilder myTypeBuilder, string propertyName, Type type)
        {
            FieldBuilder fieldBuilder = myTypeBuilder.DefineField(propertyName.ToLower(),
                                                        type,
                                                        FieldAttributes.Private);

            PropertyBuilder propertyBuilder = myTypeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, type, null);

            MethodAttributes getSetAttributes = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

            MethodBuilder getPropertyMethodBuilder = myTypeBuilder.DefineMethod(string.Concat("get_", propertyName), getSetAttributes, type, Type.EmptyTypes);

            ILGenerator getPropertyMethodIL = getPropertyMethodBuilder.GetILGenerator();

            getPropertyMethodIL.Emit(OpCodes.Ldarg_0);
            getPropertyMethodIL.Emit(OpCodes.Ldfld, fieldBuilder);
            getPropertyMethodIL.Emit(OpCodes.Ret);

            MethodBuilder setPropertyMethodBuilder = myTypeBuilder.DefineMethod(string.Concat("set_", propertyName), getSetAttributes, null, new Type[] { type });

            ILGenerator setPropertyMethodIL = setPropertyMethodBuilder.GetILGenerator();

            setPropertyMethodIL.Emit(OpCodes.Ldarg_0);
            setPropertyMethodIL.Emit(OpCodes.Ldarg_1);
            setPropertyMethodIL.Emit(OpCodes.Stfld, fieldBuilder);
            setPropertyMethodIL.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getPropertyMethodBuilder);
            propertyBuilder.SetSetMethod(setPropertyMethodBuilder);
        }

        private static void AssignPropertyValue(object instance, string propertyName, object value)
        {
            PropertyInfo prop = instance.GetType().GetProperty(propertyName);
            prop.SetValue(instance, value);
        }

        private static void AssignPropertyValue2(object instance, string propertyName, object value)
        {
            PropertyInfo prop = instance.GetType().GetProperty(propertyName);
            prop.SetValue(instance, value);
        }

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
