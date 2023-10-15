namespace MessageProcessing
{
    public interface IRepository
    {
        Task InsertAsync(ServerStatistics entity);
        Task<ServerStatistics> GetByIdAsync(Guid id);
        Task<IEnumerable<ServerStatistics>> GetAllAsync();
        Task UpdateAsync(Guid id, ServerStatistics entity);
        Task DeleteAsync(Guid id);
    }
}