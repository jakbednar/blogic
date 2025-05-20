using blogic.Components;
using blogic.Services;
using blogic.Data; // přidej i tento using pro Database

namespace blogic;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        //  Inicializuj databázi
        Database.InitDb(); // ← TADY to přidej

        builder.Services.AddScoped<ProductService>();
        builder.Services.AddScoped<CartService>();

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