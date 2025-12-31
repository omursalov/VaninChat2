namespace VaninChat2.Validators
{
    public class NameValidator
    {
        private readonly char[] _invalidChars = new[]
        {
            ':', '&', ';', '|', '*', '?', '"', '(', ')', '$', '<', '>', '{', '}', '^', '#', '\\', '%', '!', '`', '-'
        };

        public string InvalidChars => string.Join(string.Empty, _invalidChars);

        public bool Check(string name)
        {
            foreach (var c in name)
            {
                if (Char.IsWhiteSpace(c))
                {
                    return false;
                }

                if (_invalidChars.Contains(c))
                {
                    return false;
                }
            }

            return true;
        }
    }
}