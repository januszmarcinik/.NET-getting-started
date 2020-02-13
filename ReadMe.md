# .NET Core 3.0 Getting Started

# 1. Define first API endpoint
``` C#
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
	app.UseRouting()
		.UseEndpoints(endpoints =>
		{
			endpoints.MapGet("/api", async context =>
			{
				await context.Response.WriteAsync($"API is running on {env.EnvironmentName} environment");
			});
		});
}
```

# 2. Define full REST Api for entity using endpoints
``` C#
app.UseEndpoints(endpoints =>
{
	var service = endpoints.ServiceProvider.GetService<ITeamMembersService>();

	// ...
	endpoints.MapGet($"{UrlPrefix}/{{id}}", async context =>
	{
		var id = GetIdFromRoute(context.Request.RouteValues["id"]);
		var teamMember = service.GetById(id);

		await context.WriteOk(teamMember);
	});

	endpoints.MapPost(UrlPrefix, async context =>
	{
		var command = await context.GetObjectFromBody<TeamMember>();
		var id = service.Add(command);

		await context.WriteAccepted($"Successfully created team member with id '{id}'.");
	});

	// ...
});
```
[Full REST Api endpoints configuration](Configuration/TeamMembersEndpoints.cs)

# 3. Define full REST Api for entity using ApiController

## Turn on Controllers support
``` C#
public void ConfigureServices(IServiceCollection services)
{
	services.AddControllers();
}

public void Configure(IApplicationBuilder app)
{
	app.UseRouting()
		.UseEndpoints(endpoints =>
		{
			endpoints.MapControllers();
		});
}
```

``` C#
[Route("api/team-members")]
public class TeamMembersController : ControllerBase
{
    private readonly ITeamMembersService _service;

    public TeamMembersController(ITeamMembersService service)
    {
        _service = service;
    }

    [HttpGet("{id}")]
    public IActionResult GetById(Guid id)
    {
        var teamMember = _service.GetById(id);
        if (teamMember == null)
        {
            return NotFound($"Team member with given id '{id}' does not exist.");
        }

        return Ok(teamMember);
    }
}
```

## Create [Controller](Controllers/TeamMembersController.cs)

