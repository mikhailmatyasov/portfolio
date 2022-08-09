using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeSafe.DAL.Entities;

namespace WeSafe.DAL.Configurations
{
    class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.DisplayName).HasMaxLength(250);
            builder.Property(x => x.ClientId).IsRequired(false);
            builder.HasOne(x => x.Client)
                .WithMany(x => x.Users)
                .HasForeignKey(x => x.ClientId);
        }
    }
}
