using System.Text.RegularExpressions;

namespace VaninChat2.Validators
{
    public static class PassValidator
    {
        public static bool Check(string pass, out string error)
        {
            error = null;

            if (string.IsNullOrEmpty(pass))
            {
                error = "Заполните поле";
                return false;
            }

            if (pass.Length < 8)
            {
                error = "Минимум 8 символов";
                return false;
            }

            if (pass.Any(Char.IsWhiteSpace))
            {
                error = "Уберите пробелы";
                return false;
            }

            if (Regex.IsMatch(pass, @"\p{IsCyrillic}"))
            {
                error = "Уберите русс. буквы";
                return false;
            }

            return true;
        }
    }
}