## Deserialization
> The default for ASP.NET Core is now System.Text.Json, which is new in .NET Core 3.0. Consider using System.Text.Json when possible. It's high-performance and doesn't require an additional library dependency. 
[See docs](https://docs.microsoft.com/pl-pl/aspnet/core/migration/22-to-30?view=aspnetcore-3.0&tabs=visual-studio#jsonnet-support)

## Switch to Newtonsoft.JSON
To use Json.NET in an ASP.NET Core 3.0 project:
- Add a package reference to ``Microsoft.AspNetCore.Mvc.NewtonsoftJson``.
``` XML
<PackageReference Include="Microsoft.AspNetCore.Mvc.NewtonsoftJson" Version="3.0.0" />
```
- Update ``Startup.ConfigureServices`` to call ``AddNewtonsoftJson``.
``` C#
public void ConfigureServices(IServiceCollection services)
{
	services.AddControllers()
		.AddNewtonsoftJson(options =>
			options.SerializerSettings.ContractResolver =
				new CamelCasePropertyNamesContractResolver());
}
```

# 4. Define health check

## Turn on health checks
``` C#
public void ConfigureServices(IServiceCollection services)
{
	services.AddHealthChecks();
}

public void Configure(IApplicationBuilder app)
{
	app.UseRouting()
		.UseEndpoints(endpoints =>
		{
			endpoints.MapHealthChecks("/health");
		});
}
```

# 5. Logging

>.NET Core supports a logging API that works with a variety of built-in and third-party logging providers.  

There is an abstraction for it: ``ILogger<TCategoryName>``

The default ASP.NET Core project templates call CreateDefaultBuilder, which adds the following logging providers:  
- Console
- Debug
- EventSource
- EventLog (only when running on Windows)

You can replace the default providers with your own choices. Call ``ClearProviders``, and add the providers you want.
```C#
public static IHostBuilder CreateHostBuilder(string[] args) =>
	Host.CreateDefaultBuilder(args)
		.ConfigureLogging(logging =>
		{
			logging.ClearProviders();
			logging.AddConsole();
		})
		// ...
```

## Create logs - usage
```C#
[Route("api/team-members")]
public class TeamMembersController : ControllerBase
{
	private readonly ITeamMembersService _service;
	private readonly ILogger _logger;

	public TeamMembersController(ITeamMembersService service, ILogger<TeamMembersController> logger)
	{
		_service = service;
		_logger = logger;
	}

	// ...

	[HttpPost]
	public IActionResult Post([FromBody] TeamMember teamMember)
	{
		var id = _service.Add(teamMember);
		return Accepted($"Successfully created team member with id '{id}'.");
	}

	public override AcceptedResult Accepted(string message)
	{
		_logger.LogDebug(message);
		return Accepted(value: message);
	}
}
```

# 6. Middleware

Middleware is software that's assembled into an app pipeline to handle requests and responses.

The .NET Core middleware pipeline can be configured using the following methods from `IApplicationBuilder`:

## `Use()`
Adds a middleware to the pipeline. The component’s code must decide whether to terminate or continue the pipeline. We can add as many Use methods as we want. They will be executed in the order in which they were added to the pipeline. 
``` C#
app.Use(async (context, next) =>
{
	logger.LogInformation("Executing middleware...");
	context.Request.Headers.Add("correlation-id", "7a902997-bcc8-4162-aba8-fffa93d6bfad");

	await next.Invoke();

	logger.LogInformation("Middleware executed.");
});
```

## `UseWhen()`
Extends `Use()` configuration about condition specified in the predicate.  Conditionally creates a branch in the pipeline that is rejoined to the main pipeline (unlike with `MapWhen()`).
``` C#
app.UseWhen(context => context.Request.Path.Value.Contains("team-members"), appBuilder =>
{
	appBuilder.Use(async (context, next) =>
	{
		logger.LogInformation("Executing middleware...");
		context.Request.Headers.Add("correlation-id", "7a902997-bcc8-4162-aba8-fffa93d6bfad");
		
		await next.Invoke();

		logger.LogInformation("Middleware executed.");
	});
});
```

## `Map()`
Branches to appropriate middleware components, based on the incoming request's URL path.
``` C#
app.Map("/api/branch", appBuilder =>
{
	appBuilder.Use(async (context, next) =>
	{
		logger.LogInformation("Executing middleware for route '{ApiRoute}'", "api/branch");

		await next.Invoke();
	});
	
	appBuilder
		.UseRouting()
		.UseEndpoints(endpoints =>
		{
			endpoints.MapControllers();
		});
});
```

## `MapWhen()`
Extends `Map()` configuration about condition specified in the predicate.
``` C#
app.MapWhen(context => context.Request.Path.Value.Contains("team-members"), appBuilder =>
{
	appBuilder.Use(async (context, next) =>
	{
		logger.LogInformation("Executing middleware for route '{ApiRoute}'", "team-members");

		await next.Invoke();
	});
});
```

## `Run()`
These delegates don't receive a next parameter. The first `Run` delegate terminates the pipeline. Any middleware components added after Run will not be processed.
``` C#
app.MapWhen(context => context.Request.Path.Value.Contains("team-members"), appBuilder =>
{
	appBuilder.Run(async context =>
	{
		logger.LogError("Endpoint 'team-members' is not allowed while using branched middleware pipeline.");
		await context.Response.WriteAsync("End of the request.");
	});
});
```

## Exception handler middleware
```C#
public void Configure(IApplicationBuilder app)
{
	app.UseMiddleware<ExceptionHandlerMiddleware>();
}
```

[ExceptionHandlerMiddleware](Middleware/ExceptionHandlerMiddleware.cs)
```C#
internal class ExceptionHandlerMiddleware
{
	private readonly RequestDelegate _next;
	private readonly ILogger _logger;

	public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
	{
		_next = next;
		_logger = logger;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		_logger.LogInformation("ExceptionHandlerMiddleware invoked.");
		
		try
		{
			await _next(context);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, ex.Message); 
			context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
			await context.Response.WriteAsync(ex.Message);
		}
	}
}
```

# 7. Override default logger (Serilog)

## Add required packages to ``.csproj`` file
```XML
<PackageReference Include="Serilog" Version="2.9.0" />
<PackageReference Include="Serilog.AspNetCore" Version="3.1.0" />
<PackageReference Include="Serilog.Sinks.Console" Version="3.1.1" />
```

> Serilog completely replaces the logging implementation on .NET Core: it’s not just a provider that works side-by-side with the built-in logging, but rather, an alternative implementation of the .NET Core logging APIs.

That's why we can safely remove ``Logging`` configuration from ``appsettings.json``.

## Creating an instance of the logger
```c#
public static void Main(string[] args)
{
	Log.Logger = new LoggerConfiguration()
		.Enrich.FromLogContext()
		.WriteTo.Console()
		.CreateLogger();

	try
	{
		Log.Information("Starting up");
		CreateHostBuilder(args).Build().Run();
	}
	catch (Exception ex)
	{
		Log.Fatal(ex, "Host builder error");
	}
	finally
	{
		Log.CloseAndFlush();
	}
}

public static IHostBuilder CreateHostBuilder(string[] args) =>
	Host.CreateDefaultBuilder(args)
		.UseSerilog()
		.ConfigureLogging(logging =>
		{
			logging.ClearProviders();
			logging.AddConsole();
		});
```

## Add HTTP Request logging
```C#
public void Configure(IApplicationBuilder app)
{
	app.UseSerilogRequestLogging();
}
```
Example:
```
[14:54:49 INF] HTTP POST /api/team-members responded 202 in 468.0278 ms
```

# 8. Attach **Seq** as tool for storing structured logs

## Pull docker image with **Seq**
>docker pull datalust/seq

## Run Seq instance
### Empheral storage 
>docker run -e ACCEPT_EULA=Y -p 5341:80 datalust/seq:latest
### Stable storage
>docker run -e ACCEPT_EULA=Y -v C:/Seq/data:/data -p 5341:80 datalust/seq:latest

## Add required packages to ``.csproj`` file
```XML
<PackageReference Include="Serilog.Sinks.Seq" Version="4.0.0" />
```

## Add Seq sink to Serilog
``` C#
Log.Logger = new LoggerConfiguration()
	.Enrich.FromLogContext()
	.WriteTo.Console()
	.WriteTo.Seq("http://localhost:5341") // default Seq port
	.CreateLogger();
```

# 9. Replace default dependency injection container to Autofac

## Reference nuget packages
```XML
<PackageReference Include="Autofac" Version="4.9.4" />
<PackageReference Include="Autofac.Extensions.DependencyInjection" Version="5.0.1" />
```

## Override the factory used to create the service provider
```C#
public static IHostBuilder CreateHostBuilder(string[] args) =>
	Host.CreateDefaultBuilder(args)
		.UseServiceProviderFactory(new AutofacServiceProviderFactory())
		// ...
```



## Move services registration to additional ``Startup`` method called ``ConfigureContainer``
```C#
public void ConfigureContainer(ContainerBuilder builder)
{
	builder
		.Register(factory =>
		{
			var initTeamMembers = new[]
			{
				new TeamMember(Guid.NewGuid(), "John", Role.DotNet, 5),
				new TeamMember(Guid.NewGuid(), "Franc", Role.DotNet, 6),
				new TeamMember(Guid.NewGuid(), "Robert", Role.JavaScript, 2),
				new TeamMember(Guid.NewGuid(), "Alex", Role.DevOps, 5)
			};
			return new TeamMembersService(initTeamMembers);
		})
		.As<ITeamMembersService>()
		.SingleInstance();
}
```
> ConfigureContainer is where you can register things directly with Autofac.  
This runs after ConfigureServices so the things here will override registrations made in ConfigureServices.  
Don't build the container - that gets done for you by the factory.

> **HINT:** If, for some reason, you need a reference to the built container, you can use the convenience extension method ``GetAutofacRoot``.
```C#
public ILifetimeScope AutofacContainer { get; private set; }

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
	
	AutofacContainer = app.ApplicationServices.GetAutofacRoot();
}
```


# 9.

# 10.


### TODO:
- Swagger
- SignalR
- MVC
- Razor
- Blazor
- Authentication
- Authorization
- CORS
- C# 8
- Multiple web apps in one process