namespace customApiApp_3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Category",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(unicode: false),
                        Description = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Location",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Place = c.String(unicode: false),
                        Description = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Shit",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(unicode: false),
                        Description = c.String(unicode: false),
                        UpdateTime = c.DateTime(nullable: false, precision: 0),
                        Category_Id = c.Int(),
                        Location_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Category", t => t.Category_Id)
                .ForeignKey("dbo.Location", t => t.Location_Id)
                .Index(t => t.Category_Id)
                .Index(t => t.Location_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Shit", "Location_Id", "dbo.Location");
            DropForeignKey("dbo.Shit", "Category_Id", "dbo.Category");
            DropIndex("dbo.Shit", new[] { "Location_Id" });
            DropIndex("dbo.Shit", new[] { "Category_Id" });
            DropTable("dbo.Shit");
            DropTable("dbo.Location");
            DropTable("dbo.Category");
        }
    }
}
