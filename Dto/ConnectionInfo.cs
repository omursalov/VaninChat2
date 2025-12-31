namespace VaninChat2.Dto
{
    public class ConnectionInfo
    {
        public string CommonPassword { get; }

        public ConnectionInfo(params string[] values)
        {
            var characters = string.Concat(values).ToArray();
            Array.Sort(characters);
            CommonPassword = new string(characters);
        }
    }
}