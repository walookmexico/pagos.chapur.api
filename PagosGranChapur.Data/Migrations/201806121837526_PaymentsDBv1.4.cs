namespace PagosGranChapur.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PaymentsDBv14 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.transacciones", "operacionCajaId", c => c.Int());
            AddColumn("dbo.transacciones", "fechaValidacion", c => c.DateTime(precision: 0));
        }
        
        public override void Down()
        {
            DropColumn("dbo.transacciones", "fechaValidacion");
            DropColumn("dbo.transacciones", "operacionCajaId");
        }
    }
}
