namespace VaninChat2.Dto
{
    public class FileObj : IDisposable
    {
        private readonly string _name;

        public string Name => _name;
        public byte[] Bytes => File.ReadAllBytes(_name);

        public FileObj(string name)
        {
            _name = name;
        }

        public void Dispose()
        {
            File.Delete(_name);
        }
    }
}