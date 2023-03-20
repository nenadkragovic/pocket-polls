using Polls.Lib.Enums;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Polls.Lib.Attributes
{
    public class RequiredIfQuestionTypeIsChoiceAttribute : ValidationAttribute
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var field = validationContext.ObjectType.GetProperty("QuestionType");
            if (field != null)
            {
                var dependentValue = field.GetValue(validationContext.ObjectInstance, null);

                if (Enum.TryParse<QuestionType>(dependentValue.ToString(), true, out var questionType))
                {
                    if (questionType is QuestionType.SingleChoice or QuestionType.MultipleChoice)
                    {
                        if (value == null || value is not ICollection || ((ICollection)value).Count < 2)
                        {
                            return new ValidationResult(FormatErrorMessage("At least 2 choices must be specified."));
                        }
                    }
                }

                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult(FormatErrorMessage("Property QuestionType is required but not found."));
            }
        }
    }
}
