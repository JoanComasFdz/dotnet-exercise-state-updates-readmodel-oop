// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using system_reporter_oop.IntegrationTests;

Console.WriteLine("Hello, World!");

BenchmarkRunner.Run<ApiBenchmark>();

public class ApiBenchmark
{
    [Benchmark]
    public async Task TestEndpoint()
    {
        var apiTests = new ApiTests(new ApiFcatory());
        await apiTests.PostEvent_Returns1Log("/1");
    }
}