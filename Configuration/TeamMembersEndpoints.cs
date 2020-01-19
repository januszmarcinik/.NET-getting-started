using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NETCore3.Entities;
using NETCore3.Extensions;
using NETCore3.Services;

namespace NETCore3.Configuration
{
    internal static class TeamMembersEndpoints
    {
        private const string UrlPrefix = "/api/team-members";

        public static IApplicationBuilder UseTeamMembersEndpoints(this IApplicationBuilder app)
        {
            return app
                .UseEndpoints(endpoints =>
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
                        if (teamMember == null)
                        {
                            await context.WriteNotFound($"Team member with given id '{id}' does not exist.");
                            return;
                        }

                        await context.WriteOk(teamMember);
                    });

                    endpoints.MapPost(UrlPrefix, async context =>
                    {
                        var command = await context.GetObjectFromBody<TeamMember>();
                        var id = service.Add(command);

                        await context.WriteOk($"Successfully created team member with id '{id}'.");
                    });

                    endpoints.MapPut(UrlPrefix, async context =>
                    {
                        var command = await context.GetObjectFromBody<TeamMember>();
                        service.Update(command);

                        await context.WriteOk("Successfully updated team member.");
                    });

                    endpoints.MapDelete($"{UrlPrefix}/{{id}}", async context =>
                    {
                        var id = GetIdFromRoute(context.Request.RouteValues["id"]);
                        var teamMember = service.GetById(id);
                        if (teamMember == null)
                        {
                            await context.WriteNotFound($"Team member with given id '{id}' does not exist.");
                            return;
                        }
                        
                        service.Remove(teamMember);
                        await context.WriteOk("Successfully deleted team member.");
                    });
                });
        }

        private static Guid GetIdFromRoute(object routeValue) => 
            Guid.Parse(routeValue?.ToString() ?? Guid.Empty.ToString());
    }
}