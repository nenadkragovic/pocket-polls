using System.Reflection;

namespace Polls.Api.Enums
{
    public abstract class QuestionEnumeration : IComparable
    {
        public QuestionType Type { get; private set; }

        protected QuestionEnumeration(QuestionType type) => (type) = (Type);

        public override string ToString() => Type.ToString();

        public static IEnumerable<T> GetAll<T>() where T : QuestionEnumeration =>
            typeof(T).GetFields(BindingFlags.Public |
                                BindingFlags.Static |
                                BindingFlags.DeclaredOnly)
                     .Select(f => f.GetValue(null))
                     .Cast<T>();

        public override bool Equals(object obj)
        {
            if (obj is not QuestionEnumeration otherValue)
            {
                return false;
            }

            var typeMatches = GetType().Equals(obj.GetType());
            var valueMatches = Type.Equals(otherValue.Type);

            return typeMatches && valueMatches;
        }

        public int CompareTo(object other) => Type.CompareTo(((QuestionEnumeration)other).Type);
    }
}
