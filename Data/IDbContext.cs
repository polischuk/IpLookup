using MongoDB.Driver;

namespace Data
{
    public interface IDbContext
    {
        IMongoCollection<IpEntity> IpList { get; }
    }
}