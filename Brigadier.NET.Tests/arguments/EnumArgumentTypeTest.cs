using Brigadier.NET.Exceptions;
using FluentAssertions;
using Xunit;
using static Brigadier.NET.Arguments;

namespace Brigadier.NET.Tests.arguments;

public class EnumArgumentTypeTest
{
	private enum Level
	{
		Low,
		High,
	}

	[Fact]
	public void Parse()
	{
		var reader = new StringReader("high");
		Enum<Level>().Parse(reader).Should().Be(Level.High);
		reader.CanRead().Should().Be(false);
	}

	[Fact]
	public void ParseInvalid()
	{
		var reader = new StringReader("x medium");
		reader.Cursor = 2;
		Enum<Level>().Invoking(type => type.Parse(reader))
			.Should().Throw<CommandSyntaxException>()
			.Where(ex => ex.Type == CommandSyntaxException.BuiltInExceptions.EnumInvalid())
			.Where(ex => ex.RawMessage().String == "Invalid value 'medium', expected one of Low, High")
			.Where(ex => ex.Cursor == 2);
	}
}
