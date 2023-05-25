using Microsoft.EntityFrameworkCore;
using Quizer.Context;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quizer.Models
{
    public class Sessions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }
        public int? UserId { get; set; }
        public string? UserFirstname { get; set; }
        public string? UserLastname { get; set; }
        public DateTime? Enterdate { get; set; }
        [ForeignKey("UserId")]
        public Users? User { get; set; }

        public static async void CreateSession(int? UserId, string? UserFirstname, string? UserLastname, DateTime? EnterData)
        {
            using SessionsContext sessionsContext = new();

            Sessions sessions = new Sessions()
            {
                UserId = UserId,
                UserFirstname = UserFirstname,
                UserLastname = UserLastname,
                Enterdate = EnterData
            };

            sessionsContext.Add(sessions);
            await sessionsContext.SaveChangesAsync();
        }
    }

    public class SessionsContext : ApplicationContext
    {
        public DbSet<Sessions> Sessions { get; set; }

        public SessionsContext()
            :base() {
            Database.EnsureCreated();
        }
    }
}
