public class LastCommand : IBotCommand
{
    private readonly ILastActionService _lastActionService;
    private readonly ICommandFactory _commandFactory;

    public LastCommand(ILastActionService lastActionService, ICommandFactory commandFactory)
    {
        _lastActionService = lastActionService;
        _commandFactory = commandFactory;
    }

    public async Task<string> Execute(string message, long chatId)
    {
        var lastCommandText = _lastActionService.GetLastAction(chatId);

        if (string.IsNullOrWhiteSpace(lastCommandText))
        {
            return "Нет сохраненной последней команды.";
        }

        var lastCommand = _commandFactory.CreateCommand(lastCommandText);

        if (lastCommand == null)
        {
            return "Не удалось повторить последнюю команду.";
        }

        try
        {
            return await lastCommand.Execute(lastCommandText, chatId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при выполнении команды /last: {ex.Message}");
            return "Произошла ошибка при повторном выполнении команды. Пожалуйста, попробуйте позже.";
        }
    }
}
