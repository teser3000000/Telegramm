public interface IBotCommand
{
    Task<string> Execute(string message, long chatId);
}
