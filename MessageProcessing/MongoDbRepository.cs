using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;

namespace MessageProcessing
{
    public class MongoDbRepository : IRepository
    {
        private readonly IMongoCollection<ServerStatistics> _collection;

        public MongoDbRepository()
        {
            var client = new MongoClient(AppConfiguration.ConnectionString);
            var database = client.GetDatabase(AppConfiguration.DatabaseName);

            _collection = database.GetCollection<ServerStatistics>(AppConfiguration.CollectionName);
        }
        public async Task InsertAsync(ServerStatistics entity)
        {
            await _collection.InsertOneAsync(entity);
        }
        public async Task<ServerStatistics> GetByIdAsync(Guid id)
        {
            var filter = Builders<ServerStatistics>.Filter.Eq("_id", id);

            return await _collection.Find(filter).SingleOrDefaultAsync();
        }
        public async Task<IEnumerable<ServerStatistics>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }
        public async Task UpdateAsync(Guid id, ServerStatistics entity)
        {
            var filter = Builders<ServerStatistics>.Filter.Eq("_id", id);

            await _collection.ReplaceOneAsync(filter, entity);
        }
        public async Task DeleteAsync(Guid id)
        {
            var filter = Builders<ServerStatistics>.Filter.Eq("_id", id);

            await _collection.DeleteOneAsync(filter);
        }
    }
}