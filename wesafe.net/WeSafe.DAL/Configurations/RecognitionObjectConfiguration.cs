using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeSafe.DAL.Entities;

namespace WeSafe.DAL.Configurations
{
    public class RecognitionObjectConfiguration : IEntityTypeConfiguration<RecognitionObject>
    {
        public void Configure(EntityTypeBuilder<RecognitionObject> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(50);
        }
    }
}