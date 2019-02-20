# NCore Event Source

TLDR; Just [Get Started](#how-to-get-started)

What this project is:
1. A simple [event-sourcing](https://microservices.io/patterns/data/event-sourcing.html) approach for small-to-medium sized microservice projects
1. A de-coupled eventing pattern
1. A simple object-sharing pattern for micro-service [database-per-service](https://microservices.io/patterns/data/database-per-service.html) pattern

Who may beenfit from this?
_Anyone who is using a Micro-service based approach to a project._

What this project isn't:
1. A solution for massive-scale (e.g. Netflix) - hey it's still v 0.1
1. A solution to implement [CQRS](https://microservices.io/patterns/data/cqrs.html) - this is not intending to solve that

# Example Scenario
Imagine you are building a set of micrservices to divide what was typically a monolithic application.
The goals you are aiming for while building this appliation are:
1. Provide isolation so that one part of the application does not impact the other [example](https://youtu.be/-AfZxdXa7yc?t=2103).
1. Provide simpler services that can scale independently

This fictitious, but not too abstract application has 3 areas (Customer, Job, and Order).  
In addition it has just 3 back-end services (Email Delivery, Billing, and Accounting system).
Consider these alternatives:

## Point-to-Point
![Point to Point](https://raw.githubusercontent.com/rposener/NCoreEventSource/master/Point2Point.PNG)

Many interconnections, complexity and distributed coordination.  Many opportunities to create tight-coupling and introduce cascading failures.

##  NCore Event Sourced
![NCore Event System](https://raw.githubusercontent.com/rposener/NCoreEventSource/master/NCoreEvent.PNG)

Event-driven, shared-objects, and recovers from outages via event catch-up.

# How to get started
You will need to do 2 things to implement this pattern:
1. Setup your NCoreEvent service (the green box in the middle above)
1. Add the NCoreEvetnClient to each micro-service project

## Part 1 - Creating the NCoreEvent Service
This is pretty simple - here are the steps:
1. Create a new .Net Core Web Project
1. Add reference to NCoreEventServer
1. Add the EventServer to your Startup Class in the **ConfigureServices** method

e.g.
```c#
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc()
        .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

// --------- Add NCore Event Server ------------
    services.AddEventServer()
        .AddInMemoryStores()
        .AddHttpPostDeliveryWithRetry();
// --------- End NCore Event Server ------------
}
```

1. Call **UseEventServer()** to your Startup Class in the **Configure** method

e.g.
```c#
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    /// ... parts omitted for brevity ...
    app.UseHttpsRedirection();
    app.UseEventServer();   // <<- This line here
    app.UseMvc();
}
```

Of course there is more to configure and do (those lines under **AddEventServer** allow you to configure persistence and how messages are sent and delivered).

## Part 2 - Add NCoreEventClient to your micro-service projects
This is event simpler - here are the steps:
1. Add reference to NCoreEventClient
1. Add the EventCore to your Startup Class in the **ConfigureServices** method
e.g.
```c#
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc()
        .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

// --------- Add NCore Event Core ------------
    services.AddEventCore(
        Configuration.GetSection("NCoreEventOptions"));
// --------- End NCore Event Core ------------
}
```
1. Add the Configuration options to your configuration (typically appsettings.json)
```js
{
    "NCoreEventOptions": {
    "SubscriberId": "Microservice-1",
    "SubscriberBaseUrl": "https://localhost:5003/",
    "EventServerUrl": "https://localhost:5001/"
  }
}
```
1. When you want to **Send** an event just inject the **NCoreEventService** object and call **SendMessage**.

```c#
public class MyController: ControllerBase 
{
    private readonly NCoreEventService eventServerService;

    public MyController(NCoreEventService eventServerService)
    {
        this.eventServerService = eventServerService ?? throw new ArgumentNullException(nameof(eventServerService));
    }

    public async Task SomeMethod()
    {
        await eventServerService.SendMessage("EventTopic", anyData);
    }
}
```

1. When you want to **Receive** an event just decorate a Controller **POST** method with the **NCoreEvent** or **NCoreObject** attributes.

```c#
public class MyController: ControllerBase 
{
    [NCoreEvent("EventTopic")]
    public async Task Post([FromBody] TestEventData value)
    {
        logger.LogInformation("Event Received :::::: " + value.TestValue);
    }

    [NCoreObject("TestObject")]
    [HttpPost]
    public IActionResult PostObject(TestObject test)
    {


        logger.LogInformation("Object Update Received\n{0}", JsonConvert.SerializeObject(test, Formatting.Indented));
        return NoContent();
    }
}
```

_That's it to get up and running!_

__Check out the EventHost and TestHost projects in our source for examples.__
