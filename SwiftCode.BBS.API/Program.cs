

using SwiftCode.BBS.API;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilde(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilde(string[] args)
    {
        return  Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });

    }
}