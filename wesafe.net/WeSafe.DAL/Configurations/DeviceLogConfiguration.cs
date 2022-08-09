using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeSafe.DAL.Entities;

namespace WeSafe.DAL.Configurations
{
    class DeviceLogConfiguration : IEntityTypeConfiguration<DeviceLog>
    {
        public void Configure(EntityTypeBuilder<DeviceLog> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(p => p.DateTime).HasColumnType("timestamptz");
            builder.Property(x => x.ErrorMessage).IsRequired();

            builder.HasOne(x => x.Camera)
                .WithMany(x => x.DeviceLogs)
                .HasForeignKey(x => x.CameraId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Device)
                .WithMany(x => x.DeviceLogs)
                .HasForeignKey(x => x.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
