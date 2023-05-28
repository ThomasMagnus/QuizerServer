using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quizer.Models
{
    [Table("Tasks")]
    public class Tasks
    { 
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("Filepath")]
        public string? Filepath { get; set; }

        [Column("Filename")]
        public string? Filename { get; set; }

        [Column("Putdate")]
        public DateTime? Putdate { get; set; }
        //[Column("subjectid")]
        public int? subjectid { get; set; }

        //[Column("groupid")]
        public int? groupid { get; set; }
        public int? teacherid { get; set; }

        [ForeignKey("subjectid")]
        public Subjects? Subjects { get; set; }

        [ForeignKey("groupid")]
        public Groups? Groups { get; set; }

        [ForeignKey("teacherid")]
        public Teacher? Teacher { get; set; }
    }
}
