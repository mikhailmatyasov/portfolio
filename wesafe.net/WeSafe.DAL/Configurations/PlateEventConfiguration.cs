using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeSafe.DAL.Entities;

namespace WeSafe.DAL.Configurations
{
    public class PlateEventConfiguration : IEntityTypeConfiguration<PlateEvent>
    {
        public void Configure(EntityTypeBuilder<PlateEvent> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.PlateNumber).IsRequired();

            builder.HasMany(s => s.PlateEventState)
                .WithOne(e => e.PlateEvent)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
