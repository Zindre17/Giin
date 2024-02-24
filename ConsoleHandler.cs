namespace Giinn;

internal record CursorPosition(int Left, int Top);

internal class ConsoleHandler
{
    private readonly Dictionary<string, CursorPosition> checkpoints = new();

    private int startingTop = Console.CursorTop;

    public void AddCheckpoint(string label)
    {
        checkpoints.Add(label, new(Console.CursorLeft, Console.CursorTop));
    }

    public void WriteLineFromCheckpoint(string label, string? message, bool clearLinesBetween = false)
    {
        if (!checkpoints.TryGetValue(label, out var checkpoint))
        {
            return;
        }

        if (clearLinesBetween)
        {
            ClearLines(Console.CursorTop - 1 - checkpoint.Top);
        }
        SetCursorPosition(checkpoint);
        WriteLine(message);
    }

    public void ClearToCheckpoint(string label)
    {
        if (!checkpoints.TryGetValue(label, out var checkpoint))
        {
            return;
        }

        ClearLines(Console.CursorTop - checkpoint.Top);
    }

    public void WriteLine(string? message = null)
    {
        UpdateCheckpoints(GetBufferScrollAmount(message));
        Console.WriteLine(message);
    }

    private void UpdateCheckpoints(int scrollAmount)
    {
        foreach (var checkpoint in checkpoints)
        {
            checkpoints[checkpoint.Key] = new(checkpoint.Value.Left, Math.Max(checkpoint.Value.Top - scrollAmount, 0));
        }
        startingTop = Math.Max(startingTop - scrollAmount, 0);
    }

    private int GetBufferScrollAmount(string? message)
    {
        var newTop = Console.CursorTop + message.Lines();
        return Math.Max(newTop - (Console.WindowHeight - 1), 0);
    }

    public void Write(string? message)
    {
        UpdateCheckpoints(GetBufferScrollAmount(message) - 1);
        Console.Write(message);
    }

    public void ClearLines() => ClearLines(Console.CursorTop - startingTop);

    public string? ReadLine()
    {
        var result = Console.ReadLine();
        if (Console.CursorTop >= (Console.WindowHeight - 1))
        {
            UpdateCheckpoints(1);
        }
        return result;
    }

    public ConsoleKeyInfo ReadKey()
    {
        return Console.ReadKey();
    }

    private void ClearLines(int lines) => lines.AsIterations(_ => ClearLine());

    private void ClearLine()
    {
        if (Console.CursorLeft is 0 && Console.CursorTop is not 0)
        {
            Console.CursorTop -= 1;
        }
        Console.CursorLeft = 0;
        Console.Write(new string(' ', Console.BufferWidth));
        Console.CursorLeft = 0;
        Console.CursorTop -= 1;
    }

    private static void SetCursorPosition(CursorPosition position)
    {
        Console.SetCursorPosition(position.Left, position.Top);
    }

    public void ClearCharsFromCheckpoint(string label, int chars)
    {
        if (!checkpoints.TryGetValue(label, out var checkpoint))
        {
            return;
        }
        SetCursorPosition(checkpoint);
        ClearChars(chars);
    }

    public void ClearChars(int chars)
    {
        Console.Write(new string(' ', chars));
        Console.CursorLeft -= chars;
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
