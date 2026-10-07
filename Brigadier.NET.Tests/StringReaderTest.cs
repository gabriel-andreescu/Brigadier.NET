using Brigadier.NET.Exceptions;
using FluentAssertions;
using Xunit;

namespace Brigadier.NET.Tests;

public class StringReaderTest
{
    [Fact]
    public void CanRead()
    {
        var reader = new StringReader("abc");
        reader.CanRead().Should().BeTrue();
        reader.Skip(); // 'a'
        reader.CanRead().Should().BeTrue();
        reader.Skip(); // 'b'
        reader.CanRead().Should().BeTrue();
        reader.Skip(); // 'c'
        reader.CanRead().Should().BeFalse();
    }

    [Fact]
    public void RemainingLength()
    {
        var reader = new StringReader("abc");
        reader.RemainingLength.Should().Be(3);
        reader.Cursor = (1);
        reader.RemainingLength.Should().Be(2);
        reader.Cursor = (2);
        reader.RemainingLength.Should().Be(1);
        reader.Cursor = (3);
        reader.RemainingLength.Should().Be(0);
    }

    [Fact]
    public void canReadLength()
    {
        var reader = new StringReader("abc");
        reader.CanRead(1).Should().BeTrue();
        reader.CanRead(2).Should().BeTrue();
        reader.CanRead(3).Should().BeTrue();
        reader.CanRead(4).Should().BeFalse();
        reader.CanRead(5).Should().BeFalse();
    }

    [Fact]
    public void Peek()
    {
        var reader = new StringReader("abc");
        reader.Peek().Should().Be('a');
        reader.Cursor.Should().Be(0);
        reader.Cursor = 2;
        reader.Peek().Should().Be('c');
        reader.Cursor.Should().Be(2);
    }

    [Fact]
    public void peekLength()
    {
        var reader = new StringReader("abc");
        reader.Peek(0).Should().Be('a');
        reader.Peek(2).Should().Be('c');
        reader.Cursor.Should().Be(0);
        reader.Cursor = 1;
        reader.Peek(1).Should().Be('c');
        reader.Cursor.Should().Be(1);
    }

    [Fact]
    public void Read()
    {
        var reader = new StringReader("abc");
        reader.Next().Should().Be('a');
        reader.Next().Should().Be('b');
        reader.Next().Should().Be('c');
        reader.Cursor.Should().Be(3);
    }

    [Fact]
    public void Skip()
    {
        var reader = new StringReader("abc");
        reader.Skip();
        reader.Cursor.Should().Be(1);
    }

    [Fact]
    public void Remaining()
    {
        var reader = new StringReader("Hello!");
        reader.Remaining.Should().BeEquivalentTo("Hello!");
        reader.Cursor = (3);
        reader.Remaining.Should().BeEquivalentTo("lo!");
        reader.Cursor = (6);
        reader.Remaining.Should().BeEquivalentTo("");
    }

    [Fact]
    public void GetRead()
    {
        var reader = new StringReader("Hello!");
        reader.Read.Should().BeEquivalentTo("");
        reader.Cursor = (3);
        reader.Read.Should().BeEquivalentTo("Hel");
        reader.Cursor = (6);
        reader.Read.Should().BeEquivalentTo("Hello!");
    }

    [Fact]
    public void skipWhitespaceNone()
    {
        var reader = new StringReader("Hello!");
        reader.SkipWhitespace();
        reader.Cursor.Should().Be(0);
    }

    [Fact]
    public void skipWhitespaceMixed()
    {
        var reader = new StringReader(" \t \t\nHello!");
        reader.SkipWhitespace();
        reader.Cursor.Should().Be(5);
    }

    [Fact]
    public void skipWhitespaceEmpty()
    {
        var reader = new StringReader("");
        reader.SkipWhitespace();
        reader.Cursor.Should().Be(0);
    }

    [Fact]
    public void ReadUnquotedString()
    {
        var reader = new StringReader("hello world");
        reader.ReadUnquotedString().Should().BeEquivalentTo("hello");
        reader.Read.Should().BeEquivalentTo("hello");
        reader.Remaining.Should().BeEquivalentTo(" world");
    }

    [Fact]
    public void readUnquotedStringEmpty()
    {
        var reader = new StringReader("");
        reader.ReadUnquotedString().Should().BeEquivalentTo("");
        reader.Read.Should().BeEquivalentTo("");
        reader.Remaining.Should().BeEquivalentTo("");
    }

