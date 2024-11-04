using ArticleService.Endpoints;
using ArticleService.Model;
using Microsoft.EntityFrameworkCore;
using MqLibrary;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ArticleDb>(opt => opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultContext")));
builder.Services
    .AddMqLibraryConfig(builder.Configuration);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapArticleEndpoints();

app.Run();
