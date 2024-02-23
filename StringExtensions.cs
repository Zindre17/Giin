namespace Giinn;

internal static class StringExtensions
{
    public static int Lines(this string? str) => (str?.Count(c => c is '\n') ?? 0) + 1;
}
