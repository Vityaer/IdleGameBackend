using Microsoft.EntityFrameworkCore;
using Misc.Json;
using Misc.Json.Impl;
using UniverseRift.Contexts;
using UniverseRift.Controllers.Buildings.Campaigns;
using UniverseRift.Controllers.Common;
using UniverseRift.Controllers.Jsons;
using UniverseRift.Controllers.Players;
using UniverseRift.Controllers.Players.Inventories.Resources;
using UniverseRift.Controllers.Server;
using UniverseRift.Controllers.Services.Rewarders;

var builder = WebApplication.CreateBuilder(args);

string connection = builder.Configuration.GetConnectionString("DefaultConnection");

// добавляем контекст ApplicationContext в качестве сервиса в приложение
builder.Services.AddDbContext<AplicationContext>(options => options.UseSqlServer(connection), ServiceLifetime.Singleton);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddSingleton<IServerController, ServerController>();
builder.Services.AddSingleton<IResourceController, ResourcesController>();
builder.Services.AddSingleton<IJsonConverter, JsonConverter>();
builder.Services.AddSingleton<IJsonDataController, JsonDataController>();
builder.Services.AddSingleton<ICommonDictionaries, CommonDictionaries>();
builder.Services.AddSingleton<IClientRewardService, ClientRewardService>();
builder.Services.AddSingleton<ICampaignController, CampaignController>();
builder.Services.AddSingleton<IPlayersController, PlayersController>();

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
