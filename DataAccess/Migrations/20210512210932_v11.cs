using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class v11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Domains",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    GuId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Domains", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    GuId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Topics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GuId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Topics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Words",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Vocable = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Mean = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DomainId = table.Column<int>(type: "int", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GuId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Words", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Words_Domains_DomainId",
                        column: x => x.DomainId,
                        principalTable: "Domains",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EMail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    NumbersofWordLearned = table.Column<int>(type: "int", nullable: false),
                    GuId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "QuestionTests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatorId = table.Column<int>(type: "int", nullable: false),
                    TopicId = table.Column<int>(type: "int", nullable: false),
                    Question = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    CorrectAnswer = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    WrongAnswer1 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    WrongAnswer2 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    WrongAnswer3 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    WrongAnswer4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GuId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionTests_Topics_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topics",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SpaceTests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatorId = table.Column<int>(type: "int", nullable: false),
                    QuestionPart1 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AnswerPart1 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TopicId = table.Column<int>(type: "int", nullable: false),
                    QuestionPart2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AnswerPart2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GuId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpaceTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpaceTests_Topics_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topics",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserWords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WordListName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LearningDegree = table.Column<int>(type: "int", nullable: false),
                    WordId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    GuId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserWords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserWords_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserWords_Words_WordId",
                        column: x => x.WordId,
                        principalTable: "Words",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuestionTests_TopicId",
                table: "QuestionTests",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_SpaceTests_TopicId",
                table: "SpaceTests",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_EMail",
                table: "Users",
                column: "EMail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserWords_UserId",
                table: "UserWords",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserWords_WordId",
                table: "UserWords",
                column: "WordId");

            migrationBuilder.CreateIndex(
                name: "IX_Words_DomainId",
                table: "Words",
                column: "DomainId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuestionTests");

            migrationBuilder.DropTable(
                name: "SpaceTests");

            migrationBuilder.DropTable(
                name: "UserWords");

            migrationBuilder.DropTable(
                name: "Topics");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Words");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Domains");
        }
    }
}
