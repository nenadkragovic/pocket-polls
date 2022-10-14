using Microsoft.EntityFrameworkCore;
using Polls.Api.Database.Models;

namespace Polls.Api.Database
{
    public class Context : DbContext
    {
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
        public virtual DbSet<SingleChoice> SingleChoices { get; set; }
        public virtual DbSet<MultipleChoice> MultipleChoices { get; set; }

        public Context(DbContextOptions<Context> options): base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=(localdb)\\Local;Database=PollsDb;User id=sa;Password=Fazi.12358;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Question_YesNoAnswers");
            });

            modelBuilder.Entity<SingleChoiceAnswer>(entity =>
            {

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.SingleChoiceAnswers)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Question_SingleChoiceAnswers");
            });

            modelBuilder.Entity<MultipleChoiceAnswer>(entity =>
            {
                entity.HasOne(d => d.Question)
                    .WithMany(p => p.MultipleChoiceAnswer)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Question_MultipleChoiceAnswer");
            });

            modelBuilder.Entity<TextAnswer>(entity =>
            {
                entity.HasOne(d => d.Question)
                    .WithMany(p => p.TextAnswers)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Question_TextAnswers");
            });

            modelBuilder.Entity<SingleChoice>(entity =>
            {
                entity.HasOne(d => d.SingleChoiceQuestion)
                    .WithMany(p => p.Choices)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_SingleChoiceQuestion_Choices");
            });

            modelBuilder.Entity<MultipleChoice>(entity =>
            {
                entity.HasOne(d => d.MultipleChoiceQuestion)
                    .WithMany(p => p.Choices)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_MultipleChoiceQuestion_Choices");
            });

            modelBuilder.Entity<MultipleChoiceQuestionsAnswer>(entity =>
            {
                entity.HasIndex(e => new { e.AnswerId, e.MultipleChoiceId });

                entity.HasOne(d => d.Anwser)
                    .WithMany(p => p.MultipleChoiceQuestionsAnswers)
                    .HasForeignKey(d => d.AnswerId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_MultipleChoiceQuestionsAnswer_MultipleChoice");

                entity.HasOne(d => d.MultipleChoice)
                    .WithMany(p => p.MultipleChoiceQuestionsAnswers)
                    .HasForeignKey(d => d.MultipleChoiceId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_MultipleChoice_MultipleChoiceQuestionsAnswer");
            });
        }
    }
}
