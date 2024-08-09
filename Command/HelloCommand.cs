public class HelloCommand : IBotCommand
{
    public Task<string> Execute(string message, long chatId)
    {
        return Task.FromResult("Имя: Ермоленко Никита\nEmail: ermolenko.nikita@mail.ru\nGitHub: https://github.com/teser3000000/Telegramm");
    }
}