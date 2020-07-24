namespace PagosGranChapur.Data.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<PagosGranChapur.Data.PagosGranChapurContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(PagosGranChapur.Data.PagosGranChapurContext context)
        {
            
        }
    }
}
