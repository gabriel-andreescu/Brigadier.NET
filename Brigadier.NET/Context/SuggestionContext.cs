using Brigadier.NET.Tree;

namespace Brigadier.NET.Context;

[PublicAPI]
public class SuggestionContext<TSource>
{
	public CommandNode<TSource> Parent { get; }
	public int StartPos { get; }

	public SuggestionContext(CommandNode<TSource> parent, int startPos)
	{
		Parent = parent;
		StartPos = startPos;
	}
}