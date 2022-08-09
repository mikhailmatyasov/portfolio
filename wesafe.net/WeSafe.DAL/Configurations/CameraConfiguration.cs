using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeSafe.DAL.Entities;

namespace WeSafe.DAL.Configurations
{
    class CameraConfiguration : IEntityTypeConfiguration<Camera>
    {
        public virtual void Configure(EntityTypeBuilder<Camera> builder)
        {
            builder.ToTable("Cameras");
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.CameraName).IsUnique(false);

            builder.Property(x => x.Id).ValueGeneratedOnAdd().IsRequired().HasMaxLength(16);
            builder.Property(x => x.CameraName).IsRequired().HasMaxLength(250);
            builder.Property(x => x.Port).IsRequired().HasMaxLength(10);
            builder.Property(x => x.Login).HasMaxLength(100);
            builder.Property(x => x.Password).HasMaxLength(50);
            builder.Property(x => x.Status).HasMaxLength(50);
            builder.Property(x => x.NetworkStatus).HasMaxLength(50);
            builder.Property(x => x.LastActivityTime).IsRequired(false);

            builder.HasMany(c => c.Traffic)
                .WithOne(d => d.Camera)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