    [Fact]
    public void readUnquotedStringEmptyWithRemaining()
    {
        var reader = new StringReader(" hello world");
        reader.ReadUnquotedString().Should().BeEquivalentTo("");
        reader.Read.Should().BeEquivalentTo("");
        reader.Remaining.Should().BeEquivalentTo(" hello world");
    }

    [Fact]
    public void ReadQuotedString()
    {
        var reader = new StringReader("\"hello world\"");
        reader.ReadQuotedString().Should().BeEquivalentTo("hello world");
        reader.Read.Should().BeEquivalentTo("\"hello world\"");
        reader.Remaining.Should().BeEquivalentTo("");
    }

    [Fact]
    public void ReadSingleQuotedString()
    {
        var reader = new StringReader("'hello world'");
        reader.ReadQuotedString().Should().BeEquivalentTo("hello world");
        reader.Read.Should().BeEquivalentTo("'hello world'");
        reader.Remaining.Should().BeEquivalentTo("");
    }

    [Fact]
    public void ReadMixedQuotedStringDoubleInsideSingle()
    {
        var reader = new StringReader(@"'hello ""world""'");
        reader.ReadQuotedString().Should().BeEquivalentTo(@"hello ""world""");
        reader.Read.Should().BeEquivalentTo(@"'hello ""world""'");
        reader.Remaining.Should().BeEquivalentTo("");
    }

    [Fact]
    public void ReadMixedQuotedStringSingleInsideDouble()
    {
        var reader = new StringReader(@"""hello 'world'""");
        reader.ReadQuotedString().Should().BeEquivalentTo("hello 'world'");
        reader.Read.Should().BeEquivalentTo(@"""hello 'world'""");
        reader.Remaining.Should().BeEquivalentTo("");
    }

    [Fact]
    public void readQuotedStringEmpty()
    {
        var reader = new StringReader("");
        reader.ReadQuotedString().Should().BeEquivalentTo("");
        reader.Read.Should().BeEquivalentTo("");
        reader.Remaining.Should().BeEquivalentTo("");
    }

    [Fact]
    public void readQuotedStringEmptyQuoted()
    {
        var reader = new StringReader("\"\"");
        reader.ReadQuotedString().Should().BeEquivalentTo("");
        reader.Read.Should().BeEquivalentTo("\"\"");
        reader.Remaining.Should().BeEquivalentTo("");
    }

    [Fact]
    public void readQuotedStringEmptyQuotedWithRemaining()
    {
        var reader = new StringReader("\"\" hello world");
        reader.ReadQuotedString().Should().BeEquivalentTo("");
        reader.Read.Should().BeEquivalentTo("\"\"");
        reader.Remaining.Should().BeEquivalentTo(" hello world");
    }

    [Fact]
    public void readQuotedStringWithEscapedQuote()
    {
        var reader = new StringReader("\"hello \\\"world\\\"\"");
        reader.ReadQuotedString().Should().BeEquivalentTo("hello \"world\"");
        reader.Read.Should().BeEquivalentTo("\"hello \\\"world\\\"\"");
        reader.Remaining.Should().BeEquivalentTo("");
    }

    [Fact]
    public void readQuotedStringWithEscapedEscapes()
    {
        var reader = new StringReader("\"\\\\o/\"");
        reader.ReadQuotedString().Should().BeEquivalentTo("\\o/");
        reader.Read.Should().BeEquivalentTo("\"\\\\o/\"");
        reader.Remaining.Should().BeEquivalentTo("");
    }

    [Fact]
    public void readQuotedStringWithRemaining()
    {
        var reader = new StringReader("\"hello world\" foo bar");
        reader.ReadQuotedString().Should().BeEquivalentTo("hello world");
        reader.Read.Should().BeEquivalentTo("\"hello world\"");
        reader.Remaining.Should().BeEquivalentTo(" foo bar");
    }

    [Fact]
    public void readQuotedStringWithImmediateRemaining()
    {
        var reader = new StringReader("\"hello world\"foo bar");
        reader.ReadQuotedString().Should().BeEquivalentTo("hello world");
        reader.Read.Should().BeEquivalentTo("\"hello world\"");
        reader.Remaining.Should().BeEquivalentTo("foo bar");
    }

