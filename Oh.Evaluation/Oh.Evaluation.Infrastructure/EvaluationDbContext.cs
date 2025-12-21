using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Oh.Evaluation.Domain.Domain;

namespace Oh.Evaluation.Infrastructure;

public class EvaluationDbContext : DbContext
{
    public EvaluationDbContext(DbContextOptions<EvaluationDbContext> options) : base(options) { }

    public DbSet<RunnerSubmission> RunnerSubmissions { get; set; }
    public DbSet<RunnerAiReview> RunnerAiReviews { get; set; }
    public DbSet<RunnerTestCaseResult> RunnerTestCaseResults { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("evaluation");

        // -------------------- RunnerSubmission --------------------
        modelBuilder.Entity<RunnerSubmission>(entity =>
        {
            entity.ToTable("runner_submissions");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.LessonId).HasColumnName("lesson_id");
            entity.Property(e => e.PageId).HasColumnName("page_id");
            entity.Property(e => e.QuestionId).HasColumnName("question_id");
            entity.Property(e => e.SubmissionType)
                  .HasColumnName("submission_type")
                  .HasConversion<string>();
            entity.Property(e => e.Language).HasColumnName("language").HasMaxLength(32);
            entity.Property(e => e.RuntimeImage).HasColumnName("runtime_image").HasMaxLength(128);
            entity.Property(e => e.TimeLimitMs).HasColumnName("time_limit_ms");
            entity.Property(e => e.MemoryLimitMb).HasColumnName("memory_limit_mb");
            entity.Property(e => e.Code).HasColumnName("code");
            entity.Property(e => e.Status)
                  .HasColumnName("status")
                  .HasConversion<string>();
            entity.Property(e => e.Verdict)
                  .HasColumnName("verdict")
                  .HasConversion<string>();
            entity.Property(e => e.TestsPassed).HasColumnName("tests_passed").HasDefaultValue(0);
            entity.Property(e => e.TestsTotal).HasColumnName("tests_total").HasDefaultValue(0);
            entity.Property(e => e.TimeMs).HasColumnName("time_ms");
            entity.Property(e => e.MemoryKb).HasColumnName("memory_kb");

            // JSONB поле Result
            entity.Property(e => e.Result)
                  .HasColumnName("result")
                  .HasColumnType("jsonb")
                  .HasConversion(
                      v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                      v => JsonSerializer.Deserialize<string?>(v, (JsonSerializerOptions)null)
                  );

            entity.Property<Guid?>("AiReviewId").HasColumnName("ai_review_id"); // поле без FK
            entity.Property(e => e.SubmittedAt)
                  .HasColumnName("submitted_at")
                  .HasDefaultValueSql("now()");
            entity.Property(e => e.StartedAt).HasColumnName("started_at");
            entity.Property(e => e.FinishedAt).HasColumnName("finished_at");

            entity.HasCheckConstraint("ck_runner_submissions_submission_type",
                                      "submission_type IN ('TEXT', 'CODE')");
            entity.HasCheckConstraint("ck_runner_submissions_verdict",
                                      "verdict IN ('PENDING', 'ACCEPTED', 'REJECTED')");

            entity.HasMany(e => e.TestCaseResults)
                  .WithOne(r => r.Submission)
                  .HasForeignKey(r => r.SubmissionId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.AiReview)
                  .WithOne(r => r.Submission)
                  .HasForeignKey<RunnerAiReview>(r => r.SubmissionId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // -------------------- RunnerAiReview --------------------
        modelBuilder.Entity<RunnerAiReview>(entity =>
        {
            entity.ToTable("runner_ai_reviews");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.SubmissionId).HasColumnName("submission_id");
            entity.Property(e => e.Status)
                  .HasColumnName("status")
                  .HasConversion<string>();
            entity.Property(e => e.Model).HasColumnName("model").HasMaxLength(64);
            entity.Property(e => e.Score)
                  .HasColumnName("score")
                  .HasPrecision(5, 2);

            // JSONB поле Suggestions
            entity.Property(e => e.Suggestions)
                  .HasColumnName("suggestions")
                  .HasColumnType("jsonb")
                  .HasConversion(
                      v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                      v => JsonSerializer.Deserialize<string?>(v, (JsonSerializerOptions)null)
                  );

            entity.Property(e => e.Summary).HasColumnName("summary");
            entity.Property(e => e.CreatedAt)
                  .HasColumnName("created_at")
                  .HasDefaultValueSql("now()");
            entity.Property(e => e.CompletedAt).HasColumnName("completed_at");

            entity.HasCheckConstraint("ck_runner_ai_reviews_status",
                                      "status IN ('PENDING', 'DONE', 'ERROR')");

            entity.HasIndex(e => e.SubmissionId).IsUnique();
        });

        // -------------------- RunnerTestCaseResult --------------------
        modelBuilder.Entity<RunnerTestCaseResult>(entity =>
        {
            entity.ToTable("runner_test_case_results");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.SubmissionId).HasColumnName("submission_id");
            entity.Property(e => e.TestCaseId).HasColumnName("test_case_id");
            entity.Property(e => e.Status)
                  .HasColumnName("status")
                  .HasConversion<string>();
            entity.Property(e => e.TimeMs).HasColumnName("time_ms");
            entity.Property(e => e.MemoryKb).HasColumnName("memory_kb");
            entity.Property(e => e.StderrSnippet).HasColumnName("stderr_snippet");
            entity.Property(e => e.DiffSnippet).HasColumnName("diff_snippet");

            entity.HasCheckConstraint("ck_runner_test_case_results_status",
                                      "status IN ('PASS', 'FAIL', 'TLE', 'MLE', 'RE')");

            entity.HasIndex(e => e.SubmissionId);
            entity.HasIndex(e => e.TestCaseId);
        });
    }
}
