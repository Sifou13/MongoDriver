using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using System.Linq;
using MongoDB.Bson;
using System.Linq.Expressions;
using MongoDB.Bson.Serialization.Conventions;
namespace MongoDriver
{
    public class MongoDBHelper
    {
        private readonly MongoClient _mongoClient;

        public MongoDBHelper()
        {
            _mongoClient = new MongoClient("mongodb://Darwin:mongoDev@localhost:27017");
            ConventionRegistry.Register("IgnoreExtraElements", new ConventionPack { new IgnoreExtraElementsConvention(true) }, type => true);
        }

        public List<T> Find<T>(string dbName, string collectionName, T type)
        {
            IMongoDatabase mongodb = GetDatabase(dbName);

            return mongodb
                .GetCollection<T>(collectionName)
                .Find<T>(new BsonDocument()).ToList();            
        }

        public void Insert<T>(string dbName, string collectionName, T[] items)
        {
            IMongoDatabase mongodb = GetDatabase(dbName);

            mongodb
                .GetCollection<T>(collectionName)
                .InsertManyAsync(items)
                .Wait();
        }

        public void DeleteMany<T>(string dbName, string collectionName, Expression<Func<T, bool>> filter)
        {
            IMongoDatabase mongodb = GetDatabase(dbName);

            mongodb
                .GetCollection<T>(collectionName)
                .DeleteManyAsync<T>(filter)
                .Wait();
        }

        public void UpdateDocumentSection<T, TField>(string dbName, string collectionName,  Expression<Func<T, bool>> filter, Expression<Func<T, TField>> field, TField updatedField )
        {
            IMongoDatabase mongodb = GetDatabase(dbName);

            FieldDefinition<T, TField> fieldDefinition = new ExpressionFieldDefinition<T, TField>(field);

            UpdateDefinitionBuilder<T> updatedefBuilder = new UpdateDefinitionBuilder<T>();
            UpdateDefinition<T> updateDefinition = updatedefBuilder.Set(fieldDefinition, updatedField);
                mongodb
                .GetCollection<T>(collectionName)
                .UpdateOneAsync(filter,updateDefinition)
                .Wait();
        }

        public async Task<List<TField>> Project<T, TField>(string dbName, string collectionName, Expression<Func<T, bool>> filter, Expression<Func<T, TField>> field)
        {
            IMongoDatabase mongodb = GetDatabase(dbName);

            
            var ret = mongodb
            .GetCollection<T>(collectionName)
            .Find(filter).Limit(null);
            
            return await ret.Project(field).ToListAsync();
        }

        public async void CreateCollectionOptionsIndex<T>(T Type, string DBName, string collectionName, List<string> fields)
        {
            IMongoDatabase mongoDB = GetDatabase(DBName);

            IndexKeysDefinition<T> indexKeys = Builders<T>.IndexKeys.Combine(fields.Select(x => Builders<T>.IndexKeys.Ascending(x)));

            await mongoDB.GetCollection<T>(collectionName).Indexes.CreateOneAsync(indexKeys);
        }

        private IMongoDatabase GetDatabase(string name)
        {
            return _mongoClient.GetDatabase(name);
        }
    }
}