    [Fact]
    public void readQuotedStringNoOpen()
    {
        var reader = new StringReader("hello world\"");

        reader
            .Invoking(r => reader.ReadQuotedString())
            .Should()
            .Throw<CommandSyntaxException>()
            .Where(ex =>
                ex.Type == CommandSyntaxException.BuiltInExceptions.ReaderExpectedStartOfQuote()
            )
            .Where(ex => ex.Cursor == 0);
    }

    [Fact]
    public void readQuotedStringNoClose()
    {
        var reader = new StringReader("\"hello world");

        reader
            .Invoking(r => reader.ReadQuotedString())
            .Should()
            .Throw<CommandSyntaxException>()
            .Where(ex =>
                ex.Type == CommandSyntaxException.BuiltInExceptions.ReaderExpectedEndOfQuote()
            )
            .Where(ex => ex.Cursor == 12);
    }

    [Fact]
    public void readQuotedStringInvalidEscape()
    {
        var reader = new StringReader("\"hello\\nworld\"");

        reader
            .Invoking(r => reader.ReadQuotedString())
            .Should()
            .Throw<CommandSyntaxException>()
            .Where(ex => ex.Type == CommandSyntaxException.BuiltInExceptions.ReaderInvalidEscape())
            .Where(ex => ex.Cursor == 7);
    }

    [Fact]
    public void ReadQuotedStringInvalidQuoteEscape()
    {
        var reader = new StringReader("'hello\\\"\'world");

        reader
            .Invoking(r => reader.ReadQuotedString())
            .Should()
            .Throw<CommandSyntaxException>()
            .Where(ex => ex.Type == CommandSyntaxException.BuiltInExceptions.ReaderInvalidEscape())
            .Where(ex => ex.Cursor == 7);
    }

    [Fact]
    public void ReadQuotedStringNoQuotes()
    {
        var reader = new StringReader("hello world");
        reader.ReadString().Should().Be("hello");
        reader.Read.Should().BeEquivalentTo("hello");
        reader.Remaining.Should().BeEquivalentTo(" world");
    }

    [Fact]
    public void ReadQuotedStringSingleQuotes()
    {
        var reader = new StringReader("'hello world'");
        reader.ReadString().Should().Be("hello world");
        reader.Read.Should().BeEquivalentTo("'hello world'");
        reader.Remaining.Should().BeEquivalentTo("");
    }

    [Fact]
    public void ReadQuotedStringDoubleQuotes()
    {
        var reader = new StringReader(@"""hello world""");
        reader.ReadString().Should().Be("hello world");
        reader.Read.Should().BeEquivalentTo(@"""hello world""");
        reader.Remaining.Should().BeEquivalentTo("");
    }

    [Fact]
    public void ReadInt()
    {
        var reader = new StringReader("1234567890");
        reader.ReadInt().Should().Be(1234567890);
        reader.Read.Should().BeEquivalentTo("1234567890");
        reader.Remaining.Should().BeEquivalentTo("");
    }

    [Fact]
    public void readIntNegative()
    {
        var reader = new StringReader("-1234567890");
        reader.ReadInt().Should().Be(-1234567890);
        reader.Read.Should().BeEquivalentTo("-1234567890");
        reader.Remaining.Should().BeEquivalentTo("");
    }

    [Fact]
    public void readIntInvalid()
    {
        var reader = new StringReader("12.34");

        reader
            .Invoking(r => reader.ReadInt())
            .Should()
            .Throw<CommandSyntaxException>()
            .Where(ex => ex.Type == CommandSyntaxException.BuiltInExceptions.ReaderInvalidInt())
            .Where(ex => ex.Cursor == 0);
    }

    [Fact]
    public void readIntNone()
    {
        var reader = new StringReader("");

        reader
            .Invoking(r => reader.ReadInt())
            .Should()
            .Throw<CommandSyntaxException>()
            .Where(ex => ex.Type == CommandSyntaxException.BuiltInExceptions.ReaderExpectedInt())
            .Where(ex => ex.Cursor == 0);
    }

    [Fact]
    public void readIntWithRemaining()
    {
        var reader = new StringReader("1234567890 foo bar");
        reader.ReadInt().Should().Be(1234567890);
        reader.Read.Should().BeEquivalentTo("1234567890");
        reader.Remaining.Should().BeEquivalentTo(" foo bar");
    }

