using Microsoft.EntityFrameworkCore;
using Misc.Json;
using Misc.Json.Impl;
using UniverseRift.Contexts;
using UniverseRift.Controllers.Players.Inventories.Resources;
using UniverseRift.Controllers.Server;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IServerController, ServerController>();
builder.Services.AddSingleton<IResourceController, ResourcesController>();
builder.Services.AddSingleton<IJsonConverter, JsonConverter>();

string connection = builder.Configuration.GetConnectionString("DefaultConnection");

// добавляем контекст ApplicationContext в качестве сервиса в приложение
builder.Services.AddDbContext<AplicationContext>(options => options.UseSqlServer(connection));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();

app.MapControllers();
app.UseDefaultFiles();
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
