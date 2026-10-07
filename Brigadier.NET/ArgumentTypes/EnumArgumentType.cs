using Brigadier.NET.Context;
using Brigadier.NET.Exceptions;
using Brigadier.NET.Suggestion;

namespace Brigadier.NET.ArgumentTypes;

public class EnumArgumentType<T> : IArgumentType<T>
    where T : struct, Enum
{
    public T Parse(IStringReader reader)
    {
        int start = reader.Cursor;
        string input = reader.ReadUnquotedString();
        if (Enum.TryParse<T>(input, ignoreCase: true, out T level))
        {
            return level;
        }

        reader.Cursor = start;
        throw CommandSyntaxException
            .BuiltInExceptions.EnumInvalid()
            .CreateWithContext(reader, input, Names());
    }

    public IEnumerable<string> Examples => Names();

    public Task<Suggestions> ListSuggestions<TSource>(
        CommandContext<TSource> context,
        SuggestionsBuilder builder
    )
    {
        string remaining = builder.RemainingLowerCase;

        // Suggest enum names matching the current partial input (case-insensitive)
        foreach (string name in Names())
        {
            if (
                string.IsNullOrEmpty(remaining)
                || name.StartsWith(remaining, StringComparison.OrdinalIgnoreCase)
            )
            {
                builder.Suggest(name);
            }
        }

        return builder.BuildAsync();
    }

#if NET5_0_OR_GREATER
    private static string[] Names() => Enum.GetNames<T>();
#else
    private static string[] Names() => Enum.GetNames(typeof(T));
#endif
}
