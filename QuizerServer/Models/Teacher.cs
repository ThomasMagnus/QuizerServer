﻿using Microsoft.EntityFrameworkCore;
using Quizer.Context;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quizer.Models
{
    public class Teacher
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int id { get; set; }

        public string? fname { get; set; }
        public string? lname { get; set; }
        public string? pname { get; set; }
        public string? login { get; set; }
        public string? password { get; set; }
        public virtual ICollection<Tasks>? Tasks { get; set; }
        public virtual ICollection<TeacherProps>? TeacherProps { get; set; }
    }

}
