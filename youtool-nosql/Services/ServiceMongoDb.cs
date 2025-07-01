using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace youtool_nosql.Services
{
    public class ServiceMongoDb
    {
        private readonly IMongoDatabase _database;

        public ServiceMongoDb(IConfiguration configuration)
        {
            var connectionString = configuration["MongoDB:ConnectionString"];
            var databaseName = configuration["MongoDB:DatabaseName"];

            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            return _database.GetCollection<T>(name);
        }
    }
}
