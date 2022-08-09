using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeSafe.DAL.Entities;

namespace WeSafe.DAL.Configurations
{
    class TrafficEventConfiguration : IEntityTypeConfiguration<TrafficEvent>
    {
        public void Configure(EntityTypeBuilder<TrafficEvent> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Camera)
                .WithMany(x => x.Traffic)
                .HasForeignKey(x => x.CameraId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
