using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeSafe.DAL.Entities;

namespace WeSafe.DAL.Configurations
{
    class CameraMarkConfiguration : IEntityTypeConfiguration<CameraMark>
    {
        public void Configure(EntityTypeBuilder<CameraMark> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.CameraManufacturer)
                .WithMany(x => x.CameraMarks)
                .HasForeignKey(x => x.CameraManufactorId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
