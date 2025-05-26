using blogic.Components;
using blogic.Services;
using blogic.Data;
using System.Data;
using Blazored.LocalStorage;
using Microsoft.Data.Sqlite; // přidáš pro Sqlite


namespace blogic;

public class Program
{
    
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        Database.InitDb();

        builder.Services.AddScoped<ProductService>();
        builder.Services.AddScoped<CartService>();
        builder.Services.AddScoped<UserSessionService>();
        builder.Services.AddScoped<UserService>();
        
        builder.Services.AddBlazoredLocalStorage();

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
        
        app.UseStaticFiles();
        app.UseRouting();

        app.UseAntiforgery();

        app.MapStaticAssets();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}