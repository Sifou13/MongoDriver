using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using System.Linq;
using MongoDB.Bson;

namespace MongoDriver
{
    public class MongoDBHelper
    {
        private readonly MongoClient _mongoClient;

        public MongoDBHelper()
        {
            _mongoClient = new MongoClient("mongodb://localhost:27017");
        }

        public void Insert<T>(string dbName, string collectionName, T item)
        {
            IMongoDatabase mongodb = GetDatabase(dbName);
            mongodb
                .GetCollection<T>(collectionName, new MongoCollectionSettings { AssignIdOnInsert = true, GuidRepresentation = GuidRepresentation.CSharpLegacy })
                .InsertOneAsync(item)
                .Wait();            
        }

        public void Insert<T>(string dbName, string collectionName, T[] items)
        {
            IMongoDatabase mongodb = GetDatabase(dbName);

            mongodb
                .GetCollection<T>(collectionName)
                .InsertManyAsync(items)
                .Wait();
        }

        private IMongoDatabase GetDatabase(string name)
        {
            return _mongoClient.GetDatabase(name);
        }
    }
}
