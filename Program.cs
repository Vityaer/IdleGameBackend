using IdleGameServer.Contexts;
using IdleGameServer.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder();
string connection = builder.Configuration.GetConnectionString("DefaultConnection");

// добавляем контекст ApplicationContext в качестве сервиса в приложение
builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connection));
builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/players", async (ApplicationContext db) => await db.Users.ToListAsync());

app.MapGet("/api/players/{id:int}", async (int id, ApplicationContext db) =>
{
    // получаем пользователя по id
    User? player = await db.Users.FirstOrDefaultAsync(u => u.Id == id);

    // если не найден, отправляем статусный код и сообщение об ошибке
    if (player == null) return Results.NotFound(new { message = "Пользователь не найден" });

    // если пользователь найден, отправляем его
    return Results.Json(player);
});

app.MapDelete("/api/players/{id:int}", async (int id, ApplicationContext db) =>
{
    // получаем пользователя по id
    User? user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);

    // если не найден, отправляем статусный код и сообщение об ошибке
    if (user == null) return Results.NotFound(new { message = "Пользователь не найден" });

    // если пользователь найден, удаляем его
    db.Users.Remove(user);
    await db.SaveChangesAsync();
    return Results.Json(user);
});

app.MapPost("/api/players", async (User player, ApplicationContext db) =>
{
    // добавляем пользователя в массив
    await db.Users.AddAsync(player);
    await db.SaveChangesAsync();
    return player.Id;
});


app.MapPut("/api/players", async (User playerData, ApplicationContext db) =>
{
    // получаем пользователя по id
    var player = await db.Users.FirstOrDefaultAsync(u => u.Id == playerData.Id);

    // если не найден, отправляем статусный код и сообщение об ошибке
    if (player == null) return Results.NotFound(new { message = "Пользователь не найден" });

    // если пользователь найден, изменяем его данные и отправляем обратно клиенту
    player.Name = playerData.Name;
    await db.SaveChangesAsync();
    return Results.Json(player);
});

app.Run();


