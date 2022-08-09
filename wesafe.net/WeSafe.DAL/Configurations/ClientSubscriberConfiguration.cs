using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeSafe.DAL.Entities;

namespace WeSafe.DAL.Configurations
{
    class ClientSubscriberConfiguration : IEntityTypeConfiguration<ClientSubscriber>
    {
        public void Configure(EntityTypeBuilder<ClientSubscriber> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(c => new { c.ClientId, c.Phone }).IsUnique();

            builder.Property(u => u.Name).HasDefaultValue("Unnamed");
            builder.Property(x => x.Phone).IsRequired().HasMaxLength(20);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Password).HasMaxLength(50);

            builder.HasOne(x => x.Client)
                .WithMany(x => x.Subscribers)
                .HasForeignKey(x => x.ClientId);
        }
    }
}
