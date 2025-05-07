namespace SpaceInvaders;

public enum Key
{
    Left,
    Right,
    Space
}

public static class InputManager
{
    private static readonly Dictionary<Key, bool> _keyStates = new();
    private static readonly Dictionary<Key, bool> _previousKeyStates = new();

    static InputManager()
    {
        foreach (Key key in Enum.GetValues(typeof(Key)))
        {
            _keyStates[key] = false;
            _previousKeyStates[key] = false;
        }
    }

    public static void SetKeyState(Key key, bool isPressed)
    {
        _keyStates[key] = isPressed;
    }

    public static void Update()
    {
        foreach (Key key in Enum.GetValues(typeof(Key)))
        {
            _previousKeyStates[key] = _keyStates[key];
        }
    }

    public static bool IsKeyDown(Key key)
    {
        return _keyStates[key];
    }

    public static bool IsKeyPressed(Key key)
    {
        return _keyStates[key] && !_previousKeyStates[key];
    }

    public static bool IsKeyReleased(Key key)
    {
        return !_keyStates[key] && _previousKeyStates[key];
    }
}