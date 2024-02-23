namespace Giinn;

/// <summary>
///     Contains convenient methods for taking user input.
/// </summary>
public static class Input
{
    /// <summary>
    ///     Take direct input from user.
    /// </summary>
    /// <param name="label">
    ///     Hint for user to know what to input.
    /// </param>
    /// <param name="validator">
    ///     A function to validate the input,
    ///     and let user retry if it is not valid.
    /// </param>
    /// <param name="retryMessage">
    ///     A message to show the user when the input was not valid.
    /// </param>
    /// <param name="maximumRetries">
    ///     Maximum allowed retries. Defaults to -1, meaning endless retries.
    /// </param>
    /// <returns>
    ///     A valid input or null.
    /// </returns>
    public static string? Enter(
        string? label = null,
        Func<string?, bool>? validator = null,
        string? retryMessage = null,
        int maximumRetries = -1
    )
    {
        string? input;
        var isValid = true;
        var retries = 0;
        var endlessRetries = maximumRetries < 0;
        var io = new ConsoleHandler(LineTrackingMode.Track);
        do
        {
            io.ClearLines();

            if (!isValid)
            {
                PrintRetryMessage();

                if (!endlessRetries)
                {
                    PrintAttemptsLeft();
                }
            }

            (input, isValid) = TakeInput();

        } while (!isValid && (endlessRetries || retries < maximumRetries));

        return input;

        void PrintRetryMessage()
        {
            if (retryMessage is not null)
            {
                io.WriteLine(retryMessage + "\n");
            }
        }

        void PrintAttemptsLeft()
        {
            var attempts = maximumRetries - retries++;
            var attemptString = attempts is 1 ? "attempt" : "attempts";
            io.WriteLine($"{attempts} {attemptString} remaining.\n");
        }

        (string?, bool) TakeInput()
        {
            io.Write(label + " ");
            var input = io.ReadLine();
            var isValid = validator?.Invoke(input) ?? true;
            return (input, isValid);
        }
    }

    /// <summary>
    ///     Let user pick from a given set of options.
    /// </summary>
    /// <param name="options">
    ///     The set of options tho choose from.
    /// </param>
    /// <param name="label">
    ///     A hint to show the user.
    /// </param>
    /// <param name="selector">
    ///     The indicator showing what options is currently focued.
    ///     Default is " >"
    /// </param>
    /// <param name="limitRows">
    ///     Optionally limit the amount of options to show at the same time.
    ///     "..." will indicate when there are more option "above" or "below".
    /// </param>
    /// <param name="startAtIndex">
    ///     Set focus on the options with the given index at the start.
    ///     Default is 0.
    /// </param>
    /// <returns>
    ///     The index of the selected option, and the option
    /// </returns>
    public static (int index, string option) Pick(
        string[] options,
        string? label = null,
        string selector = " >",
        int? limitRows = null,
        int? startAtIndex = null
    )
    {
        var index = startAtIndex ?? 0;
        var optionsToShow = Math.Min(limitRows ?? options.Length, options.Length);
        var startIndex = optionsToShow < options.Length ? Math.Max(0, index - 1) : 0;

        var selected = false;
        var padding = new string(' ', selector.Length);
        var io = new ConsoleHandler(LineTrackingMode.Track);
        var optionsCheckpointLabel = "options";
        var choiceCheckpointLabel = "choice";

        io.DisableCursor();

        PrintLabel();

        io.AddCheckpoint(optionsCheckpointLabel);

        Choose();
        PrintChoice();

        io.EnableCursor();

        return (index, options[index]);

        void PrintLabel()
        {
            if (label is not null)
            {
                io.Write(label + " ");
            }
            io.AddCheckpoint(choiceCheckpointLabel);
            io.WriteLine();
        }

        void Choose()
        {
            while (!selected)
            {
                PrintSelectionArea();
                (index, selected) = HandleInput();
            }
        }

        void PrintChoice() => io.WriteLineFromCheckpoint(choiceCheckpointLabel, options[index]);

        (int, bool) HandleInput() => io.ReadKey().Key switch
        {
            ConsoleKey.UpArrow or ConsoleKey.W => (MoveUp(), false),
            ConsoleKey.DownArrow or ConsoleKey.D => (MoveDown(), false),
            ConsoleKey.Enter => (index, true),
            _ => (index, false)
        };

        void PrintSelectionArea()
        {
            io.ClearToCheckpoint(optionsCheckpointLabel);

            var doesNotShowTop = startIndex is not 0;
            var doesNotShowBottom = startIndex != options.Length - optionsToShow;

            for (int i = 0; i < optionsToShow; i++)
            {
                var optionIndex = i + startIndex;
                if (
                    (i is 0 && doesNotShowTop)
                    || (i == optionsToShow - 1 && doesNotShowBottom))
                {
                    io.WriteLine($"{padding} ...");
                    continue;
                }

                if (optionIndex == index)
                {
                    io.WriteLine($"{selector} {options[optionIndex]}");
                    continue;
                }
                io.WriteLine($"{padding} {options[optionIndex]}");
            }
        }

        int MoveUp()
        {
            if (index is 0)
            {
                startIndex = options.Length - optionsToShow;
                return options.Length - 1;
            }
            if (startIndex == index - 1 && index is not 1)
            {
                startIndex -= 1;
            }
            return index - 1;
        }

        int MoveDown()
        {
            if (index + 1 == options.Length)
            {
                startIndex = 0;
                return 0;
            }
            if (startIndex + optionsToShow - 1 == index + 1 && index != options.Length - 2)
            {
                startIndex += 1;
            }
            return index + 1;
        }
    }

    /// <summary>
    ///     Make user confirm something.
    /// </summary>
    /// <param name="label">
    ///     The hint to show the user. I.e. what to confirm.
    /// </param>
    /// <param name="default">
    ///     The fallback when user does not confirm nor reject.
    /// </param>
    /// <returns>
    ///     If the user confirmed or rejected.
    /// </returns>
    public static bool Confirm(string? label = null, bool @default = true)
    {
        Console.Write($"{label} ({(@default ? "Y/n" : "y/N")}): ");
        var position = Console.CursorLeft;
        var input = Console.ReadLine()?.ToLower();
        var result = input switch
        {
            "y" or "yes" => true,
            "" => @default,
            _ => false
        };

        Console.CursorTop -= 1;
        Console.CursorLeft = position;
        Console.Write(new string(' ', input?.Length ?? 0));
        Console.CursorLeft = position;
        Console.WriteLine(result ? "y" : "n");
        return result;
    }
}
