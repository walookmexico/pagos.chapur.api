namespace PagosGranChapur.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class PaymentsDBv18 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.transacciones", "estatusTransaccion", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.transacciones", "estatusTransaccion");
        }
    }
}