    [Fact]
    public void readIntWithRemainingImmediate()
    {
        var reader = new StringReader("1234567890foo bar");
        reader.ReadInt().Should().Be(1234567890);
        reader.Read.Should().BeEquivalentTo("1234567890");
        reader.Remaining.Should().BeEquivalentTo("foo bar");
    }

    [Fact]
    public void ReadLong()
    {
        var reader = new StringReader("1234567890");
        reader.ReadLong().Should().Be(1234567890L);
        reader.Read.Should().BeEquivalentTo("1234567890");
        reader.Remaining.Should().BeEquivalentTo("");
    }

    [Fact]
    public void readLongNegative()
    {
        var reader = new StringReader("-1234567890");
        reader.ReadLong().Should().Be(-1234567890L);
        reader.Read.Should().BeEquivalentTo("-1234567890");
        reader.Remaining.Should().BeEquivalentTo("");
    }

    [Fact]
    public void readLongInvalid()
    {
        var reader = new StringReader("12.34");

        reader
            .Invoking(r => reader.ReadLong())
            .Should()
            .Throw<CommandSyntaxException>()
            .Where(ex => ex.Type == CommandSyntaxException.BuiltInExceptions.ReaderInvalidLong())
            .Where(ex => ex.Cursor == 0);
    }

    [Fact]
    public void readLongNone()
    {
        var reader = new StringReader("");

        reader
            .Invoking(r => reader.ReadLong())
            .Should()
            .Throw<CommandSyntaxException>()
            .Where(ex => ex.Type == CommandSyntaxException.BuiltInExceptions.ReaderExpectedLong())
            .Where(ex => ex.Cursor == 0);
    }

    [Fact]
    public void readLongWithRemaining()
    {
        var reader = new StringReader("1234567890 foo bar");
        reader.ReadLong().Should().Be(1234567890L);
        reader.Read.Should().BeEquivalentTo("1234567890");
        reader.Remaining.Should().BeEquivalentTo(" foo bar");
    }

    [Fact]
    public void readLongWithRemainingImmediate()
    {
        var reader = new StringReader("1234567890foo bar");
        reader.ReadLong().Should().Be(1234567890L);
        reader.Read.Should().BeEquivalentTo("1234567890");
        reader.Remaining.Should().BeEquivalentTo("foo bar");
    }

    [Fact]
    public void ReadDouble()
    {
        var reader = new StringReader("123");
        reader.ReadDouble().Should().Be(123.0);
        reader.Read.Should().BeEquivalentTo("123");
        reader.Remaining.Should().BeEquivalentTo("");
    }

    [Fact]
    public void readDoubleWithDecimal()
    {
        var reader = new StringReader("12.34");
        reader.ReadDouble().Should().Be(12.34);
        reader.Read.Should().BeEquivalentTo("12.34");
        reader.Remaining.Should().BeEquivalentTo("");
    }

    [Fact]
    public void readDoubleNegative()
    {
        var reader = new StringReader("-123");
        reader.ReadDouble().Should().Be(-123.0);
        reader.Read.Should().BeEquivalentTo("-123");
        reader.Remaining.Should().BeEquivalentTo("");
    }

    [Fact]
    public void readDoubleInvalid()
    {
        var reader = new StringReader("12.34.56");
        reader
            .Invoking(r => reader.ReadDouble())
            .Should()
            .Throw<CommandSyntaxException>()
            .Where(ex => ex.Type == CommandSyntaxException.BuiltInExceptions.ReaderInvalidDouble())
            .Where(ex => ex.Cursor == 0);
    }

    [Fact]
    public void readDoubleNone()
    {
        var reader = new StringReader("");
        reader
            .Invoking(r => reader.ReadDouble())
            .Should()
            .Throw<CommandSyntaxException>()
            .Where(ex => ex.Type == CommandSyntaxException.BuiltInExceptions.ReaderExpectedDouble())
            .Where(ex => ex.Cursor == 0);
    }

    [Fact]
    public void readDoubleWithRemaining()
    {
        var reader = new StringReader("12.34 foo bar");
        reader.ReadDouble().Should().Be(12.34);
        reader.Read.Should().BeEquivalentTo("12.34");
        reader.Remaining.Should().BeEquivalentTo(" foo bar");
    }

    [Fact]
    public void readDoubleWithRemainingImmediate()
    {
        var reader = new StringReader("12.34foo bar");
        reader.ReadDouble().Should().Be(12.34);
        reader.Read.Should().BeEquivalentTo("12.34");
        reader.Remaining.Should().BeEquivalentTo("foo bar");
    }

