using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeSafe.DAL.Entities;

namespace WeSafe.DAL.Configurations
{
    class CameraLogConfiguration : IEntityTypeConfiguration<CameraLog>
    {
        public void Configure(EntityTypeBuilder<CameraLog> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(c => c.Time);

            builder.HasOne(x => x.Camera)
                .WithMany(x => x.CameraLogs)
                .HasForeignKey(x => x.CameraId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
