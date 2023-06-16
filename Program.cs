using IdleGameServer.Contexts;
using IdleGameServer.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder();
string connection = builder.Configuration.GetConnectionString("DefaultConnection");

// ��������� �������� ApplicationContext � �������� ������� � ����������
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
    // �������� ������������ �� id
    User? player = await db.Users.FirstOrDefaultAsync(u => u.Id == id);

    // ���� �� ������, ���������� ��������� ��� � ��������� �� ������
    if (player == null) return Results.NotFound(new { message = "������������ �� ������" });

    // ���� ������������ ������, ���������� ���
    return Results.Json(player);
});

app.MapDelete("/api/players/{id:int}", async (int id, ApplicationContext db) =>
{
    // �������� ������������ �� id
    User? user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);

    // ���� �� ������, ���������� ��������� ��� � ��������� �� ������
    if (user == null) return Results.NotFound(new { message = "������������ �� ������" });

    // ���� ������������ ������, ������� ���
    db.Users.Remove(user);
    await db.SaveChangesAsync();
    return Results.Json(user);
});

app.MapPost("/api/players", async (User player, ApplicationContext db) =>
{
    // ��������� ������������ � ������
    await db.Users.AddAsync(player);
    await db.SaveChangesAsync();
    return player.Id;
});


app.MapPut("/api/players", async (User playerData, ApplicationContext db) =>
{
    // �������� ������������ �� id
    var player = await db.Users.FirstOrDefaultAsync(u => u.Id == playerData.Id);

    // ���� �� ������, ���������� ��������� ��� � ��������� �� ������
    if (player == null) return Results.NotFound(new { message = "������������ �� ������" });

    // ���� ������������ ������, �������� ��� ������ � ���������� ������� �������
    player.Name = playerData.Name;
    await db.SaveChangesAsync();
    return Results.Json(player);
});

app.Run();


