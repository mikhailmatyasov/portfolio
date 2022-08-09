using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeSafe.DAL.Entities;

namespace WeSafe.DAL.Configurations
{
    class MobileDeviceConfiguration : IEntityTypeConfiguration<MobileDevice>
    {
        public void Configure(EntityTypeBuilder<MobileDevice> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.MobileUser)
                .WithMany(x => x.Devices)
                .HasForeignKey(x => x.MobileUserId);
        }
    }
}
