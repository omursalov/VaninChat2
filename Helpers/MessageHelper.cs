using VaninChat2.Helpers.Crypto;

namespace VaninChat2.Helpers
{
    public static class MessageHelper
    {
        public static string DefinePassword(string pass)
            => $"[[[PASS:{pass}]]]";

        public static string ExtractPassword(string value)
            => value.Replace("[[[PASS:", string.Empty)
            .Replace("]]]", string.Empty);

        public static string DefineMessage(string text)
            => $"[[[MESSAGE:{text}]]]";

        public static string ExtractMessage(string value)
            => value.Replace("[[[MESSAGE:", string.Empty)
            .Replace("]]]", string.Empty);
    }
}