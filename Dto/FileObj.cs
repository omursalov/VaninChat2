namespace VaninChat2.Dto
{
    public class FileObj : IDisposable
    {
        private readonly string _name;
        private readonly object _lockObj = new object();

        public string Name => _name;
        public byte[] Bytes => File.ReadAllBytes(_name);

        public FileInfo FileInfo => new FileInfo(_name);

        public FileObj(string name)
        {
            _name = name;
        }

        ~FileObj()
        {
            Dispose();
        }

        public void Dispose()
        {
            lock (_lockObj)
            {
                if (File.Exists(_name))
                {
                    File.Delete(_name);
                }
            }
        }
    }
}