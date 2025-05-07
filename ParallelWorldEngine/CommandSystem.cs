namespace ParallelWorldEngine;

public interface ICommand
{
    Task ExecuteAsync();
}

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

public class PropertyChangeCommand<T> : ICommand
{
    private readonly object _target;
    private readonly string _propertyName;
    private readonly T _oldValue;
    private readonly T _newValue;
    private readonly Action<T> _setter;

    public PropertyChangeCommand(object target, string propertyName, T oldValue, T newValue, Action<T> setter)
    {
        _target = target;
        _propertyName = propertyName;
        _oldValue = oldValue;
        _newValue = newValue;
        _setter = setter;
    }

    public Task ExecuteAsync()
    {
        _setter(_newValue);
        return Task.CompletedTask;
    }
}

public class ReactiveProperty<T>
{
    private T _value;
    private readonly CommandSystem _commandSystem;
    private readonly object _owner;
    private readonly string _propertyName;

    public ReactiveProperty(T initialValue, object owner, string propertyName, CommandSystem commandSystem)
    {
        _value = initialValue;
        _owner = owner;
        _propertyName = propertyName;
        _commandSystem = commandSystem;
    }

    public T Value
    {
        get => _value;
        set
        {
            if (!EqualityComparer<T>.Default.Equals(_value, value))
            {
                var oldValue = _value;
                    
                if (_commandSystem != null)
                {
                    // 変更をコマンドキューに入れる
                    _commandSystem.EnqueueCommand(new PropertyChangeCommand<T>(
                        _owner, 
                        _propertyName, 
                        oldValue, 
                        value, 
                        v => _value = v));
                }
                else
                {
                    // コマンドシステムがない場合は直接変更
                    _value = value;
                }
            }
        }
    }

    // 暗黙的型変換演算子
    public static implicit operator T(ReactiveProperty<T> property) => property.Value;
}