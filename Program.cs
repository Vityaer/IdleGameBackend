using Microsoft.AspNetCore.Diagnostics;
using Misc.Json;
using Misc.Json.Impl;
using System.Net;
using UniverseRift.Contexts;
using UniverseRift.Controllers.Buildings.Achievments;
using UniverseRift.Controllers.Buildings.Arenas;
using UniverseRift.Controllers.Buildings.Battlepases;
using UniverseRift.Controllers.Buildings.Campaigns;
using UniverseRift.Controllers.Buildings.ChallengeTowers;
using UniverseRift.Controllers.Buildings.DailyRewards;
using UniverseRift.Controllers.Buildings.FortuneWheels;
using UniverseRift.Controllers.Buildings.GameCycles;
using UniverseRift.Controllers.Buildings.Guilds;
using UniverseRift.Controllers.Buildings.Industries;
using UniverseRift.Controllers.Buildings.Industries.Mines;
using UniverseRift.Controllers.Buildings.Shops;
using UniverseRift.Controllers.Buildings.TaskBoards;
using UniverseRift.Controllers.Buildings.TimeMenagers;
using UniverseRift.Controllers.Buildings.TravelCircles;
using UniverseRift.Controllers.Buildings.Tutorials;
using UniverseRift.Controllers.Buildings.Voyages;
using UniverseRift.Controllers.Common;
using UniverseRift.Controllers.Jsons;
using UniverseRift.Controllers.Players;
using UniverseRift.Controllers.Players.Heroes;
using UniverseRift.Controllers.Players.Inventories;
using UniverseRift.Controllers.Players.Inventories.Items;
using UniverseRift.Controllers.Players.Inventories.Splinters;
using UniverseRift.Controllers.Server;
using UniverseRift.Services;
using UniverseRift.Services.Resources;
using UniverseRift.Services.Rewarders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AplicationContext>(ServiceLifetime.Singleton);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddSingleton<IServerController, ServerController>();
builder.Services.AddSingleton<IResourceManager, ResourcesManager>();
builder.Services.AddSingleton<IJsonConverter, JsonConverter>();
builder.Services.AddSingleton<IJsonDataController, JsonDataController>();
builder.Services.AddSingleton<ICommonDictionaries, CommonDictionaries>();
builder.Services.AddSingleton<IRewardService, RewardManager>();
builder.Services.AddSingleton<ICampaignController, CampaignController>();
builder.Services.AddSingleton<IPlayersController, PlayersController>();
builder.Services.AddSingleton<IHeroesController, HeroesController>();
builder.Services.AddSingleton<IInventoriesController, InventoriesController>();
builder.Services.AddSingleton<IItemsController, ItemsController>();
builder.Services.AddSingleton<IChallengeTowerController, ChallengeTowerController>();
builder.Services.AddSingleton<ITaskBoardController, TaskBoardController>();
builder.Services.AddSingleton<IMarketController, MarketController>();
builder.Services.AddSingleton<IMineController, MineController>();
builder.Services.AddSingleton<IDailyRewardController, DailyRewardController>();
builder.Services.AddSingleton<IIndustryController, IndustryController>();
builder.Services.AddSingleton<IGameCycleController, GameCycleController>();
builder.Services.AddSingleton<IAchievmentController, AchievmentController>();
builder.Services.AddSingleton<IVoyageController, VoyageController>();
builder.Services.AddSingleton<IArenaController, ArenaController>();
builder.Services.AddSingleton<ITravelCircleController, TravelCircleController>();
builder.Services.AddSingleton<ITutorialController, TutorialController>();
builder.Services.AddSingleton<ITimeManagerController, TimeManagerController>();
builder.Services.AddSingleton<IGuildController, GuildController>();
builder.Services.AddSingleton<IFortuneWheelController, FortuneWheelController>();
builder.Services.AddSingleton<ISplinterController, SplinterController>();
builder.Services.AddSingleton<IBattlepasController, BattlepasController>();

builder.Services.AddHostedService<MyHostedService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();
app.UseDefaultFiles();
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(
        options =>
        {
            options.Run(
                async context =>
                {
                    var directoryPath = "Logs";
                    var fileName = "UniverseRift_error.txt";
                    var writer = TextUtils.GetFileWriterStream(directoryPath, fileName, true);
                    var exceptionObject = context.Features.Get<IExceptionHandlerFeature>();
                    var Message = "Unknown";
                    var StackTrace = "Unknown";

                    if (null != exceptionObject)
                    {
                        Message = exceptionObject.Error.Message;
                        StackTrace = exceptionObject.Error.StackTrace;
                    }
                    writer.WriteLine($"{DateTime.UtcNow.ToString("MM.dd.yyyy HH:mm:ss.ff")} [{HttpStatusCode.InternalServerError.ToString().ToUpper()}]\nMessage: {Message}\nStackTrace: {StackTrace}\n");
                    writer.Close();
                    writer.Dispose();
                });
        }
    );

app.Run();
