namespace customApiApp_3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class second : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Location", "Name", c => c.String(unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Location", "Name");
        }
    }
}
