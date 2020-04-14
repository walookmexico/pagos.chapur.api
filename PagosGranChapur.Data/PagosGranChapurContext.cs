using PagosGranChapur.Data.Configuration;
using PagosGranChapur.Entities;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace PagosGranChapur.Data
{
    public class PagosGranChapurContext: DbContext
    {
        public PagosGranChapurContext() : base("PaymentsDB")
        {
            Database.SetInitializer<PagosGranChapurContext>(null);
            Configuration.LazyLoadingEnabled   = false;
            Configuration.ProxyCreationEnabled = false;
        }

        #region EntitySets
        public IDbSet<Transaction> Transaction        { get; set; }
        public IDbSet<User> User                      { get; set; }
        public IDbSet<LogTransaction> LogTransaction  { get; set; }
        public IDbSet<Store> Store                    { get; set; }
        public IDbSet<Platform> Platform              { get; set; }
        #endregion

        public virtual void Commit()
        {
            base.SaveChanges();
        }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Configurations.Add(new TransactionConfig());
            modelBuilder.Configurations.Add(new UserConfig());
            modelBuilder.Configurations.Add(new LogTransactionConfig());
            modelBuilder.Configurations.Add(new StoreConfig());
            modelBuilder.Configurations.Add(new PlatformConfig());
        }
    }
}
