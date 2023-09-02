using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.IdentityModel.Tokens;

using MVCClientApiServerAuthSample.Server.Models;
using MVCClientApiServerAuthSample.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// TODO: Move to extensions! In this case, it is simplified for the sake of example

builder.Services.AddSingleton<TokenGenerator>();

builder.Services.AddAuthorization();

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
       .AddJwtBearer(options =>
                     {
                         var jwtOptions = builder.Configuration
                                                 .GetRequiredSection(JwtOptions.SectionName)
                                                 .Get<JwtOptions>()!;

                         options.TokenValidationParameters = new TokenValidationParameters
                                                             {
                                                                 ValidateIssuer = true,
                                                                 ValidateAudience = true,
                                                                 ValidateLifetime = jwtOptions.IsValidateLifetime,
                                                                 ValidateIssuerSigningKey = true,
                                                                 ValidIssuer = jwtOptions.Issuer,
                                                                 ValidAudience = jwtOptions.Audience,
                                                                 IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecurityKey))
                                                             };
                     });

builder.Services.AddHttpLogging(options => options.LoggingFields = HttpLoggingFields.All);

var app = builder.Build();

app.UseHttpLogging();

app.MapGet("/getToken/{name:required}", (string name, TokenGenerator generator) => generator.GetAccessToken(name));

app.MapGet("/secret", () => "super secret information")
   .RequireAuthorization();

app.Run();