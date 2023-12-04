# Exercise: Create a log from state changes in a traditional OOP

This repo contains the implementation in traditional Object Oriented Programming way of the functinality described here:
- https://github.com/JoanComasFdz/dotnet-exercise-state-updates-readmodel-docs

## Technical details
- I have disabled AoT because it just adds noise for this exercise.
- I have added Unit Tests as part of the traditional apporach.
- I have added Integration Tests with the main goal of benchnmarking them.
- I have added Benchmarks for the Integration Tests to get a baseline time estimation.

Run the Benchmarks yourself with `dotnet run -c Release`. My results

```
| Method       | Mean     | Error    | StdDev   |
|------------- |---------:|---------:|---------:|
| TestEndpoint | 36.54 ms | 3.733 ms | 11.01 ms |
```

## Alternative 1 conclusions
- Nothing new here for someone who has been working professionally already for some time.
- Absolute overkill for the functionality at hand, my main issues are:
  - No driver or requirement requires to isolate the functionality in a service and or the queries in a repository.
  - The repository is just a facade.
  - The service and repositories use a 1:1 interface and are only used in one place.
- The main argument is usually testability, but creating tests for an ASP .NET Core application is really easy nowadays, maybe it takes 5 more
minutes once, which is negligible.
- In terms of running tests, the benchmarks show a more than acceptable baseline execution time of ~37ms. This means that if your service is
small enough, writing the tests as ASP .NET Core integration tests (with in memory DB) won't have a measurable impact on your daily work.

## Alternative 2 conclcusions
- In this particular case, alternative 2 seems more than enough for what it needs to be achieved.
- Testability is not impacted as the integration tests can cover all the 4 paths very easily.
- Getting rid of 1:1 interfaces used only in once with shallow classes gives a lot of depth to the code in the controller.