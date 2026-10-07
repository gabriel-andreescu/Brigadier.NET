// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Brigadier.NET.ArgumentTypes;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Brigadier.NET.Tests.arguments;

public class StringArgumentTypeTest
{
    [Fact]
    public void TestParseWord()
    {
        IStringReader reader = Substitute.For<IStringReader>();
        reader.ReadUnquotedString().Returns("hello");
        Arguments.Word().Parse(reader).Should().BeEquivalentTo("hello");

        reader.Received().ReadUnquotedString();
    }

    [Fact]
    public void TestParseString()
    {
        IStringReader reader = Substitute.For<IStringReader>();
        reader.ReadString().Returns("hello world");
        Arguments.String().Parse(reader).Should().BeEquivalentTo("hello world");
        reader.Received().ReadString();
    }

    [Fact]
    public void TestParseGreedyString()
    {
        var reader = new StringReader("Hello world! This is a test.");
        Arguments
            .GreedyString()
            .Parse(reader)
            .Should()
            .BeEquivalentTo("Hello world! This is a test.");
        reader.CanRead().Should().Be(false);
    }

    [Fact]
    public void TestToString()
    {
        Arguments.String().ToString().Should().BeEquivalentTo("string()");
    }

    [Fact]
    public void testEscapeIfRequiredNotRequired()
    {
        StringArgumentType.EscapeIfRequired("hello").Should().BeEquivalentTo("hello");
        StringArgumentType.EscapeIfRequired("").Should().BeEquivalentTo("");
    }

    [Fact]
    public void testEscapeIfRequiredMultipleWords()
    {
        StringArgumentType
            .EscapeIfRequired("hello world")
            .Should()
            .BeEquivalentTo("\"hello world\"");
    }

    [Fact]
    public void testEscapeIfRequiredQuote()
    {
        StringArgumentType
            .EscapeIfRequired("hello \"world\"!")
            .Should()
            .BeEquivalentTo("\"hello \\\"world\\\"!\"");
    }

    [Fact]
    public void testEscapeIfRequiredEscapes()
    {
        StringArgumentType.EscapeIfRequired("\\").Should().BeEquivalentTo("\"\\\\\"");
    }

    [Fact]
    public void testEscapeIfRequiredSingleQuote()
    {
        StringArgumentType.EscapeIfRequired("\"").Should().BeEquivalentTo("\"\\\"\"");
    }
}
