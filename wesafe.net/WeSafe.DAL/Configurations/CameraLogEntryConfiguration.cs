using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeSafe.DAL.Entities;

namespace WeSafe.DAL.Configurations
{
    class CameraLogEntryConfiguration : IEntityTypeConfiguration<CameraLogEntry>
    {
        public void Configure(EntityTypeBuilder<CameraLogEntry> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.UrlExpiration).IsRequired(false);

            builder.HasOne(x => x.CameraLog)
                .WithMany(x => x.Entries)
                .HasForeignKey(x => x.CameraLogId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
