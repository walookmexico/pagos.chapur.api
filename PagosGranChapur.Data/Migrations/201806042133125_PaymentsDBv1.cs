namespace PagosGranChapur.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class PaymentsDBv1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.logtransacciones",
                c => new
                    {
                        logTransaccionId = c.Int(nullable: false, identity: true),
                        transaccionId = c.Int(),
                        ordenCompra = c.Int(nullable: false),
                        fechaAplicacion = c.DateTime(nullable: false, precision: 0),
                        estatus = c.Int(nullable: false),
                        plataformaId = c.Int(nullable: false),
                        mensaje = c.String(nullable: false, unicode: false),
                    })
                .PrimaryKey(t => t.logTransaccionId)
                .ForeignKey("dbo.transacciones", t => t.transaccionId)
                .Index(t => t.transaccionId);
            
            CreateTable(
                "dbo.transacciones",
                c => new
                    {
                        transaccionId = c.Int(nullable: false, identity: true),
                        fechaHoraAplicacion = c.DateTime(precision: 0),
                        total = c.Decimal(precision: 18, scale: 2),
                        cuentahabienteNombre = c.String(maxLength: 500, unicode: false, storeType: "nvarchar"),
                        correoElectronicoUsuario = c.String(maxLength: 500, unicode: false, storeType: "nvarchar"),
                        autorizacionId = c.String(unicode: false),
                        tiendaId = c.Int(),
                        tiendaDescripcion = c.String(maxLength: 500, unicode: false, storeType: "nvarchar"),
                        plataformaId = c.Int(),
                        descripcionPlataforma = c.String(maxLength: 250, unicode: false, storeType: "nvarchar"),
                        usuarioId = c.Int(),
                        ordenCompraId = c.Int(),
                    })
                .PrimaryKey(t => t.transaccionId);
            
            CreateTable(
                "dbo.usuarios",
                c => new
                    {
                        usuarioId = c.Int(nullable: false, identity: true),
                        FullName = c.String(nullable: false, maxLength: 500, unicode: false, storeType: "nvarchar"),
                        nombreUsuario = c.String(nullable: false, maxLength: 50, unicode: false, storeType: "nvarchar"),
                        contrasenia = c.String(nullable: false, unicode: false),
                        correoElectronico = c.String(nullable: false, maxLength: 300, unicode: false, storeType: "nvarchar"),
                        rolId = c.Int(nullable: false),
                        contraseniaTemp = c.String(unicode: false),
                        cambiarContrasenia = c.Boolean(nullable: false, storeType: "bit"),
                        fechaCreacion = c.DateTime(nullable: false, precision: 0),
                        fechaActualizacion = c.DateTime(nullable: false, precision: 0),
                    })
                .PrimaryKey(t => t.usuarioId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.logtransacciones", "transaccionId", "dbo.transacciones");
            DropIndex("dbo.logtransacciones", new[] { "transaccionId" });
            DropTable("dbo.usuarios");
            DropTable("dbo.transacciones");
            DropTable("dbo.logtransacciones");
        }
    }
}
