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




#### TODO:
- Autofac
- Swagger
- SignalR
- MVC
- Health check
- Razor
- Blazor
- Authentication
- Authorization
- CORS
- C# 8