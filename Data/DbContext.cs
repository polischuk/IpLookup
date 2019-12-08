using MongoDB.Driver;

namespace Data
{
    public class DbContext : IDbContext
    {
        private readonly IMongoDatabase _db;
        public DbContext(IMongoDbConfig config)
        {
            var client = new MongoClient(config.ConnectionString);
            _db = client.GetDatabase(config.Database);
        }

        public IMongoCollection<IpEntity> IpList => _db.GetCollection<IpEntity>("IpList");
    }
}