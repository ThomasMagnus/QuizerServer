using Quizer.Context;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Quizer.Models
{
    public class Subjects
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Name { get; set; }
        public ICollection<Tasks>? Tasks { get; set; }
        public ICollection<TeacherProps>? TeacherProps { get; set; }
    }

}
