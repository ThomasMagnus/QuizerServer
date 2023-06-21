using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quizer.Models;

namespace QuizerServer.DataConfigurations
{
    public class TeacherPropsConfiguration : IEntityTypeConfiguration<TeacherProps>
    {
        public void Configure(EntityTypeBuilder<TeacherProps> builder)
        {
            builder.HasKey(x => x.id);

            builder
                .HasOne(x => x.Subjects)
                .WithMany(x => x.TeacherProps)
                .HasForeignKey(x => x.subjectsid);

            builder
                .HasOne(x => x.Teacher)
                .WithMany(x => x.TeacherProps)
                .HasForeignKey(x => x.teacherid);
        }
    }
}
