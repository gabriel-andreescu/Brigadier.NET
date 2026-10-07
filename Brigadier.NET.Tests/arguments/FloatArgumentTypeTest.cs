// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Brigadier.NET.Exceptions;
using FluentAssertions;
using Xunit;
using static Brigadier.NET.Arguments;

namespace Brigadier.NET.Tests.arguments;

public class FloatArgumentTypeTest
{
    [Fact]
    public void Parse()
    {
        var reader = new StringReader("15");
        Float().Parse(reader).Should().Be(15f);
        reader.CanRead().Should().Be(false);
    }

    [Fact]
    public void parseTooSmall()
    {
        var reader = new StringReader("-5");
        Float(0, 100)
            .Invoking(l => l.Parse(reader))
            .Should()
            .Throw<CommandSyntaxException>()
            .Where(ex => ex.Type == CommandSyntaxException.BuiltInExceptions.FloatTooLow())
            .Where(ex => ex.Cursor == 0);
    }

    [Fact]
    public void parseTooBig()
    {
        var reader = new StringReader("5");
        Float(-100, 0)
            .Invoking(l => l.Parse(reader))
            .Should()
            .Throw<CommandSyntaxException>()
            .Where(ex => ex.Type == CommandSyntaxException.BuiltInExceptions.FloatTooHigh())
            .Where(ex => ex.Cursor == 0);
    }

    [Fact]
    public void TestEquals()
    {
        new EqualsTester()
            .AddEqualityGroup(Float(), Float())
            .AddEqualityGroup(Float(-100, 100), Float(-100, 100))
            .AddEqualityGroup(Float(-100, 50), Float(-100, 50))
            .AddEqualityGroup(Float(-50, 100), Float(-50, 100));
    }

    [Fact]
    public void TestToString()
    {
        Float().ToString().Should().BeEquivalentTo("float()");
        Float(-100).ToString().Should().BeEquivalentTo("float(-100.0)");
        Float(-100, 100).ToString().Should().BeEquivalentTo("float(-100.0, 100.0)");
        //WTF? I actually expected the answer to be -2147483648.0
        Float(int.MinValue, 100).ToString().Should().BeEquivalentTo("float(-2147484000.0, 100.0)");
    }
}
