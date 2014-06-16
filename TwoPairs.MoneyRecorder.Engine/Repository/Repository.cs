using System.Data.Entity;
using TwoPairs.MoneyRecorder.Data;

namespace TwoPairs.MoneyRecorder.Engine.Data
{
    public class Repository : DbContext
    {
        public Repository()
            : base("name=Default")
        {
            Database.SetInitializer<Repository>(null);
        }

        public virtual IDbSet<Currency> Currencies { get; set; }

        public virtual IDbSet<Statement> Statements { get; set; }

        public virtual IDbSet<User> Users { get; set; }

        public virtual void SetEntityState<T>(T entity, EntityState state)
            where T : class
        {
            Entry(entity).State = state;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Configurations.Add(new CurrencyMap());
            modelBuilder.Configurations.Add(new StatementMap());
            modelBuilder.Configurations.Add(new UserMap());
        }
    }
}