namespace DN.States
{
    public class StateManager
    {
        private GameState _currentState;
        private GameState _nextGameState = null;
        public StateManager()
        {
        }

        public void SetState(GameState gameState)
        {
            _nextGameState = gameState;
        }

        public void Update(float dt)
        {
            if (_nextGameState != null)
            {
                ChangeState();
                _nextGameState = null;
            }
            _currentState.Update(dt);
        }

        public void Draw(float dt)
        {
            _currentState.Draw(dt);
        }


        private void ChangeState()
        {
            if (_currentState != null)
                _currentState.UnloadContent();
            _currentState = _nextGameState;
            _currentState.LoadContent();
            _currentState.Init();
        }
    }
}
