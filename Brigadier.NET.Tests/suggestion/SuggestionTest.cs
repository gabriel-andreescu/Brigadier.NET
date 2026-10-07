// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Brigadier.NET.Context;
using FluentAssertions;
using Xunit;

namespace Brigadier.NET.Tests.suggestion;

public class SuggestionTest {
	[Fact]
	public void applyInsertionStart() {
		var suggestion = new Suggestion.Suggestion(StringRange.At(0), "And so I said: ");
		suggestion.Apply("Hello world!").Should().BeEquivalentTo("And so I said: Hello world!");
	}

	[Fact]
	public void applyInsertionMiddle() {
		var suggestion = new Suggestion.Suggestion(StringRange.At(6), "small ");
		suggestion.Apply("Hello world!").Should().BeEquivalentTo("Hello small world!");
	}

	[Fact]
	public void applyInsertionEnd() {
		var suggestion = new Suggestion.Suggestion(StringRange.At(5), " world!");
		suggestion.Apply("Hello").Should().BeEquivalentTo("Hello world!");
	}

	[Fact]
	public void applyReplacementStart() {
		var suggestion = new Suggestion.Suggestion(StringRange.Between(0, 5), "Goodbye");
		suggestion.Apply("Hello world!").Should().BeEquivalentTo("Goodbye world!");
	}

	[Fact]
	public void applyReplacementMiddle() {
		var suggestion = new Suggestion.Suggestion(StringRange.Between(6, 11), "Alex");
		suggestion.Apply("Hello world!").Should().BeEquivalentTo("Hello Alex!");
	}

	[Fact]
	public void applyReplacementEnd() {
		var suggestion = new Suggestion.Suggestion(StringRange.Between(6, 12), "Creeper!");
		suggestion.Apply("Hello world!").Should().BeEquivalentTo("Hello Creeper!");
	}

	[Fact]
	public void applyReplacementEverything() {
		var suggestion = new Suggestion.Suggestion(StringRange.Between(0, 12), "Oh dear.");
		suggestion.Apply("Hello world!").Should().BeEquivalentTo("Oh dear.");
	}

	[Fact]
	public void expandUnchanged() {
		var suggestion = new Suggestion.Suggestion(StringRange.At(1), "oo");
		suggestion.Expand("f", StringRange.At(1)).Should().BeEquivalentTo(suggestion);
	}

	[Fact]
	public void expandLeft() {
		var suggestion = new Suggestion.Suggestion(StringRange.At(1), "oo");
		suggestion.Expand("f", StringRange.Between(0, 1)).Should().BeEquivalentTo(new Suggestion.Suggestion(StringRange.Between(0, 1), "foo"));
	}

	[Fact]
	public void expandRight() {
		var suggestion = new Suggestion.Suggestion(StringRange.At(0), "minecraft:");
		suggestion.Expand("fish", StringRange.Between(0, 4)).Should().BeEquivalentTo(new Suggestion.Suggestion(StringRange.Between(0, 4), "minecraft:fish"));
	}

	[Fact]
	public void expandBoth() {
		var suggestion = new Suggestion.Suggestion(StringRange.At(11), "minecraft:");
		suggestion.Expand("give Steve fish_block", StringRange.Between(5, 21)).Should().BeEquivalentTo(new Suggestion.Suggestion(StringRange.Between(5, 21), "Steve minecraft:fish_block"));
	}

	[Fact]
	public void expandReplacement() {
		var suggestion = new Suggestion.Suggestion(StringRange.Between(6, 11), "strangers");
		suggestion.Expand("Hello world!", StringRange.Between(0, 12)).Should().BeEquivalentTo(new Suggestion.Suggestion(StringRange.Between(0, 12), "Hello strangers!"));
	}
}