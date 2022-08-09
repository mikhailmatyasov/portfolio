using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeSafe.DAL.Entities;

namespace WeSafe.DAL.Configurations
{
    class ClientSubscriberSettingsConfiguration : IEntityTypeConfiguration<ClientSubscriberSettings>
    {
        public void Configure(EntityTypeBuilder<ClientSubscriberSettings> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Mute).IsRequired(false);

            builder.HasOne(x => x.Camera)
                .WithMany(x => x.SubscriberSettings)
                .HasForeignKey(x => x.CameraId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.ClientSubscriber)
                .WithMany(x => x.Settings)
                .HasForeignKey(x => x.ClientSubscriberId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
