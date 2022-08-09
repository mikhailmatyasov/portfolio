using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeSafe.DAL.Entities;

namespace WeSafe.DAL.Configurations
{
    class ClientConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).HasMaxLength(250);
            builder.Property(x => x.Phone).HasMaxLength(50);
            builder.HasIndex(c => new { c.CreatedAt, c.IsActive });
            builder.HasIndex(c => c.Token);
        }
    }
}
