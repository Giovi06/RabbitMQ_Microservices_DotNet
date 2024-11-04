using JwtLibrary;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MqLibrary;
using OrderService;
using OrderService.Endpoints;
using OrderService.Model;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddDbContext<OrderDb>(opt => opt.UseSqlite(builder.Configuration.GetConnectionString("OrderDb")));
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
builder.Services.AddJwtHelpers();
builder.Services.AddScoped<IJwtSubjectLookup<Client?>, JwtClientLookup>();
builder.Services
    .AddMqLibraryConfig(builder.Configuration)
    .AddMqLibrary();



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// see https://www.freecodespot.com/blog/use-jwt-bearer-authorization-in-swagger/
builder.Services.AddSwaggerGen(o =>
{
    o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
    });
    o.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();


app.UseJwtMiddleware<Client?>();
app.MapOrderEndpoints();

app.Run();
