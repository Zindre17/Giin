namespace Giinn;

internal enum LineTrackingMode
{
    Track,
    DoNotTrack
}

internal record Checkpoint(int LinesWrittenOffset, int LinePosition);

internal class ConsoleHandler
{
    public ConsoleHandler(LineTrackingMode lineTracking)
    {
        this.lineTracking = lineTracking;
    }

    private int writtenLines;
    private readonly LineTrackingMode lineTracking;

    private readonly Dictionary<string, Checkpoint> checkpoints = new();

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

    public void AddCheckpoint(string label)
    {
        checkpoints.Add(label, new(writtenLines, Console.CursorLeft));
    }

    public void WriteLineFromCheckpoint(string label, string? message, bool clearLinesBetween = false)
    {
        if (!checkpoints.TryGetValue(label, out var checkpoint))
        {
            return;
        }

        if (clearLinesBetween)
        {
            ClearLines(writtenLines - checkpoint.LinesWrittenOffset);
        }
        else
        {
            Console.CursorTop -= writtenLines - checkpoint.LinesWrittenOffset;
        }
        Console.CursorLeft = checkpoint.LinePosition;
        Console.WriteLine(message);
    }

    public void ClearToCheckpoint(string label)
    {
        if (!checkpoints.TryGetValue(label, out var checkpoint))
        {
            return;
        }

        ClearLines(writtenLines - checkpoint.LinesWrittenOffset);
    }

    public void WriteLine(string? message = null)
    {
        UpdateLineTracking(message);
        Console.WriteLine(message);
    }

    public void Write(string? message)
    {
        UpdateLineTracking(message.Lines() - 1);
        Console.Write(message);
    }

    public void ClearLines() => ClearLines(writtenLines);

    public string? ReadLine()
    {
        UpdateLineTracking(1);
        return Console.ReadLine();
    }

    public ConsoleKeyInfo ReadKey()
    {
        return Console.ReadKey();
    }

    private void ClearLines(int lines) => lines.AsIterations(_ => ClearLine());

    private void ClearLine()
    {
        UpdateLineTracking(-1);
        if (Console.CursorLeft is 0)
        {
            Console.CursorTop -= 1;
        }
        Console.CursorLeft = 0;
        Console.Write(new string(' ', Console.BufferWidth));
        Console.CursorLeft = 0;
        Console.CursorTop -= 1;
    }

    public void DisableCursor()
    {
        Console.CancelKeyPress += EnableCursor;
        Console.CursorVisible = false;
    }

    public void EnableCursor() => EnableCursor(null, null);

    private void EnableCursor(object? _, EventArgs? __)
    {
        Console.CursorVisible = true;
        Console.CancelKeyPress -= EnableCursor;
    }
}
