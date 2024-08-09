public class HelpCommand : IBotCommand
{
    public Task<string> Execute(string message, long chatId)
    {
        return Task.FromResult("Доступные команды:\n/start\n/help\n/hello\n/inn\n/okved\n/egrul\n/last");
    }
}