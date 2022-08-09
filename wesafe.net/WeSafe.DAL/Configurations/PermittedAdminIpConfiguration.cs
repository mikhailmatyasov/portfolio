using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeSafe.DAL.Entities;

namespace WeSafe.DAL.Configurations
{
    class PermittedAdminIpConfiguration : IEntityTypeConfiguration<PermittedAdminIp>
    {
        public void Configure(EntityTypeBuilder<PermittedAdminIp> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
