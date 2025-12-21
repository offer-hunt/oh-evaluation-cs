using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oh.Evaluation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RestrictEvaluation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS evaluation.__migration_probe (
                    id BIGSERIAL PRIMARY KEY,
                    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
                );
                GRANT SELECT, INSERT, UPDATE, DELETE ON TABLE evaluation.__migration_probe TO evaluation_user;
                GRANT USAGE, SELECT ON SEQUENCE evaluation.__migration_probe_id_seq TO evaluation_user;
            ");

            migrationBuilder.Sql(@"
                REVOKE ALL ON SCHEMA public FROM PUBLIC;
                REVOKE CREATE ON SCHEMA public FROM PUBLIC;
            ");

            migrationBuilder.Sql(@"
                REVOKE ALL ON DATABASE evaluation_db FROM PUBLIC;
                GRANT CONNECT, TEMPORARY ON DATABASE evaluation_db TO evaluation_user;
                ALTER ROLE evaluation_user SET search_path = evaluation;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DROP TABLE IF EXISTS evaluation.__migration_probe;
                ALTER ROLE evaluation_user RESET search_path;
                GRANT ALL ON SCHEMA public TO PUBLIC;
            ");
        }
    }
}
