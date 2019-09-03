public static class StringUtility
{
    public static bool IsPasswordEmpty(this string value)
    {
        var size = value.Length;
        return (size == 0) || (size == 1 && value[0] == '\u200B');
    }
}
