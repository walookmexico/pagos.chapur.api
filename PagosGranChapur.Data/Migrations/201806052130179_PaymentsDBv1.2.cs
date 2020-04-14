namespace PagosGranChapur.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PaymentsDBv12 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.tiendas", "tiendaId", c => c.Int(nullable: false));
            AlterColumn("dbo.plataformas", "plataformaId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.plataformas", "plataformaId", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.tiendas", "tiendaId", c => c.Int(nullable: false, identity: true));
        }
    }
}
