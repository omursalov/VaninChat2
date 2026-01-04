namespace VaninChat2.Validators
{
    public static class MessageValidator
    {
        public static bool Check(string message, out string error)
        {
            error = null;

            if (string.IsNullOrEmpty(message))
            {
                error = "Заполните сообщение";
                return false;
            }

            if (message.IndexOf('\n') >= 0)
            {
                error = "Уберите переносы строк";
                return false;
            }

            return true;
        }
    }
}
