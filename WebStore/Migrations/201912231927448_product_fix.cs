namespace WebStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class product_fix : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CartLines", "Product_ProductId", "dbo.ProductModels");
            DropIndex("dbo.CartLines", new[] { "Product_ProductId" });
            RenameColumn(table: "dbo.CartLines", name: "Product_ProductId", newName: "ProductId");
            AlterColumn("dbo.CartLines", "ProductId", c => c.Int(nullable: false));
            CreateIndex("dbo.CartLines", "ProductId");
            AddForeignKey("dbo.CartLines", "ProductId", "dbo.ProductModels", "ProductId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CartLines", "ProductId", "dbo.ProductModels");
            DropIndex("dbo.CartLines", new[] { "ProductId" });
            AlterColumn("dbo.CartLines", "ProductId", c => c.Int());
            RenameColumn(table: "dbo.CartLines", name: "ProductId", newName: "Product_ProductId");
            CreateIndex("dbo.CartLines", "Product_ProductId");
            AddForeignKey("dbo.CartLines", "Product_ProductId", "dbo.ProductModels", "ProductId");
        }
    }
}
