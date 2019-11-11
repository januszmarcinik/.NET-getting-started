## .NET Core 3.0 Getting Started

#### 1. Define first API endpoint
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

#### 2. Define full REST Api for entity using endpoints
``` C#
app.UseEndpoints(endpoints =>
{
	var service = endpoints.ServiceProvider.GetService<ITeamMembersService>();

	endpoints.MapGet(UrlPrefix, async context =>
	{
		var teamMembers = service.GetAll();

		await context.WriteOk(teamMembers);
	});

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

	endpoints.MapPut(UrlPrefix, async context =>
	{
		var command = await context.GetObjectFromBody<TeamMember>();
		service.Update(command);

		await context.WriteAccepted("Successfully updated team member.");
	});

	endpoints.MapDelete($"{UrlPrefix}/{{id}}", async context =>
	{
		var id = GetIdFromRoute(context.Request.RouteValues["id"]);
		service.Remove(id);

		await context.WriteAccepted("Successfully deleted team member.");
	});
});
```

#### TODO:
- Autofac
- Swagger
- SignalR
- Controllers
- MVC
- Razor
- Blazor
- Authentication
- Authorization
- CORS
- C# 8