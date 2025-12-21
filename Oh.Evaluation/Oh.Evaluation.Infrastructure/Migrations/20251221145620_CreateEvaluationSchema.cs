using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oh.Evaluation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateEvaluationSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "evaluation");

            migrationBuilder.CreateTable(
                name: "runner_submissions",
                schema: "evaluation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    course_id = table.Column<Guid>(type: "uuid", nullable: true),
                    lesson_id = table.Column<Guid>(type: "uuid", nullable: true),
                    page_id = table.Column<Guid>(type: "uuid", nullable: false),
                    question_id = table.Column<Guid>(type: "uuid", nullable: false),
                    submission_type = table.Column<string>(type: "text", nullable: false),
                    language = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    runtime_image = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    time_limit_ms = table.Column<int>(type: "integer", nullable: true),
                    memory_limit_mb = table.Column<int>(type: "integer", nullable: true),
                    code = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "text", nullable: false),
                    verdict = table.Column<string>(type: "text", nullable: false),
                    tests_passed = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    tests_total = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    time_ms = table.Column<int>(type: "integer", nullable: true),
                    memory_kb = table.Column<int>(type: "integer", nullable: true),
                    result = table.Column<string>(type: "jsonb", nullable: true),
                    submitted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    started_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    finished_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ai_review_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_runner_submissions", x => x.id);
                    table.CheckConstraint("ck_runner_submissions_submission_type", "submission_type IN ('TEXT', 'CODE')");
                    table.CheckConstraint("ck_runner_submissions_verdict", "verdict IN ('PENDING', 'ACCEPTED', 'REJECTED')");
                });

            migrationBuilder.CreateTable(
                name: "runner_ai_reviews",
                schema: "evaluation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    submission_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    model = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    score = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    summary = table.Column<string>(type: "text", nullable: true),
                    suggestions = table.Column<string>(type: "jsonb", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RunnerSubmissionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_runner_ai_reviews", x => x.id);
                    table.CheckConstraint("ck_runner_ai_reviews_status", "status IN ('PENDING', 'DONE', 'ERROR')");
                    table.ForeignKey(
                        name: "FK_runner_ai_reviews_runner_submissions_submission_id",
                        column: x => x.submission_id,
                        principalSchema: "evaluation",
                        principalTable: "runner_submissions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "runner_test_case_results",
                schema: "evaluation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    submission_id = table.Column<Guid>(type: "uuid", nullable: false),
                    test_case_id = table.Column<Guid>(type: "uuid", nullable: true),
                    status = table.Column<string>(type: "text", nullable: false),
                    time_ms = table.Column<int>(type: "integer", nullable: true),
                    memory_kb = table.Column<int>(type: "integer", nullable: true),
                    stderr_snippet = table.Column<string>(type: "text", nullable: true),
                    diff_snippet = table.Column<string>(type: "text", nullable: true),
                    RunnerSubmissionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_runner_test_case_results", x => x.id);
                    table.CheckConstraint("ck_runner_test_case_results_status", "status IN ('PASS', 'FAIL', 'TLE', 'MLE', 'RE')");
                    table.ForeignKey(
                        name: "FK_runner_test_case_results_runner_submissions_submission_id",
                        column: x => x.submission_id,
                        principalSchema: "evaluation",
                        principalTable: "runner_submissions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_runner_ai_reviews_submission_id",
                schema: "evaluation",
                table: "runner_ai_reviews",
                column: "submission_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_runner_test_case_results_submission_id",
                schema: "evaluation",
                table: "runner_test_case_results",
                column: "submission_id");

            migrationBuilder.CreateIndex(
                name: "IX_runner_test_case_results_test_case_id",
                schema: "evaluation",
                table: "runner_test_case_results",
                column: "test_case_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "runner_ai_reviews",
                schema: "evaluation");

            migrationBuilder.DropTable(
                name: "runner_test_case_results",
                schema: "evaluation");

            migrationBuilder.DropTable(
                name: "runner_submissions",
                schema: "evaluation");
        }
    }
}
