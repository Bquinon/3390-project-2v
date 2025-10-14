namespace ConnectMore.Scripts;

public static class Extensions
{
    public static bool IsBetween(this int i, int start, int end)
    {
        return i >= start && i < end;
    }
}