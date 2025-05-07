namespace ParallelWorldEngine;

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