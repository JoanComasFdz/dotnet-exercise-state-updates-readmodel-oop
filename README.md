# Exercise: Create a log from state changes in a traditional OOP

In this project I will try to approach a little functionality I faced
recently, in the tradutional Object Oriented Programming way.

This is a simplification and an abstraction of the actual functionality, not the full picture.
Do not critizie its architecture based on how would you have implemented it.

I will create also a traditional OOP implementation to be able to properly compare both approaches.

I consider traditional OOP to include SOLID principles.

## The functionality
The `hardware-connetion-monitor` service monitors the state of the connection to a hardware unit. When that
connection state changes, it publishes it with the hardware unit id, the new state and the current datetime.
The state can be `CONNECTED`, `DISCONNECTED` or `WAITING`.

The `system-reporter` service is listening to the `hardware-connetion-monitor` state changes and creates a log
of disconnections, which includes the hardware unit id, the state, the start time and the end time.

The log is created as follows:

1. If the last disconnection with the hardware unit id does not exist and the given state is not `CONNECTED`, 
 create a new disconnection with the given hardware unit id, state and the datetime as start time, leaveing end time null.

2. If the last disconnection with the hardware unit id exists and the end time is null, set its end time to the given datetime.

3. If the last disconnection with the hardware unit id exists and the given state is not `CONNECTED`,
 create a new disconnection with the given hardware unit id, state and the datetime as start time, leaveing end time null.

 > Important: There is no case where 2 events come with the same hardware unit id and the same state.

## Implementation constrains
- Only the `system-reporter` will be implemented.
- The events will be simulated via a Web API Controller.
- Since it is a read model, the data should be stored already in the way and format that will be returned when queried.
- Since it is a read model, the Controller method to return the log will not do any complex SQL query nor any in memory
 operation, besides obtaining the data from the DB and returning it. No mapping, no Linq, no loops, no ifs.

## My take on how to implement it
My goal is to create an impure-pure-impure sandwitch as follows:
1. Query the DB
2. Decide what to do
3. Update the DB accordingly

So all the logic to decide how the DB is updated should be in step 2, and it has to return everything that step 3 needs
to do all the DB updates.

To implement that I will try to do a descriminated union so that step 3 can do pattern matching. So let's look at the
cases:

Cases 1 and 3 tell me that it is the same action: Create a new disconnection and return it as action `AddNew`.

For case 2, the endTime has to be updated and returned as `UpdateLast`.
But case 2 and 3 are two consecutive things to do for 1 single event, for example:

> Event comes with state `WAITING` and a disconnection exists with end time `null`.

In this case, the actions are:
1. Update the last disconnection end time to the event datetime.
1. Add new disconnection with state `WAITING`, start time is the event datetime and end time is null.

Finally, there is a case in which the log only has to update the end time of the last disconnection: When the last end time is null and the event 
state is `CONNECTED`.

So, step 2 return 2 cases:
1. `AddNew`, which carries the new disconnection instance.
1. `UpdateLast` which carries the edited disconnection.
1. `UpdateLastEndTimeAndAddNew`, which carries the update disconnection and the new disconnection instance.

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