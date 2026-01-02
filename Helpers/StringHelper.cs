namespace VaninChat2.Helpers
{
    public static class StringHelper
    {
        public static string SortCharacters(params string[] values)
        {
            var characters = string.Concat(values).ToArray();
            Array.Sort(characters);
            return new string(characters);
        }
    }
}