using BenchmarkDotNet.Running;

namespace Brigadier.NET.Benchmarks;

internal static class Program
{
	static void Main(string[] args)
	{
		BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
	}
}