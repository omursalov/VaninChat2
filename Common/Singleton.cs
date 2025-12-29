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

        public void Set(string key, object value)
        {
            lock (_locker)
            {
                _data.Add(key, value);
            }
        }

        public T TryGet<T>(string key) where T : class
        {
            lock (_locker)
            {
                if (_data.ContainsKey(key))
                    return (T)_data[key];
                else
                    return null;
            }
        }
    }
}