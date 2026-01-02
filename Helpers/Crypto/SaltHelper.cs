namespace VaninChat2.Helpers.Crypto
{
    public class SaltHelper
    {
        private readonly int _newEveryMinutes;

        public SaltHelper(int newEveryMinutes = 10)
        {
            _newEveryMinutes = newEveryMinutes;
        }

        public string Generate()
        {
            var result = RoundDown(DateTime.UtcNow, TimeSpan.FromMinutes(_newEveryMinutes))
                .ToString("yyyyMMddHHmmss");
            return StringHelper.SortCharacters(result);
        }

        private DateTime RoundDown(DateTime dt, TimeSpan d)
        {
            var delta = dt.Ticks % d.Ticks;
            return new DateTime(dt.Ticks - delta, dt.Kind);
        }
    }
}