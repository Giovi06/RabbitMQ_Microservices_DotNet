using AuthService.Endpoints;
using AuthService.Helpers;
using JwtLibrary;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection(AppSettings.SectionName))
    .PostConfigure<AppSettings>(appsettings =>
    {
        if (string.IsNullOrEmpty(appsettings.ApiUrl))
        {
            throw new Exception($"Configuration missing: '{AppSettings.SectionName}:{nameof(appsettings.ApiUrl)}' (URL for remote authentication API)");
        }
        if (!appsettings.ApiUrl.StartsWith("https") && !appsettings.ApiUrl.StartsWith("http"))
        {
            throw new Exception($"Configuration invalid: '{AppSettings.SectionName}:{nameof(appsettings.ApiUrl)}' (URL must start with http or https)");
        }
        if (!appsettings.ApiUrl.EndsWith("/"))
        {
            appsettings.ApiUrl += "/";
        }
    })
    .AddOptionsWithValidateOnStart<AppSettings>(); ;

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName))
    .PostConfigure<JwtOptions>(options =>
    {
        if (string.IsNullOrEmpty(options.Secret))
        {
            throw new Exception($"Configuration missing: '{JwtOptions.SectionName}:{nameof(options.Secret)}' (JWT secret)");
        }
    }).
    AddOptionsWithValidateOnStart<JwtOptions>();

// use singleton HTTP client -- see https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient-guidelines
builder.Services.AddSingleton<HttpClient>();

builder.Services.AddSingleton<RemoteAuthApi>();
builder.Services.AddJwtHelpers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.MapAuthEndpoints();

app.Run();
