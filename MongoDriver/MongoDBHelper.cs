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

        public List<T> Find<T>(string dbName, string collectionName)
        {
            IMongoDatabase mongodb = GetDatabase(dbName);

            return mongodb
                .GetCollection<T>(collectionName)
                .Find<T>(null).ToList();            
        }

        public void Insert<T>(string dbName, string collectionName, T[] items)
        {
            IMongoDatabase mongodb = GetDatabase(dbName);

            mongodb
                .GetCollection<T>(collectionName)
                .InsertManyAsync(items)
                .Wait();
        }

        public void UpdateOne<T>(string dbName, string collectionName,  documentName, T documentMatchCriteria )
        {
            IMongoDatabase mongodb = GetDatabase(dbName);

            FilterDefinition<T> filter =  new FilterDefinitionBuilder<T>().Eq(documentName, documentMatchCriteria)
            mongodb
                .GetCollection<T>(collectionName)
                .UpdateOneAsync(item)
                .Wait();
        }

        private IMongoDatabase GetDatabase(string name)
        {
            return _mongoClient.GetDatabase(name);
        }
    }
}
