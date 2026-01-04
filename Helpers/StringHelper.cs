namespace VaninChat2.Helpers
{
    public static class StringHelper
    {
        public static string SortCharacters(params string[] values)
        {
            var characters = string.Concat(values).ToArray();
            var arr = characters.OrderByDescending(c => c).ToArray();
            return new string(arr);
        }
    }
}