using System.Text.RegularExpressions;

namespace VaninChat2.Validators
{
    public static class NameValidator
    {
        public static bool Check(string name, out string error)
        {
            error = null;

            if (string.IsNullOrEmpty(name))
            {
                error = "Заполните поле";
                return false;
            }

            if (name.Length < 4)
            {
                error = "Минимум 4 символа";
                return false;
            }

            if (name.Any(Char.IsWhiteSpace))
            {
                error = "Уберите пробелы";
                return false;
            }

            if (!Regex.IsMatch(name, @"^[a-zA-Z0-9]+$"))
            {
                error = "Только англ. буквы и цифры";
                return false;
            }

            return true;
        }
    }
}