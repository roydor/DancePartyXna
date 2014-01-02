using System;
using System.Collections.Generic;
namespace DanceParty.GameStates
{
    /// <summary>
    /// Simple Command Pattern for executing Actions on the GameStateStack.
    /// </summary>
    public interface IStackAction
    {
        void Execute(Stack<IGameState> stack);
    }

    /// <summary>
    /// A class to enqueue a new game state.
    /// </summary>
    public class EnqueueStateAction : IStackAction
    {
        private IGameState _nextState;
        public EnqueueStateAction(IGameState nextState)
        {
            _nextState = nextState;
        }

        public void Execute(Stack<IGameState> stack)
        {
            stack.Push(_nextState);
        }
    }

    /// <summary>
    /// A class to pop the current state.
    /// </summary>
    public class PopStateAction : IStackAction
    {
        public void Execute(Stack<IGameState> stack)
        {
            stack.Pop();
        }
    }

}
