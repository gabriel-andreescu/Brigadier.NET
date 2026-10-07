// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
using Brigadier.NET.Builder;
using Brigadier.NET.Tree;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Brigadier.NET.Tests.tree;

public abstract class AbstractCommandNodeTest
{
    private readonly Command<object> _command = Substitute.For<Command<object>>();

    protected abstract CommandNode<object> GetCommandNode();

    [Fact]
    public void TestAddChild()
    {
        CommandNode<object> node = GetCommandNode();

        node.AddChild(new LiteralArgumentBuilder<object>("child1").Build());
        node.AddChild(new LiteralArgumentBuilder<object>("child2").Build());
        node.AddChild(new LiteralArgumentBuilder<object>("child1").Build());

        node.Children.Should().HaveCount(2);
    }

    [Fact]
    public void TestAddChildMergesGrandchildren()
    {
        CommandNode<object> node = GetCommandNode();

        node.AddChild(
            new LiteralArgumentBuilder<object>("child").Then(r => r.Literal("grandchild1")).Build()
        );

        node.AddChild(
            new LiteralArgumentBuilder<object>("child").Then(r => r.Literal("grandchild2")).Build()
        );

        node.Children.Should().HaveCount(1);
        node.Children.First().Children.Should().HaveCount(2);
    }

    [Fact]
    public void TestAddChildPreservesCommand()
    {
        CommandNode<object> node = GetCommandNode();

        node.AddChild(new LiteralArgumentBuilder<object>("child").Executes(_command).Build());
        node.AddChild(new LiteralArgumentBuilder<object>("child").Build());

        node.Children.First().Command.Should().Be(_command);
    }

    [Fact]
    public void TestAddChildPreservesDescription()
    {
        CommandNode<object> node = GetCommandNode();

        node.AddChild(new LiteralArgumentBuilder<object>("child").Describes("A child.").Build());
        node.AddChild(new LiteralArgumentBuilder<object>("child").Build());

        node.Children.First().Description!.String.Should().Be("A child.");
    }

    [Fact]
    public void TestAddChildOverwritesDescription()
    {
        CommandNode<object> node = GetCommandNode();

        node.AddChild(new LiteralArgumentBuilder<object>("child").Build());
        node.AddChild(new LiteralArgumentBuilder<object>("child").Describes("A child.").Build());

        node.Children.First().Description!.String.Should().Be("A child.");
    }

    [Fact]
    public void TestAddChildOverwritesCommand()
    {
        CommandNode<object> node = GetCommandNode();

        node.AddChild(new LiteralArgumentBuilder<object>("child").Build());
        node.AddChild(new LiteralArgumentBuilder<object>("child").Executes(_command).Build());

        node.Children.First().Command.Should().Be(_command);
    }
}
