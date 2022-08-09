using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeSafe.DAL.Entities;

namespace WeSafe.DAL.Configurations
{
    class ClientSubscriberAssignmentConfiguration : IEntityTypeConfiguration<ClientSubscriberAssignment>
    {
        public void Configure(EntityTypeBuilder<ClientSubscriberAssignment> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(c => c.Camera)
                .WithMany(x => x.Assignments)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.Device)
                .WithMany(x => x.Assignments);

            builder.HasOne(c => c.Subscriber)
                .WithMany(x => x.Assignments);
        }
    }
}
