namespace VaninChat2.Workers
{
    public class PassWorker
    {
        private const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!#@$%^&*()-+<>?";

        public string Generate(int length = 16)
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