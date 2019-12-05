namespace WebStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class instore : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductModels", "CountInStore", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductModels", "CountInStore");
        }
    }
}
