namespace VaninChat2.Helpers
{
    public class AttemptHelper
    {
        private readonly int _number;
        private readonly int _delaySec;

        public AttemptHelper(int number, int delaySec)
        {
            _number = number;
            _delaySec = delaySec;
        }

        public async Task<T> ExecuteAsync<T>(Func<Task<T>> funcAsync)
        {
            T result = default;

            for (var i = 0; i < _number; i++)
            {
                try
                {
                    result = await funcAsync();
                    break;
                }
                catch (Exception ex)
                {
                    await Task.Delay(_delaySec * 1000);
                }
            }

            return result;
        }
    }
}