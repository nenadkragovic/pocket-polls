using Microsoft.AspNetCore.Identity;

namespace Polls.Lib.Database.Models
{
    public class User : IdentityUser<string>
    {
        public virtual ICollection<Poll> Polls { get; set; }
        public virtual ICollection<YesNoAnswer> YesNoAnswers { get; set; }
        public virtual ICollection<SingleChoiceAnswer> SingleChoiceAnswers { get; set; }
        public virtual ICollection<MultipleChoiceAnswer> MultipleChoiceAnswers { get; set; }
        public virtual ICollection<TextAnswer> TextAnswers { get; set; }
    }

    public class Role : IdentityRole<string>
    {

    }
}