    [Fact]
    public void ReadFloat()
    {
        var reader = new StringReader("123");
        reader.ReadFloat().Should().Be(123.0f);
        reader.Read.Should().BeEquivalentTo("123");
        reader.Remaining.Should().BeEquivalentTo("");
    }

    [Fact]
    public void readFloatWithDecimal()
    {
        var reader = new StringReader("12.34");
        reader.ReadFloat().Should().Be(12.34f);
        reader.Read.Should().BeEquivalentTo("12.34");
        reader.Remaining.Should().BeEquivalentTo("");
    }

    [Fact]
    public void readFloatNegative()
    {
        var reader = new StringReader("-123");
        reader.ReadFloat().Should().Be(-123.0f);
        reader.Read.Should().BeEquivalentTo("-123");
        reader.Remaining.Should().BeEquivalentTo("");
    }

    [Fact]
    public void readFloatInvalid()
    {
        var reader = new StringReader("12.34.56");

        reader
            .Invoking(r => r.ReadFloat())
            .Should()
            .Throw<CommandSyntaxException>()
            .Where(ex => ex.Type == CommandSyntaxException.BuiltInExceptions.ReaderInvalidFloat())
            .Where(ex => ex.Cursor == 0);
    }

    [Fact]
    public void readFloatNone()
    {
        var reader = new StringReader("");

        reader
            .Invoking(r => r.ReadFloat())
            .Should()
            .Throw<CommandSyntaxException>()
            .Where(ex => ex.Type == CommandSyntaxException.BuiltInExceptions.ReaderExpectedFloat())
            .Where(ex => ex.Cursor == 0);
    }

    [Fact]
    public void readFloatWithRemaining()
    {
        var reader = new StringReader("12.34 foo bar");
        reader.ReadFloat().Should().Be(12.34f);
        reader.Read.Should().BeEquivalentTo("12.34");
        reader.Remaining.Should().BeEquivalentTo(" foo bar");
    }

    [Fact]
    public void readFloatWithRemainingImmediate()
    {
        var reader = new StringReader("12.34foo bar");
        reader.ReadFloat().Should().Be(12.34f);
        reader.Read.Should().BeEquivalentTo("12.34");
        reader.Remaining.Should().BeEquivalentTo("foo bar");
    }

    [Fact]
    public void expectCorrect()
    {
        var reader = new StringReader("abc");
        reader.Expect('a');
        reader.Cursor.Should().Be(1);
    }

    [Fact]
    public void expectIncorrect()
    {
        var reader = new StringReader("bcd");
        reader
            .Invoking(r => r.Expect('a'))
            .Should()
            .Throw<CommandSyntaxException>()
            .Where(ex => ex.Type == CommandSyntaxException.BuiltInExceptions.ReaderExpectedSymbol())
            .Where(ex => ex.Cursor == 0);
    }

    [Fact]
    public void expectNone()
    {
        var reader = new StringReader("");
        reader
            .Invoking(r => r.Expect('a'))
            .Should()
            .Throw<CommandSyntaxException>()
            .Where(ex => ex.Type == CommandSyntaxException.BuiltInExceptions.ReaderExpectedSymbol())
            .Where(ex => ex.Cursor == 0);
    }

    [Fact]
    public void readBooleanCorrect()
    {
        var reader = new StringReader("true");
        reader.ReadBoolean().Should().Be(true);
        reader.Read.Should().BeEquivalentTo("true");
    }

    [Fact]
    public void readBooleanIncorrect()
    {
        var reader = new StringReader("tuesday");
        reader
            .Invoking(r => r.ReadBoolean())
            .Should()
            .Throw<CommandSyntaxException>()
            .Where(e => e.Type == CommandSyntaxException.BuiltInExceptions.ReaderInvalidBool())
            .Where(e => e.Cursor == 0);
    }

    [Fact]
    public void readBooleanNone()
    {
        var reader = new StringReader("");
        reader
            .Invoking(r => reader.ReadBoolean())
            .Should()
            .Throw<CommandSyntaxException>()
            .Where(ex => ex.Type == CommandSyntaxException.BuiltInExceptions.ReaderExpectedBool())
            .Where(ex => ex.Cursor == 0);
    }
}
