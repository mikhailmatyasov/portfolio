using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeSafe.DAL.Entities;

namespace WeSafe.DAL.Configurations
{
    class MobileUserConfiguration : IEntityTypeConfiguration<MobileUser>
    {
        public void Configure(EntityTypeBuilder<MobileUser> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(c => c.Phone).IsUnique();

            builder.Property(x => x.Phone).IsRequired().HasMaxLength(20);
            builder.Property(x => x.Mute).IsRequired(false);
        }
    }
}
