namespace PagosGranChapur.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PaymentsDBv16 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.transacciones", "detallePiorpi", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.transacciones", "detallePiorpi");
        }
    }
}
