using Brigadier.NET.Builder;
using Brigadier.NET.Context;
using Brigadier.NET.Exceptions;
using Brigadier.NET.Suggestion;

namespace Brigadier.NET.Tree;

[PublicAPI]
public class LiteralCommandNode<TSource>
    : CommandNode<TSource>,
        IEquatable<LiteralCommandNode<TSource>>
{
    public LiteralCommandNode(
        string literal,
        Command<TSource>? command,
        Predicate<TSource> requirement,
        CommandNode<TSource>? redirect,
        RedirectModifier<TSource>? modifier,
        bool forks
    )
        : base(command, requirement, redirect, modifier, forks)
    {
        Literal = literal;
        LiteralLowerCase = literal.ToLowerInvariant();
    }

    public string Literal { get; }

    public string LiteralLowerCase { get; }

    public override string Name => Literal;

    public override void Parse(StringReader reader, CommandContextBuilder<TSource> contextBuilder)
    {
        int start = reader.Cursor;
        int end = Parse(reader);

        if (end > -1)
        {
            contextBuilder.WithNode(this, StringRange.Between(start, end));
            return;
        }

        throw CommandSyntaxException
            .BuiltInExceptions.LiteralIncorrect()
            .CreateWithContext(reader, Literal);
    }

    private int Parse(StringReader reader)
    {
        int start = reader.Cursor;
        if (reader.CanRead(Literal.Length))
        {
            int end = start + Literal.Length;
            if (reader.String.AsSpan(start, end - start).SequenceEqual(Literal.AsSpan()))
            {
                reader.Cursor = end;
                if (!reader.CanRead() || reader.Peek() == ' ')
                {
                    return end;
                }
                else
                {
                    reader.Cursor = start;
                }
            }
        }
        return -1;
    }

    public override Task<Suggestions> ListSuggestions(
        CommandContext<TSource> context,
        SuggestionsBuilder builder
    )
    {
        if (LiteralLowerCase.StartsWith(builder.RemainingLowerCase, StringComparison.Ordinal))
        {
            return (
                Description == null
                    ? builder.Suggest(Literal)
                    : builder.Suggest(Literal, Description)
            ).BuildAsync();
        }
        else
        {
            return Suggestions.Empty();
        }
    }

    protected override bool IsValidInput(string input)
    {
        return Parse(new StringReader(input)) > -1;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        return obj is LiteralCommandNode<TSource> other && Equals(other);
    }

    public bool Equals(LiteralCommandNode<TSource>? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return string.Equals(Literal, other.Literal, StringComparison.Ordinal);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Literal);
    }

    public override string UsageText => Literal;

    public override IArgumentBuilder<TSource, CommandNode<TSource>> CreateBuilder()
    {
        var builder = new LiteralArgumentBuilder<TSource>(Literal);
        builder.Requires(Requirement);
        builder.Forward(Redirect, RedirectModifier, IsFork);
        if (Command != null)
        {
            builder.Executes(Command);
        }

        if (Description != null)
        {
            builder.Describes(Description);
        }

        return builder;
    }

    public override IEnumerable<string> Examples => [Literal];
}
