using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;

public class MainBot
{
    private readonly ITelegramBotClient _botClient;
    private readonly ICommandFactory _commandFactory;
    private readonly ILastActionService _lastActionService;

    public MainBot(ITelegramBotClient botClient, ICommandFactory commandFactory, ILastActionService lastActionService)
    {
        _botClient = botClient;
        _commandFactory = commandFactory;
        _lastActionService = lastActionService;
    }

    public void Start()
    {
        var cancellationToken = new CancellationTokenSource().Token;

        _botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            pollingErrorHandler: HandleErrorAsync,
            cancellationToken: cancellationToken
        );
    }

    public void Stop()
    {
        // если требуется поддержка остановки
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Message is not { } message)
            return;

        if (message.Text != null)
        {
            try
            {
                var command = _commandFactory.CreateCommand(message.Text);
                string response;

                if (command != null)
                {
                    // Сохраняем последнюю команду, если она не /last
                    if (message.Text != "/last")
                    {
                        _lastActionService.SetLastAction(message.Chat.Id, message.Text);
                    }

                    response = await command.Execute(message.Text, message.Chat.Id);
                }
                else
                {
                    // Если команда не распознана и нет сохраненной команды
                    response = "Неизвестная команда. Воспользуйтесь /help для получения списка команд.";
                }

                if (!string.IsNullOrWhiteSpace(response))
                {
                    await _botClient.SendTextMessageAsync(message.Chat.Id, response, cancellationToken: cancellationToken);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
                await _botClient.SendTextMessageAsync(message.Chat.Id, "Произошла ошибка. Пожалуйста, попробуйте позже.", cancellationToken: cancellationToken);
            }
        }
    }

    private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(errorMessage);
        return Task.CompletedTask;
    }


}
