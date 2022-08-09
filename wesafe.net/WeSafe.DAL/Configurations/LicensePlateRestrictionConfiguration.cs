using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeSafe.DAL.Entities;

namespace WeSafe.DAL.Configurations
{

    public class LicensePlateRestrictionConfiguration : IEntityTypeConfiguration<LicensePlateRestriction>
    {
        public void Configure(EntityTypeBuilder<LicensePlateRestriction> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(p => p.LicensePlate).IsRequired().HasMaxLength(10);
        }
    }
}
