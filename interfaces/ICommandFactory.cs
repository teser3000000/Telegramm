public interface ICommandFactory
{
    IBotCommand CreateCommand(string message);
}
