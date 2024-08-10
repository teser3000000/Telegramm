using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;


class Program
{
    private static ServiceProvider ConfigureServices()
    {
        var serviceCollection = new ServiceCollection();

        // Загрузка конфигурации
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        serviceCollection.AddSingleton<IConfiguration>(configuration);

        // Регистрация HttpClient
        serviceCollection.AddHttpClient();

        // Регистрация BotConfiguration
        serviceCollection.AddSingleton<BotConfiguration>();

        // Регистрация TelegramBotClient
        serviceCollection.AddSingleton<ITelegramBotClient>(provider =>
            new TelegramBotClient(provider.GetRequiredService<BotConfiguration>().TelegramToken));

        // Регистрация сервисов и команд
        serviceCollection.AddTransient<ICompanyInfoService, CompanyInfoService>();
        serviceCollection.AddTransient<ICommandFactory, CommandFactory>(); 
        serviceCollection.AddSingleton<ILastActionService, LastActionService>();

        // Регистрация команд
        serviceCollection.AddTransient<StartCommand>();
        serviceCollection.AddTransient<HelpCommand>();
        serviceCollection.AddTransient<HelloCommand>();
        serviceCollection.AddTransient<InnCommand>();
        serviceCollection.AddTransient<OkvedCommand>();
        serviceCollection.AddTransient<EgrulCommand>();
        serviceCollection.AddTransient<LastCommand>();

        // Регистрация бота
        serviceCollection.AddSingleton<MainBot>();

        return serviceCollection.BuildServiceProvider();
    }

    static void Main(string[] args)
    {
        Console.WriteLine("Программа запускается...");
        var serviceProvider = ConfigureServices();
        Console.WriteLine("Сервисы настроены.");
        var bot = serviceProvider.GetService<MainBot>();
        Console.WriteLine("Бот получен.");
        bot.Start();
        Console.WriteLine("Бот запущен.");
        Console.ReadLine();
        bot.Stop();
    }

}
