﻿using Microsoft.AspNetCore.Identity;
using Polls.Lib.Enums;
using System.ComponentModel.DataAnnotations;

namespace Polls.Lib.Database.Models
{
    public abstract class Answer
    {
        [Key]
        public long Id { get; set; }
        /// <summary>
        /// Id of user that answerd questions
        /// </summary>
        public Guid UserId { get; set; }
        public long QuestionId { get; set; }
        public QuestionType QuestionType { get; set; }
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;

        public virtual User User { get; set; }
    }

    public class YesNoAnswer : Answer
    {
        public YesNoQuestion Question { get; set; }
        public bool Answer { get; set; }
    }

    public class SingleChoiceAnswer : Answer
    {
        public long SingleChoiceId { get; set; }

        public SingleChoiceQuestion Question { get; set; }
        public SingleChoiceOption SingleChoice { get; set; }
    }

    public class MultipleChoiceAnswer : Answer
    {
        public MultipleChoiceQuestion Question { get; set; }
        public ICollection<MultipleChoiceQuestionsAnswer> MultipleChoiceQuestionsAnswers { get; set; }
    }

    public class TextAnswer : Answer
    {
        public TextQuestion Question { get; set; }
        public string Answer { get; set; }
    }
}
