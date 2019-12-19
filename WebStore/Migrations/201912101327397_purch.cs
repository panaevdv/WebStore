namespace WebStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class purch : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Purchases",
                c => new
                    {
                        PurchaseId = c.Int(nullable: false, identity: true),
                        TotalValue = c.Double(nullable: false),
                        PurchaseTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.PurchaseId);
            
            CreateTable(
                "dbo.CartLines",
                c => new
                    {
                        LineId = c.Int(nullable: false, identity: true),
                        Quantity = c.Int(nullable: false),
                        Product_ProductId = c.Int(),
                        Purchase_PurchaseId = c.Int(),
                    })
                .PrimaryKey(t => t.LineId)
                .ForeignKey("dbo.ProductModels", t => t.Product_ProductId)
                .ForeignKey("dbo.Purchases", t => t.Purchase_PurchaseId)
                .Index(t => t.Product_ProductId)
                .Index(t => t.Purchase_PurchaseId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CartLines", "Purchase_PurchaseId", "dbo.Purchases");
            DropForeignKey("dbo.CartLines", "Product_ProductId", "dbo.ProductModels");
            DropIndex("dbo.CartLines", new[] { "Purchase_PurchaseId" });
            DropIndex("dbo.CartLines", new[] { "Product_ProductId" });
            DropTable("dbo.CartLines");
            DropTable("dbo.Purchases");
        }
    }
}
