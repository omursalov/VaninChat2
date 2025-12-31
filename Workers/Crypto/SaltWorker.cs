namespace VaninChat2.Workers.Crypto
{
    public class SaltWorker
    {
        private readonly int _newEveryMinutes;

        public SaltWorker(int newEveryMinutes = 10)
        {
            _newEveryMinutes = newEveryMinutes;
        }

        public string Generate()
        {
            var result = RoundDown(DateTime.UtcNow, TimeSpan.FromMinutes(_newEveryMinutes))
                .ToString("yyyyMMddHHmmss");
            var chars = result.OrderBy(c => c).ToArray();
            return string.Join(string.Empty, chars);
        }

        private DateTime RoundDown(DateTime dt, TimeSpan d)
        {
            var delta = dt.Ticks % d.Ticks;
            return new DateTime(dt.Ticks - delta, dt.Kind);
        }
    }
}