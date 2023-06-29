using Microsoft.EntityFrameworkCore;
using Quizer.Context;
using Quizer.Models;
using QuizerServer.HelperInterfaces;

namespace QuizerServer.HelperClasses
{
    public class Services<T> where T : class
    {
        public ApplicationContext db;
        public T value;

        public Services(ApplicationContext db, T value) => (this.db, this.value) = (db, value);

        async public Task<IEnumerable<Users>> GetEntity()
        {
            return await db.Users.ToListAsync();
        }
    }

}
