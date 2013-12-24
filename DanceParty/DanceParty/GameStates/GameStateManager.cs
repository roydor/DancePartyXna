using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DanceParty.GameStates
{
    public class GameStateManager
    {
        private Stack<IGameState> _stateStack;
        private Queue<IStackAction> _stackActions;

        private static GameStateManager _instance;
        public static GameStateManager Instance
        {
            get
            {
                return _instance ?? (_instance = new GameStateManager());
            }
        }

        private GameStateManager()
        {
            _stateStack = new Stack<IGameState>();
            _stackActions = new Queue<IStackAction>();

        }

        public void EnqueueGameState(IGameState newGameState)
        {
            _stackActions.Enqueue(new EnqueueStateAction(newGameState));
        }

        public void PopGameState()
        {
            _stackActions.Enqueue(new PopStateAction());
        }

        public IGameState GetCurrentState()
        {
            return _stateStack.Peek();
        }

        public void ProcessStateActions()
        {
            while (_stackActions.Count > 0)
                _stackActions.Dequeue().Execute(_stateStack);
        }
    }
}
