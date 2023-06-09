﻿using Microsoft.EntityFrameworkCore;
using Quizer.Context;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quizer.Models
{
    public class TeacherProps
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int subjectsid { get; set; }
        public int[]? groupsid { get; set; }
        public int teacherid { get; set; }

        [ForeignKey("teacherid")]
        public Teacher? Teacher { get; set; }

        [ForeignKey("subjectsid")]
        public Subjects? Subjects { get; set; }
    }
}
