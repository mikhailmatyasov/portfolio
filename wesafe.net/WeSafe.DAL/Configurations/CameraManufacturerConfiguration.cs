using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeSafe.DAL.Entities;

namespace WeSafe.DAL.Configurations
{
    class CameraManufacturerConfiguration : IEntityTypeConfiguration<CameraManufacturer>
    {
        public void Configure(EntityTypeBuilder<CameraManufacturer> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
