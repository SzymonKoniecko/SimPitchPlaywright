namespace SimPitchPlaywright.Utils;

public static class TextHelper
{
    public static string ReplaceNumberToAssert(string value)
    {
        return value.Replace(",", "").Replace(".", "");
    }
}
