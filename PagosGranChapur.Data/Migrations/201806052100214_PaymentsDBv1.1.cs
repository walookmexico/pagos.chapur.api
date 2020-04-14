namespace PagosGranChapur.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PaymentsDBv11 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tiendas",
                c => new
                    {
                        tiendaId = c.Int(nullable: false, identity: true),
                        descripcion = c.String(nullable: false, unicode: false),
                    })
                .PrimaryKey(t => t.tiendaId);
            
            CreateTable(
                "dbo.plataformas",
                c => new
                    {
                        plataformaId = c.Int(nullable: false, identity: true),
                        descripcion = c.String(nullable: false, unicode: false),
                    })
                .PrimaryKey(t => t.plataformaId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.plataformas");
            DropTable("dbo.tiendas");
        }
    }
}
