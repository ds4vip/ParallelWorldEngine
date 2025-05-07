namespace ParallelWorldEngine;

    /// <summary>
    /// コンポーネント基底クラス
    /// </summary>
    public abstract class Component
    {
        public Entity Owner { get; internal set; }
        public CommandSystem CommandSystem => Owner?.World?.CommandSystem;

        public virtual void Initialize() { }
        public virtual void Update(float deltaTime) { }
    }
