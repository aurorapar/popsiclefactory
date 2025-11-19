namespace API.Common
{
    public static class CommonMethods
    {
        public static string RemoveWhitespace(this string input)
        {
            return new string(input
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }
    }
}
