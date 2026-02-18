using Microsoft.AspNetCore.Hosting;
using Sample.Data;

namespace Sample.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
        
        var host = builder.Build();

        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<AppDbContext>();
                DbInitializer.SeedAsync(context).Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        host.Run();
    }
}