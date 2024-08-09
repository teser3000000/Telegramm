public class LastActionService : ILastActionService
{
    private readonly Dictionary<long, string> _lastActions = new Dictionary<long, string>();

    public void SetLastAction(long chatId, string action)
    {
        _lastActions[chatId] = action;
    }

    public string GetLastAction(long chatId)
    {
        return _lastActions.ContainsKey(chatId) ? _lastActions[chatId] : null;
    }
}