namespace PagosGranChapur.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PaymentsDBv15 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.transacciones", "estatusPiorpi", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.transacciones", "estatusPiorpi");
        }
    }
}
