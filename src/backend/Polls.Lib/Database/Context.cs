using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Polls.Lib.Database.Models;

namespace Polls.Lib.Database
{
    public class Context : IdentityDbContext<User, Role, string>
    {
        private const string ADMIN_USERNAME = "Admin";
        private const string ADMIN_PASSWORD = "Admin";
        private const string ADMIN_EMAIL = "test@admin.com";
        private const string ADMIN_ID = "1efc5e3a-283b-4b05-b1ea-d2cd424c59d4";
        public const string ADMIN_ROLE_ID = "0c68c99b-e95e-4247-8b71-4925c444268f";
        public const string USER_ROLE_ID = "0c68c99b-e95e-4247-8b71-4925c444268E";

        public virtual DbSet<Poll> Polls { get; set; }
        public virtual DbSet<YesNoQuestion> YesNoQuestions { get; set; }
        public virtual DbSet<SingleChoiceQuestion> SingleChoiceQuestions { get; set; }
        public virtual DbSet<MultipleChoiceQuestion> MultipleChoiceQuestions { get; set; }
        public virtual DbSet<TextQuestion> TextQuestions { get; set; }
        public virtual DbSet<YesNoAnswer> YesNoAnswers { get; set; }
        public virtual DbSet<SingleChoiceAnswer> SingleChoiceAnswers { get; set; }
        public virtual DbSet<MultipleChoiceAnswer> MultipleChoiceAnswers { get; set; }
        public virtual DbSet<TextAnswer> TextAnswers { get; set; }
        public virtual DbSet<MultipleChoiceQuestionsAnswer> MultipleChoiceQuestionsAnswer { get; set; }
        public virtual DbSet<SingleChoiceOption> SingleChoiceOption { get; set; }
        public virtual DbSet<MultipleChoiceOption> MultipleChoiceOption { get; set; }

        public Context(DbContextOptions<Context> options): base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                throw new Exception("Database server is not configured!");
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Ignore<IdentityUserLogin<string>>();
            modelBuilder.Ignore<IdentityUserRole<string>>();
            modelBuilder.Ignore<IdentityUserClaim<string>>();
            modelBuilder.Ignore<IdentityUserToken<string>>();
            modelBuilder.Ignore<IdentityUser<string>>();

            var hasher = new PasswordHasher<IdentityUser>();

            modelBuilder.Entity<User>().HasData(new User()
            {
                Id = ADMIN_ID,
                UserName = ADMIN_USERNAME,
                NormalizedUserName = ADMIN_USERNAME.ToUpperInvariant(),
                Email = ADMIN_EMAIL,
                NormalizedEmail = ADMIN_EMAIL.ToUpperInvariant(),
                PasswordHash = hasher.HashPassword(null, ADMIN_PASSWORD),
                EmailConfirmed = true,
                SecurityStamp = string.Empty
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasData(new List<Role>()
                {
                    new Role() { Id = ADMIN_ROLE_ID, Name = "Admin", NormalizedName = "ADMIN" },
                    new Role() { Id = USER_ROLE_ID, Name = "User", NormalizedName = "USER" }
                });
            });

            modelBuilder.Entity<Poll>(entity =>
            {
                entity.HasOne(d => d.User)
                    .WithMany(p => p.Polls)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_User_Polls");
            });

            modelBuilder.Entity<YesNoQuestion>(entity =>
            {
                entity.HasOne(d => d.Poll)
                    .WithMany(p => p.YesNoQuestions)
                    .HasForeignKey(d => d.PollId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Poll_YesNoQuestions");
            });

            modelBuilder.Entity<MultipleChoiceQuestion>(entity =>
            {
                entity.HasOne(d => d.Poll)
                    .WithMany(p => p.MultipleChoiceQuestions)
                    .HasForeignKey(d => d.PollId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Poll_MultipleChoiceQuestions");
            });

            modelBuilder.Entity<SingleChoiceQuestion>(entity =>
            {
                entity.HasOne(d => d.Poll)
                    .WithMany(p => p.SingleChoiceQuestions)
                    .HasForeignKey(d => d.PollId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Poll_SingleChoiceQuestions");
            });

            modelBuilder.Entity<TextQuestion>(entity =>
            {
                entity.HasOne(d => d.Poll)
                    .WithMany(p => p.TextQuestions)
                    .HasForeignKey(d => d.PollId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Poll_TextQuestions");
            });

            modelBuilder.Entity<YesNoAnswer>(entity =>
            {
                entity.HasOne(d => d.Question)
                    .WithMany(p => p.YesNoAnswers)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_Question_YesNoAnswers");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.YesNoAnswers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_User_YesNoAnswers");
            });

            modelBuilder.Entity<SingleChoiceAnswer>(entity =>
            {

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.SingleChoiceAnswers)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_Question_SingleChoiceAnswers");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.SingleChoiceAnswers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_User_SingleChoiceAnswers");
            });

            modelBuilder.Entity<MultipleChoiceAnswer>(entity =>
            {
                entity.HasOne(d => d.Question)
                    .WithMany(p => p.MultipleChoiceAnswer)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Question_MultipleChoiceAnswer");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.MultipleChoiceAnswers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_User_MultipleChoiceAnswers");
            });

            modelBuilder.Entity<TextAnswer>(entity =>
            {
                entity.HasOne(d => d.Question)
                    .WithMany(p => p.TextAnswers)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Question_TextAnswers");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TextAnswers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_User_TextAnswers");
            });

            modelBuilder.Entity<SingleChoiceOption>(entity =>
            {
                entity.HasOne(d => d.SingleChoiceQuestion)
                    .WithMany(p => p.Choices)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_SingleChoiceQuestion_Choices");
            });

            modelBuilder.Entity<MultipleChoiceOption>(entity =>
            {
                entity.HasOne(d => d.MultipleChoiceQuestion)
                    .WithMany(p => p.Choices)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_MultipleChoiceQuestion_Choices");
            });

            modelBuilder.Entity<MultipleChoiceQuestionsAnswer>(entity =>
            {
                entity.HasIndex(e => new { e.AnswerId, e.MultipleChoiceOptionId });

                entity.HasOne(d => d.Anwser)
                    .WithMany(p => p.MultipleChoiceQuestionsAnswers)
                    .HasForeignKey(d => d.AnswerId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_MultipleChoiceQuestionsAnswer_MultipleChoiceOption");

                entity.HasOne(d => d.MultipleChoiceOption)
                    .WithMany(p => p.MultipleChoiceQuestionsAnswers)
                    .HasForeignKey(d => d.MultipleChoiceOptionId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_MultipleChoiceOption_MultipleChoiceQuestionsAnswer");
            });
        }
    }
}
