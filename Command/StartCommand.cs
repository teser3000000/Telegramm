public class StartCommand : IBotCommand
{
    public Task<string> Execute(string message, long chatId)
    {
        return Task.FromResult("Добро пожаловать! Используйте /help для списка команд.");
    }
}