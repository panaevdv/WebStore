namespace WebStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MyMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductPhotoes",
                c => new
                    {
                        ProductId = c.Int(nullable: false),
                        MimeType = c.String(),
                        Photo = c.Binary(),
                    })
                .PrimaryKey(t => t.ProductId)
                .ForeignKey("dbo.ProductModels", t => t.ProductId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.ProductModels",
                c => new
                    {
                        ProductId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        Category = c.String(nullable: false),
                        Subcategory = c.String(nullable: false),
                        Price = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.ProductId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductPhotoes", "ProductId", "dbo.ProductModels");
            DropIndex("dbo.ProductPhotoes", new[] { "ProductId" });
            DropTable("dbo.ProductModels");
            DropTable("dbo.ProductPhotoes");
        }
    }
}
