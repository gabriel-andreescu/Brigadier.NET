// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Threading.Tasks;
using Brigadier.NET.Builder;
using Brigadier.NET.Context;
using Brigadier.NET.Suggestion;
using Brigadier.NET.Tree;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Brigadier.NET.Tests.tree;

public class ArgumentCommandNodeTest : AbstractCommandNodeTest {
	private readonly ArgumentCommandNode<object, int> _node;
	private readonly CommandContextBuilder<object> _contextBuilder;

	protected override CommandNode<object> GetCommandNode() {
		return _node;
	}

	public ArgumentCommandNodeTest()
	{
		_node = new RequiredArgumentBuilder<object, int>("foo", Arguments.Integer()).Build();
		_contextBuilder = new CommandContextBuilder<object>(new CommandDispatcher<object>(), new object(), new RootCommandNode<object>(), 0);
	}

	[Fact]
	public void TestParse(){
		var reader = new StringReader("123 456");
		_node.Parse(reader, _contextBuilder);

		_contextBuilder.GetArguments().ContainsKey("foo").Should().Be(true);
		_contextBuilder.GetArguments()["foo"].Result.Should().Be(123);
	}

	[Fact]
	public void TestUsage(){
		_node.UsageText.Should().Be("<foo>");
	}

	[Fact]
	public async Task TestSuggestions(){
        Suggestions result = await _node.ListSuggestions(_contextBuilder.Build(""), new SuggestionsBuilder("", 0));
		result.IsEmpty().Should().Be(true);
	}

	[Fact]
	public void TestEquals(){
        Command<object> command = Substitute.For<Command<object>>();

		new EqualsTester()
			.AddEqualityGroup(
				new RequiredArgumentBuilder<object, int>("foo", Arguments.Integer()).Build(),
				new RequiredArgumentBuilder<object, int>("foo", Arguments.Integer()).Build()
			)
			.AddEqualityGroup(
				new RequiredArgumentBuilder<object, int>("foo", Arguments.Integer()).Executes(command).Build(),
				new RequiredArgumentBuilder<object, int>("foo", Arguments.Integer()).Executes(command).Build()
			)
			.AddEqualityGroup(
				new RequiredArgumentBuilder<object, int>("bar", Arguments.Integer(-100, 100)).Build(),
				new RequiredArgumentBuilder<object, int>("bar", Arguments.Integer(-100, 100)).Build()
			)
			.AddEqualityGroup(
				new RequiredArgumentBuilder<object, int>("foo", Arguments.Integer(-100, 100)).Build(),
				new RequiredArgumentBuilder<object, int>("foo", Arguments.Integer(-100, 100)).Build()
			)
			.AddEqualityGroup(
				new RequiredArgumentBuilder<object, int>("foo", Arguments.Integer()).Then(
					new RequiredArgumentBuilder<object, int>("bar", Arguments.Integer())
				).Build(),
				new RequiredArgumentBuilder<object, int>("foo", Arguments.Integer()).Then(
					new RequiredArgumentBuilder<object, int>("bar", Arguments.Integer())
				).Build()
			);
	}

	[Fact]
	public void TestCreateBuilder()
	{
		var builder = (RequiredArgumentBuilder<object, int>)_node.CreateBuilder();
		builder.Name.Should().Be(_node.Name);
		builder.Type.Should().Be(_node.Type);
		builder.Requirement.Should().Be(_node.Requirement);
		builder.Command.Should().Be(_node.Command);
	}

	[Fact]
	public void TestCreateBuilderKeepsDescription()
	{
        ArgumentCommandNode<object, int> node = new RequiredArgumentBuilder<object, int>("foo", Arguments.Integer()).Describes("A number of foos.").Build();

		var builder = (RequiredArgumentBuilder<object, int>)node.CreateBuilder();

		builder.Description!.String.Should().Be("A number of foos.");
	}
}