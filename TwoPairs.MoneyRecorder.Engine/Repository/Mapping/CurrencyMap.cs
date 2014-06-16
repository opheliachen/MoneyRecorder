using System.Data.Entity.ModelConfiguration;

namespace TwoPairs.MoneyRecorder.Data
{
    public class CurrencyMap : EntityTypeConfiguration<Currency>
    {
        public CurrencyMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Name)
                .HasMaxLength(512);

            // Table & Column Mappings
            ToTable("Currencies");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.Name).HasColumnName("Title");
            Property(t => t.Symbol).HasColumnName("Symbol");
            Property(t => t.UpdatedOn).HasColumnName("UpdatedOn");
            Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
        }
    }
}