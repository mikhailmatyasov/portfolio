using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeSafe.DAL.Entities;

namespace WeSafe.DAL.Configurations
{
    public class DetectedCameraConfiguration : IEntityTypeConfiguration<DetectedCamera>
    {
        public void Configure(EntityTypeBuilder<DetectedCamera> builder)
        {
            builder.ToTable("DetectedCameras");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Ip).IsRequired().HasMaxLength(15);
            builder.Property(x => x.Port).IsRequired().HasMaxLength(10);
            builder.Property(x => x.Login).HasMaxLength(100);
            builder.Property(x => x.Password).HasMaxLength(50);
        }
    }
}