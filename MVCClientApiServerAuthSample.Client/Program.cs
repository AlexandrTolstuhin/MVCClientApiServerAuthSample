using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

using MVCClientApiServerAuthSample.Client.Services;

using Refit;

var builder = WebApplication.CreateBuilder(args);

builder.Services
       .AddRefitClient<IApiClient>()
       .ConfigureHttpClient(c => c.BaseAddress = new Uri(builder.Configuration["API:Uri"]!));

builder.Services.AddAuthorization();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
       .AddCookie(options => { options.LoginPath = "/loginRequired"; });

var app = builder.Build();

app.MapGet("/", () => "Hello!");

app.MapGet("/loginRequired", () => "To authorize, follow the link /login/name. Where instead of name, enter your name");

app.MapGet("/login/{name:required}",
           async (string name, IApiClient client, HttpContext context) =>
           {
               var token = await client.GetTokenAsync(name);

               ClaimsIdentity claimsIdentity = new(new Claim[]
                                                   {
                                                       new("Token", $"Bearer {token}")
                                                   },
                                                   CookieAuthenticationDefaults.AuthenticationScheme);

               await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                         new ClaimsPrincipal(claimsIdentity));

               context.Response.Redirect("/");
           });

app.MapGet("/logout",
           async context =>
           {
               await context.SignOutAsync();

               context.Response.Redirect("/");
           })
   .RequireAuthorization();

app.MapGet("/secret",
           async (IApiClient client, HttpContext context) =>
           {
               var token = context.User.FindFirstValue("Token")!;

               return await client.GetSecretAsync(token);
           })
   .RequireAuthorization();

app.Run();