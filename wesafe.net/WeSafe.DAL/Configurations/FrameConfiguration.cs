using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeSafe.DAL.Entities;

namespace WeSafe.DAL.Configurations
{
    public class FrameConfiguration : IEntityTypeConfiguration<Frame>
    {
        public void Configure(EntityTypeBuilder<Frame> builder)
        {
            builder.HasKey(f => f.Id);
            builder.HasOne(f => f.PlateEvent)
                .WithMany(e => e.Frames)
                .HasForeignKey(f => f.PlateEventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(f => f.ImageUrl).IsRequired();
        }
    }
}
