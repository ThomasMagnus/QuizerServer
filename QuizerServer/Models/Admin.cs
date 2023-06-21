using QuizerServer.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quizer.Models
{
    public class Admin : BaseModel
    {
        public string? FName { get; set; }
        public string? LName { get; set; }
        public string? Login { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
