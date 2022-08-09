using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeSafe.DAL.Entities;
using WeSafe.Shared.Extensions;

namespace WeSafe.DAL.Configurations
{
    class DeviceConfiguration : IEntityTypeConfiguration<Device>
    {
        public void Configure(EntityTypeBuilder<Device> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.User)
                .WithMany(x => x.Devices)
                .HasForeignKey(x => x.CreatedBy);
            builder.HasIndex(c => c.Token);
            builder.Property(p => p.Name).HasDefaultValue("Unnamed");
            builder.Property(p => p.CurrentSshPassword).HasDefaultValue("P@$$w0rd".Encrypt()).IsRequired();
            builder.Property(x => x.ActivationDate).IsRequired(false);
            builder.Property(x => x.AssemblingDate).IsRequired(false);
            builder.Property(x => x.NetworkStatus).HasMaxLength(50);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        }
    }
}
