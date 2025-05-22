using blogic.Components;
using blogic.Services;
using blogic.Data;
using System.Data;
using Microsoft.Data.Sqlite; // přidáš pro Sqlite

namespace blogic;

public class Program
{
    
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Inicializuj databázi
        Database.InitDb();

        // Registrace služeb
        builder.Services.AddScoped<ProductService>();
        builder.Services.AddScoped<CartService>();
        builder.Services.AddScoped<UserSessionService>();
        builder.Services.AddScoped<UserService>();

        // DB připojení (nezapomeň složku + soubor!)
        builder.Services.AddScoped<IDbConnection>(sp =>
            new SqliteConnection("Data Source=Data/DutyFree.db"));

        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseAntiforgery();

        app.MapStaticAssets();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}