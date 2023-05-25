using Microsoft.EntityFrameworkCore;
using Quizer.Context;
using QuizerServer.HelperInterfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quizer.Models
{
    public class Users
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? Patronymic { get; set; }
        public string? Password { get; set; }
        public int? GroupsId { get; set; }
        public Groups? Groups { get; set; }
        public List<Sessions>? Sessions { get; set; } = new();
    }

    public class UsersServices : IServices<Users>
    {
        public ApplicationContext? db { get; set; }

        async public Task AddEntity(Dictionary<string, object> value) 
        {
            try
            {
                await db!.Users.AddAsync(new Users()
                {
                    Firstname = value["firstname"].ToString(),
                    Lastname = value["lastname"].ToString(),
                    Password = value["Password"].ToString(),
                    GroupsId = int.Parse(value["GroupsId"].ToString()!)
                });

                await db.SaveChangesAsync();
            }
            catch
            {
                Console.WriteLine("Какое-то из значений не было передано!");
            }
        }

        async public Task<List<Users>> EntityLIst()
        {
            return await db!.Users.ToListAsync();
        }

        public IEnumerable<Users> GetEntity()
        {
            return db!.Users.Select(x => x);
        }

        public async Task<Users?> GetEntity(Dictionary<string, object>? value)
        {
            try
            {
                Users? user = await db!.Users.FirstOrDefaultAsync(x => x.Firstname!.ToLower() == value!["firstname"].ToString()!.ToLower()
                                                                    && x.Lastname!.ToLower() == value["lastname"].ToString()!.ToLower()
                                                                    && x.Patronymic!.ToLower() == value["patronymic"].ToString()!.ToLower()
                                                                    && x.Password == value["password"].ToString()
                                                                    && x.GroupsId == int.Parse(value["group"].ToString()!));

                return user;
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }

            throw new NotImplementedException();
        }
    }
}