using System;

namespace DN.States
{
    public abstract class GameState:IDisposable
    {
        protected StateManager StateManager;

        public GameState(StateManager stateManager)
        {
            StateManager = stateManager;
        }

        internal abstract void LoadContent();
        internal abstract void UnloadContent();

        internal abstract void Init();
        internal abstract void Update(float dt);
        internal abstract void Draw(float dt);

        public void Dispose()
        {
            UnloadContent();
        }
    }
}
