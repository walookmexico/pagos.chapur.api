namespace PagosGranChapur.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class PaymentsDBv17 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.transacciones", "detallePiorpi", c => c.String(unicode: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.transacciones", "detallePiorpi", c => c.Int());
        }
    }
}
