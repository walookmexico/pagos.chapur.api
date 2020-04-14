namespace PagosGranChapur.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PaymentsDBv19 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.transacciones", "meses", c => c.Int(nullable: false));
            AddColumn("dbo.transacciones", "conIntereses", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.transacciones", "conIntereses");
            DropColumn("dbo.transacciones", "meses");
        }
    }
}
