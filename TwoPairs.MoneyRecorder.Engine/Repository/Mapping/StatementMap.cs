using System.Data.Entity.ModelConfiguration;

namespace TwoPairs.MoneyRecorder.Data
{
    public class StatementMap : EntityTypeConfiguration<Statement>
    {
        public StatementMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Name)
                .HasMaxLength(512);

            // Table & Column Mappings
            ToTable("Statements");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.Name).HasColumnName("Title");
            Property(t => t.CurrencyId).HasColumnName("CurrencyId");
            Property(t => t.UpdatedOn).HasColumnName("UpdatedOn");
            Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");

            HasRequired(t => t.Currency).WithMany(t => t.Statements).HasForeignKey(s => s.CurrencyId);
        }
    }
}