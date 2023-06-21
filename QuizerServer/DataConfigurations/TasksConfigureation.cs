using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quizer.Models;

namespace QuizerServer.DataConfigurations
{
    public class TasksConfigureation : IEntityTypeConfiguration<Tasks>
    {
        public void Configure(EntityTypeBuilder<Tasks> builder)
        {
            builder
                .HasOne(x => x.Teachers)
                .WithMany(x => x.Tasks)
                .HasForeignKey(x => x.teacherid);

            builder
                .HasOne(x => x.Subjects)
                .WithMany(x => x.Tasks)
                .HasForeignKey(x => x.subjectid);

            builder
                .HasOne(x => x.Groups)
                .WithMany(x => x.Tasks)
                .HasForeignKey(x => x.groupid);
        }
    }
}
