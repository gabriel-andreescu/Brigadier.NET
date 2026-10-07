// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Brigadier.NET.Builder;
using Brigadier.NET.Tree;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Brigadier.NET.Tests.builder;

public class LiteralArgumentBuilderTest
{
    private readonly LiteralArgumentBuilder<object> _builder;
    private readonly Command<object> _command = Substitute.For<Command<object>>();

    public LiteralArgumentBuilderTest()
    {
        _builder = new LiteralArgumentBuilder<object>("foo");
    }

    [Fact]
    public void TestBuild()
    {
        LiteralCommandNode<object> node = _builder.Build();

        node.Literal.Should().Be("foo");
    }

    [Fact]
    public void TestBuildWithDescription()
    {
        LiteralCommandNode<object> node = _builder.Describes("Says foo.").Build();

        node.Description!.String.Should().Be("Says foo.");
    }

    [Fact]
    public void TestBuildWithExecutor()
    {
        LiteralCommandNode<object> node = _builder.Executes(_command).Build();

        node.Literal.Should().Be("foo");
        node.Command.Should().Be(_command);
    }

    [Fact]
    public void TestBuildWithChildren()
    {
        _builder.Then(r => r.Argument("bar", Arguments.Integer()));
        _builder.Then(r => r.Argument("baz", Arguments.Integer()));
        LiteralCommandNode<object> node = _builder.Build();

        node.Children.Should().HaveCount(2);
    }
}
