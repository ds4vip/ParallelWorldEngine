namespace ParallelWorldEngine;

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
