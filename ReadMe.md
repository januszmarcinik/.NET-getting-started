# .NET Core 3.0 Getting Started

## 1. Define first API endpoint
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

## 2. Define full REST Api for entity using endpoints
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

## 3. Define full REST Api for entity using ApiController

### Turn on Controllers support
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

### Create [Controller](Controllers/TeamMembersController.cs)

### Deserialization
> The default for ASP.NET Core is now System.Text.Json, which is new in .NET Core 3.0. Consider using System.Text.Json when possible. It's high-performance and doesn't require an additional library dependency. 
[See docs](https://docs.microsoft.com/pl-pl/aspnet/core/migration/22-to-30?view=aspnetcore-3.0&tabs=visual-studio#jsonnet-support)

### Switch to Newtonsoft.JSON
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

## 4. Define health check

### Turn on health checks
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

## 5. Logging

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

### Create logs - usage
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

## 6. Exception handler middleware

Once the logger is already defined, we could log each exception in the API, and as a response send back only an error message.

### Middleware registration
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
	private readonly RequestDelegate _request;
	private readonly ILogger _logger;

	public ExceptionHandlerMiddleware(RequestDelegate request, ILogger<ExceptionHandlerMiddleware> logger)
	{
		_request = request;
		_logger = logger;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		try
		{
			await _request(context);
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

## 7. Override default logger (Serilog)

### Add required packages to ``.csproj`` file
```XML
<PackageReference Include="Serilog" Version="2.9.0" />
<PackageReference Include="Serilog.AspNetCore" Version="3.1.0" />
<PackageReference Include="Serilog.Sinks.Console" Version="3.1.1" />
```

> Serilog completely replaces the logging implementation on .NET Core: it’s not just a provider that works side-by-side with the built-in logging, but rather, an alternative implementation of the .NET Core logging APIs.

That's why we can safely remvoe ``Logging`` configuration from ``appsettings.json``.

### Creating an instance of the logger
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
```

### Add HTTP Request logging
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


## 8. 

## 9.

## 10.


#### TODO:
- Autofac
- Swagger
- SignalR
- MVC
- Razor
- Blazor
- Authentication
- Authorization
- CORS
- C# 8