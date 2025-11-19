namespace API.Common
{
    public static class CommonMethods
    {
        public static bool IsEmptyString(string? input)
        {
            return string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input);
        }
    }
}
