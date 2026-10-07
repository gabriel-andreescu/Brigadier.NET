// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Brigadier.NET;
using Brigadier.NET.Builder;

namespace Brigadier.NET.Benchmarks;

[MarkdownExporterAttribute.GitHub]
[MemoryDiagnoser]
public class ExecuteBenchmarks
{
    private CommandDispatcher<object> dispatcher = null!;
    private ParseResults<object> simple = null!;
    private ParseResults<object> singleRedirect = null!;
    private ParseResults<object> forkedRedirect = null!;

    [GlobalSetup]
    public void setup()
    {
        dispatcher = new CommandDispatcher<object>();
        dispatcher.Register(r => r.Literal("command").Executes(c => 0));
        dispatcher.Register(r => r.Literal("redirect").Redirect(dispatcher.Root));
        dispatcher.Register(r =>
            r.Literal("fork")
                .Fork(
                    dispatcher.Root,
                    o => new List<object> { new object(), new object(), new object() }
                )
        );
        simple = dispatcher.Parse("command", new object());
        singleRedirect = dispatcher.Parse("redirect command", new object());
        forkedRedirect = dispatcher.Parse("fork command", new object());
    }

    [Benchmark]
    public void executeSimple()
    {
        dispatcher.Execute(simple);
    }

    [Benchmark]
    public void executeSingleRedirect()
    {
        dispatcher.Execute(singleRedirect);
    }

    [Benchmark]
    public void executeForkedRedirect()
    {
        dispatcher.Execute(forkedRedirect);
    }
}
