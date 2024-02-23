namespace Giinn;

internal enum LineTrackingMode
{
    Track,
    DoNotTrack
}

internal class ConsoleHandler
{
    public ConsoleHandler(LineTrackingMode lineTracking)
    {
        this.lineTracking = lineTracking;
    }

    private int writtenLines;
    private readonly LineTrackingMode lineTracking;

    private void UpdateLineTracking(string? message)
    {
        UpdateLineTracking(message.Lines());
    }

    private void UpdateLineTracking(int lines)
    {
        if (lineTracking is not LineTrackingMode.Track)
        {
            return;
        }
        writtenLines += lines;
    }

    public void WriteLine(string? message)
    {
        UpdateLineTracking(message);
        Console.WriteLine(message);
    }

    public void Write(string? message)
    {
        UpdateLineTracking(message.Lines() - 1);
        Console.Write(message);
    }

    public void ClearLines()
    {
        ClearLines(writtenLines);
        writtenLines = 0;
    }

    public string? ReadLine()
    {
        UpdateLineTracking(1);
        return Console.ReadLine();
    }

    public static void ClearLines(int lines) => lines.AsIterations(_ => ClearLine());

    public static void ClearLine()
    {
        if (Console.CursorLeft is 0)
        {
            Console.CursorTop -= 1;
        }
        Console.CursorLeft = 0;
        Console.Write(new string(' ', Console.BufferWidth));
        Console.CursorLeft = 0;
        Console.CursorTop -= 1;
    }
}
