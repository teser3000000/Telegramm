public interface ILastActionService
{
    void SetLastAction(long chatId, string action);
    string GetLastAction(long chatId);
}