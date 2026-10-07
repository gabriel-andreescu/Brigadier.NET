using System;
using Brigadier.NET.Tree;
using FluentAssertions;
using FluentAssertions.Primitives;

namespace Brigadier.NET.Tests;

internal sealed class EqualsTester
{
    public EqualsTester AddEqualityGroup<T>(params T[] equivalents)
    {
        foreach (T? equivalent in equivalents)
        {
            foreach (T? equivalent1 in equivalents)
            {
                if (ReferenceEquals(equivalent, equivalent1))
                {
                    continue;
                }

                equivalent.Should().Be(equivalent1);
            }
        }

        return this;
    }
}

public static class TestNodeExtensions
{
    public static ObjectAssertions Should<TArg>(this CommandNode<TArg>? obj)
    {
        return ((object?)obj).Should();
    }
}
