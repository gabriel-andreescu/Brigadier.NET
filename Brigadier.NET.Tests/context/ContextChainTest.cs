using Brigadier.NET.Builder;
using Brigadier.NET.Context;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Brigadier.NET.Tests.context;

public class ContextChainTest
{
    [Fact]
    public void ExecuteAllForSingleCommand()
    {
        ResultConsumer<object> consumer = Substitute.For<ResultConsumer<object>>();
        Command<object> command = Substitute.For<Command<object>>();

        command.Invoke(Arg.Any<CommandContext<object>>()).Returns(4);

        var dispatcher = new CommandDispatcher<object>();
        dispatcher.Register(l => l.Literal("foo").Executes(command));
        string source = "compile_source";

        ParseResults<object> parse = dispatcher.Parse("foo", source);
        CommandContext<object> topContext = parse.Context.Build("foo");
        topContext.TryFlatten(out ContextChain<object>? chain).Should().BeTrue();

        string runtimeSource = "runtime_source";
        chain!.ExecuteAll(runtimeSource, consumer).Should().Be(4);

        command
            .Received()
            .Invoke(Arg.Is<CommandContext<object>>(c => ReferenceEquals(c.Source, runtimeSource)));
        consumer
            .Received()
            .Invoke(
                Arg.Is<CommandContext<object>>(c => ReferenceEquals(c.Source, runtimeSource)),
                true,
                4
            );
        //verifyNoMoreInteractions(consumer);
    }

    [Fact]
    public void ExecuteAllForRedirectedCommand()
    {
        ResultConsumer<object> consumer = Substitute.For<ResultConsumer<object>>();
        Command<object> command = Substitute.For<Command<object>>();

        command.Invoke(Arg.Any<CommandContext<object>>()).Returns(4);

        string redirectedSource = "redirected_source";

        var dispatcher = new CommandDispatcher<object>();
        dispatcher.Register(l => l.Literal("foo").Executes(command));
        dispatcher.Register(l => l.Literal("bar").Redirect(dispatcher.Root, _ => redirectedSource));
        string source = "compile_source";

        ParseResults<object> parse = dispatcher.Parse("bar foo", source);
        CommandContext<object> topContext = parse.Context.Build("bar foo");
        topContext.TryFlatten(out ContextChain<object>? chain).Should().BeTrue();

        string runtimeSource = "runtime_source";
        chain!.ExecuteAll(runtimeSource, consumer).Should().Be(4);

        command
            .Received()
            .Invoke(
                Arg.Is<CommandContext<object>>(c => ReferenceEquals(c.Source, redirectedSource))
            );
        consumer
            .Received()
            .Invoke(
                Arg.Is<CommandContext<object>>(c => ReferenceEquals(c.Source, redirectedSource)),
                true,
                4
            );
        //verifyNoMoreInteractions(consumer);
    }

    [Fact]
    public void SingleStageExecution()
    {
        var dispatcher = new CommandDispatcher<object>();
        dispatcher.Register(l => l.Literal("foo").Executes(_ => 1));
        object source = new object();

        ParseResults<object> result = dispatcher.Parse("foo", source);
        CommandContext<object> topContext = result.Context.Build("foo");
        topContext.TryFlatten(out ContextChain<object>? stage0).Should().BeTrue();

        stage0!.CurrentStage.Should().Be(ContextChain<object>.Stage.Execute);
        stage0.TopContext.Should().Be(topContext);
        stage0.NextStage().Should().BeNull();
    }

    [Fact]
    public void MultiStageExecution()
    {
        var dispatcher = new CommandDispatcher<object>();
        dispatcher.Register(l => l.Literal("foo").Executes(_ => 1));
        dispatcher.Register(l => l.Literal("bar").Redirect(dispatcher.Root));
        object source = new object();

        ParseResults<object> result = dispatcher.Parse("bar bar foo", source);
        CommandContext<object> topContext = result.Context.Build("bar bar foo");
        topContext.TryFlatten(out ContextChain<object>? stage0).Should().BeTrue();

        stage0!.CurrentStage.Should().Be(ContextChain<object>.Stage.Modify);
        stage0.TopContext.Should().Be(topContext);

        ContextChain<object>? stage1 = stage0.NextStage();
        stage1.Should().NotBeNull();
        stage1!.CurrentStage.Should().Be(ContextChain<object>.Stage.Modify);
        stage1.TopContext.Should().Be(topContext.Child);

        ContextChain<object>? stage2 = stage1.NextStage();
        stage2.Should().NotBeNull();
        stage2!.CurrentStage.Should().Be(ContextChain<object>.Stage.Execute);
        stage2.TopContext.Should().Be(topContext.Child!.Child);

        stage2.NextStage().Should().BeNull();
    }

    [Fact]
    public void MissingExecute()
    {
        var dispatcher = new CommandDispatcher<object>();
        dispatcher.Register(l => l.Literal("foo").Executes(_ => 1));
        dispatcher.Register(l => l.Literal("bar").Redirect(dispatcher.Root));

        object source = new object();
        CommandContext<object> topContext = dispatcher
            .Parse("bar bar", source)
            .Context.Build("bar bar");
        topContext.TryFlatten(out ContextChain<object>? flattened).Should().BeFalse();
        flattened.Should().BeNull();
    }
}
