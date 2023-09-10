namespace BookingCalendarApi.Repository.NETFramework.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ColorAssignments",
                c => new
                    {
                        BookingId = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        Color = c.String(nullable: false, unicode: false),
                    })
                .PrimaryKey(t => t.BookingId);
            
            CreateTable(
                "dbo.Floors",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false, unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Rooms",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        FloorId = c.Long(nullable: false),
                        Number = c.String(nullable: false, unicode: false),
                        Type = c.String(nullable: false, unicode: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Floors", t => t.FloorId, cascadeDelete: true)
                .Index(t => t.FloorId);
            
            CreateTable(
                "dbo.Nations",
                c => new
                    {
                        Iso = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        Code = c.Long(nullable: false),
                        Description = c.String(nullable: false, unicode: false),
                    })
                .PrimaryKey(t => t.Iso);
            
            CreateTable(
                "dbo.RoomAssignments",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        RoomId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Rooms", t => t.RoomId, cascadeDelete: true)
                .Index(t => t.RoomId);
            
            CreateTable(
                "dbo.Structures",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserRefreshTokens",
                c => new
                    {
                        RefreshToken = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        Username = c.String(nullable: false, unicode: false),
                        ExpiresAt = c.DateTime(nullable: false, precision: 0),
                    })
                .PrimaryKey(t => t.RefreshToken);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        StructureId = c.Long(nullable: false),
                        Username = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
                        PasswordHash = c.String(nullable: false, unicode: false),
                        VisibleName = c.String(maxLength: 100, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Structures", t => t.StructureId, cascadeDelete: true)
                .Index(t => t.StructureId)
                .Index(t => t.Username, unique: true);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Users", "StructureId", "dbo.Structures");
            DropForeignKey("dbo.RoomAssignments", "RoomId", "dbo.Rooms");
            DropForeignKey("dbo.Rooms", "FloorId", "dbo.Floors");
            DropIndex("dbo.Users", new[] { "Username" });
            DropIndex("dbo.Users", new[] { "StructureId" });
            DropIndex("dbo.RoomAssignments", new[] { "RoomId" });
            DropIndex("dbo.Rooms", new[] { "FloorId" });
            DropTable("dbo.Users");
            DropTable("dbo.UserRefreshTokens");
            DropTable("dbo.Structures");
            DropTable("dbo.RoomAssignments");
            DropTable("dbo.Nations");
            DropTable("dbo.Rooms");
            DropTable("dbo.Floors");
            DropTable("dbo.ColorAssignments");
        }
    }
}
