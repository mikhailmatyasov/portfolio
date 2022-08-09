using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeSafe.DAL.Entities;

namespace WeSafe.DAL.Configurations
{
    class UnhandledExceptionConfiguration : IEntityTypeConfiguration<UnhandledException>
    {
        public void Configure(EntityTypeBuilder<UnhandledException> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ErrorMessage).IsRequired();
            builder.Property(x => x.StackTrace).IsRequired();

            builder.HasOne(x => x.User)
                .WithMany(x => x.Exceptions)
                .HasForeignKey(x => x.UserId);
        }
    }
}
