using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeSafe.DAL.Entities;

namespace WeSafe.DAL.Configurations
{
    public class PlateEventStateConfiguration : IEntityTypeConfiguration<PlateEventState>
    {
        public void Configure(EntityTypeBuilder<PlateEventState> builder)
        {
            builder.HasKey(e => e.Id);
        }
    }
}
