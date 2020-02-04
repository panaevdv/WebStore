namespace WebStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class purch51 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Purchases", new[] { "ApplicationUser_Id" });
            DropColumn("dbo.Purchases", "ApplicationUserId");
            RenameColumn(table: "dbo.Purchases", name: "ApplicationUser_Id", newName: "ApplicationUserId");
            AlterColumn("dbo.Purchases", "ApplicationUserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Purchases", "ApplicationUserId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Purchases", new[] { "ApplicationUserId" });
            AlterColumn("dbo.Purchases", "ApplicationUserId", c => c.Int(nullable: false));
            RenameColumn(table: "dbo.Purchases", name: "ApplicationUserId", newName: "ApplicationUser_Id");
            AddColumn("dbo.Purchases", "ApplicationUserId", c => c.Int(nullable: false));
            CreateIndex("dbo.Purchases", "ApplicationUser_Id");
        }
    }
}
