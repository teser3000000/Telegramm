using Microsoft.Extensions.DependencyInjection;

public class CommandFactory : ICommandFactory
{
    private readonly IServiceProvider _serviceProvider;

    public CommandFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IBotCommand CreateCommand(string message)
    {
        return message.Split(' ')[0] switch
        {
            "/start" => _serviceProvider.GetService<StartCommand>(),
            "/help" => _serviceProvider.GetService<HelpCommand>(),
            "/hello" => _serviceProvider.GetService<HelloCommand>(),
            "/inn" => _serviceProvider.GetService<InnCommand>(),
            "/okved" => _serviceProvider.GetService<OkvedCommand>(),
            "/egrul" => _serviceProvider.GetService<EgrulCommand>(),
            "/last" => _serviceProvider.GetService<LastCommand>(),
            _ => null,
        };
    }
}
