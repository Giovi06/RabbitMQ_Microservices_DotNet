using ArticleService.Endpoints;
using ArticleService.Model;
using Microsoft.EntityFrameworkCore;
using MqLibrary;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ArticleDb>(opt => opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultContext")));
builder.Services
    .AddMqLibraryConfig(builder.Configuration)
    .AddMqLibrary();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
    app.UseSwagger();
    app.UseSwaggerUI();

app.MapCategoryEndpoints();
app.MapArticleEndpoints();

app.Run();
