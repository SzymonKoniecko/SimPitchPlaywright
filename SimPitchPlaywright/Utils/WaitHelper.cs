namespace SimPitchPlaywright.Utils;

public static class WaitHelper
{
    internal static Task Sleep(int milliseconds = 1500) => Task.Delay(milliseconds);
}
