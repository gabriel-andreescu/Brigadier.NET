using Brigadier.NET.Exceptions;

namespace Brigadier.NET.ArgumentTypes;

[PublicAPI]
public class LongArgumentType : IArgumentType<long>
{
	private static readonly IEnumerable<string> LongExamples = ["0", "123", "-123"];

	internal LongArgumentType(long minimum, long maximum)
	{
		Minimum = minimum;
		Maximum = maximum;
	}

	public long Minimum { get; }

	public long Maximum { get; }

	/// <exception cref="CommandSyntaxException" />
	public long Parse(IStringReader reader)
	{
        int start = reader.Cursor;
        long result = reader.ReadLong();
		if (result < Minimum) {
			reader.Cursor = start;
			throw CommandSyntaxException.BuiltInExceptions.LongTooLow().CreateWithContext(reader, result, Minimum);
		}
		if (result > Maximum) {
			reader.Cursor = start;
			throw CommandSyntaxException.BuiltInExceptions.LongTooHigh().CreateWithContext(reader, result, Maximum);
		}
		return result;
	}

	public IEnumerable<string> Examples => LongExamples;


	public override bool Equals(object? obj)
	{
		if (this == obj)
        {
            return true;
        }

        if (obj is not LongArgumentType that)
        {
            return false;
        }

        return Maximum == that.Maximum && Minimum == that.Minimum;
	}

	public override int GetHashCode()
	{
		return 31 * Minimum.GetHashCode() + Maximum.GetHashCode();
	}

	public override string ToString()
	{
		if (Minimum == long.MinValue && Maximum == long.MaxValue)
		{
			return "longArg()";
		}
		else if (Maximum == long.MaxValue)
		{
			return $"longArg({Minimum})";
		}
		else
		{
			return $"longArg({Minimum}, {Maximum})";
		}
	}
}