using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeSafe.DAL.Entities;

namespace WeSafe.DAL.Configurations
{
    class RtspPathConfiguration : IEntityTypeConfiguration<RtspPath>
    {
        public void Configure(EntityTypeBuilder<RtspPath> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.CameraMark)
                .WithMany(x => x.RtspPaths)
                .HasForeignKey(x => x.CameraMarkId);
        }
    }
}
