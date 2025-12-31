namespace VaninChat2.Common
{
    public class Singleton
    {
        private static Singleton _instance;
        private static object _locker = new object();
        private readonly Dictionary<string, object> _data;

        private Singleton()
        {
            _data = new Dictionary<string, object>();
        }

        public static Singleton Get()
        {
            lock (_locker)
            {
                if (_instance == null)
                    _instance = new Singleton();
                return _instance;
            }
        }

        public void Add(object value)
        {
            lock (_locker)
            {
                _data.Add(value.GetType().Name, value);
            }
        }

        public T Get<T>() where T : class
        {
            lock (_locker)
            {
                var key = typeof(T).Name;
                if (_data.ContainsKey(key))
                    return (T)_data[key];
                else
                    return null;
            }
        }

        public void DisposeAndClear()
        {
            foreach (var item in _data.Values)
            {
                if (item is IDisposable)
                {
                    ((IDisposable)item).Dispose();
                }
            }
            _data.Clear();
        }
    }
}