namespace Giinn;

internal static class IntExtensions
{
    public static void AsIterations(this int iterations, Action<int> action)
    {
        for (var i = 0; i < iterations; i++)
        {
            action(i);
        }
    }
}
