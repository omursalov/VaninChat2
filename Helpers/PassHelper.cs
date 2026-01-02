namespace VaninChat2.Helpers
{
    public static class PassHelper
    {
        private const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!#@$%^&*()-+<>?=";

        public static string Generate(int length = 16)
        {
            var rand = new Random();
            var password = string.Empty;

            for (var i = 0; i < length; i++)
            {
                var index = rand.Next(characters.Length);
                password += characters[index];
            }

            return password;
        }
    }
}