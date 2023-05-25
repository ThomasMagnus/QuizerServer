using Quizer.Models;
using Microsoft.EntityFrameworkCore;
using Quizer.Context;

namespace QuizerServer.HelperInterfaces
{
    public interface IServices<T> where T : class
    {
        public ApplicationContext? db { get; set; }
        public Task<List<T>> EntityLIst();
        public IEnumerable<T> GetEntity();
        public Task<T?> GetEntity(Dictionary<string, object> value);
        public Task AddEntity(Dictionary<string, object> value);
    }
}
