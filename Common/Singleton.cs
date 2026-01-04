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

        public static Singleton Instance
        {
            get
            {
                lock (_locker)
                {
                    if (_instance == null)
                        _instance = new Singleton();
                    return _instance;
                }
            }
        }

        public void Add(object value)
        {
            lock (_locker)
            {
                _data.Add(value.GetType().Name, value);
            }
        }

        public T Get<T>()
        {
            lock (_locker)
            {
                var key = typeof(T).Name;
                if (_data.ContainsKey(key))
                    return (T)_data[key];
                else
                    return default;
            }
        }

        public void Remove<T>()
        {
            lock (_locker)
            {
                var key = typeof(T).Name;
                _data.Remove(key);
            }
        }
    }
}