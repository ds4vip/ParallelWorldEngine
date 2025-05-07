namespace ParallelWorldEngine;

public class CommandSystem
{
    private readonly List<ICommand> _currentFrameCommands = new();
    private readonly object _commandLock = new();

    public void EnqueueCommand(ICommand command)
    {
        lock (_commandLock)
        {
            _currentFrameCommands.Add(command);
        }
    }

    public async Task ExecuteQueuedCommandsAsync()
    {
        List<ICommand> commands;
            
        lock (_commandLock)
        {
            commands = new List<ICommand>(_currentFrameCommands);
            _currentFrameCommands.Clear();
        }
            
        foreach (var command in commands)
        {
            await command.ExecuteAsync();
        }
    }
}
