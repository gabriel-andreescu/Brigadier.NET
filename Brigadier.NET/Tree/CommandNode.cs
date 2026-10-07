using System.Collections;
using Brigadier.NET.Builder;
using Brigadier.NET.Context;
using Brigadier.NET.Exceptions;
using Brigadier.NET.Suggestion;

namespace Brigadier.NET.Tree;

[PublicAPI]
public abstract class CommandNode<TSource> : IEnumerable<CommandNode<TSource>>
{
    private readonly IDictionary<string, CommandNode<TSource>> _children =
        new Dictionary<string, CommandNode<TSource>>();
    private readonly Dictionary<string, LiteralCommandNode<TSource>> _literals =
        new Dictionary<string, LiteralCommandNode<TSource>>();
    private readonly IDictionary<string, ArgumentCommandNode<TSource>> _arguments =
        new Dictionary<string, ArgumentCommandNode<TSource>>();

    protected CommandNode(
        Command<TSource>? command,
        Predicate<TSource> requirement,
        CommandNode<TSource>? redirect,
        RedirectModifier<TSource>? modifier,
        bool forks
    )
    {
        Command = command;
        Requirement = requirement;
        Redirect = redirect;
        RedirectModifier = modifier;
        IsFork = forks;
    }

    public Command<TSource>? Command { get; set; }

    public IMessage? Description { get; set; }

    //PortNote: ICollection might be needed
    public ICollection<CommandNode<TSource>> Children => _children.Values;

    //PortNote: .NET Dictionaries throw if the key does not exist.
    public CommandNode<TSource>? GetChild(string name)
    {
        return _children.TryGetValue(name, out CommandNode<TSource>? node) ? node : null;
    }

    public CommandNode<TSource>? Redirect { get; }

    public RedirectModifier<TSource>? RedirectModifier { get; }

    public bool CanUse(TSource source)
    {
        return Requirement(source);
    }

    /// <summary>
    /// Provide collection initialization, same as calling AddChild
    /// </summary>
    /// <param name="node">Command nodes to add</param>
    public void Add(CommandNode<TSource> node)
    {
        AddChild(node);
    }

    /// <summary>
    /// Provide collection initialization, same as calling AddChild
    /// </summary>
    /// <param name="nodes">Command nodes to add</param>
    public void Add(params CommandNode<TSource>[] nodes)
    {
        foreach (CommandNode<TSource> node in nodes)
        {
            AddChild(node);
        }
    }

    public void AddChild(CommandNode<TSource> node)
    {
        if (node is RootCommandNode<TSource>)
        {
            throw new InvalidOperationException(
                $"Cannot add a {nameof(RootCommandNode<TSource>)} as a child to any other {nameof(CommandNode<TSource>)}"
            );
        }

        if (_children.TryGetValue(node.Name, out CommandNode<TSource>? child))
        {
            // We've found something to merge onto
            if (node.Command != null)
            {
                child.Command = node.Command;
            }

            if (node.Description != null)
            {
                child.Description = node.Description;
            }

            foreach (CommandNode<TSource> grandchild in node.Children)
            {
                child.AddChild(grandchild);
            }
        }
        else
        {
            _children.Add(node.Name, node);
            if (node is LiteralCommandNode<TSource> literalCommandNode)
            {
                _literals.Add(node.Name, literalCommandNode);
            }
            else if (node is ArgumentCommandNode<TSource> argumentCommandNode)
            {
                _arguments.Add(node.Name, argumentCommandNode);
            }
        }
    }

    public void FindAmbiguities(AmbiguityConsumer<TSource> consumer)
    {
        var matches = new HashSet<string>();

        foreach (CommandNode<TSource> child in _children.Values)
        {
            foreach (CommandNode<TSource> sibling in _children.Values)
            {
                if (child == sibling)
                {
                    continue;
                }

                foreach (string input in child.Examples)
                {
                    if (sibling.IsValidInput(input))
                    {
                        matches.Add(input);
                    }
                }

                if (matches.Count > 0)
                {
                    consumer(this, child, sibling, matches);
                    matches = [];
                }
            }

            child.FindAmbiguities(consumer);
        }
    }

    protected abstract bool IsValidInput(string input);

    public Predicate<TSource> Requirement { get; }

    public abstract string Name { get; }

    public abstract string UsageText { get; }

    /// <exception cref="CommandSyntaxException"></exception>
    public abstract void Parse(StringReader reader, CommandContextBuilder<TSource> contextBuilder);

    /// <exception cref="CommandSyntaxException"></exception>
    public abstract Task<Suggestions> ListSuggestions(
        CommandContext<TSource> context,
        SuggestionsBuilder builder
    );

    public abstract IArgumentBuilder<TSource, CommandNode<TSource>> CreateBuilder();

    public IEnumerable<CommandNode<TSource>> GetRelevantNodes(StringReader input)
    {
        if (_literals.Count > 0)
        {
            int cursor = input.Cursor;
            while (input.CanRead() && input.Peek() != ' ')
            {
                input.Skip();
            }
            string text = input.String.Substring(cursor, input.Cursor - cursor);
            input.Cursor = cursor;
            if (_literals.TryGetValue(text, out LiteralCommandNode<TSource>? literal))
            {
                yield return literal;
            }
            else
            {
                foreach (ArgumentCommandNode<TSource> node in _arguments.Values)
                {
                    yield return node;
                }
            }
        }
        else
        {
            foreach (ArgumentCommandNode<TSource> node in _arguments.Values)
            {
                yield return node;
            }
        }
    }

    public bool IsFork { get; }

    public abstract IEnumerable<string> Examples { get; }

    public IEnumerator<CommandNode<TSource>> GetEnumerator()
    {
        return _children.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
