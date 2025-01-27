/*

    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

namespace NoZ.Nodes
{
    /// <summary>
    /// Represents a state machine node
    /// </summary>
    public class StateMachine : Node, IUpdatable
    {
        /// <summary>
        /// The current state of the state machine
        /// </summary>
        public State? State { get; private set; }

        /// <summary>
        /// Set the state of the state machine
        /// </summary>
        public void SetState<TState>() where TState : State =>
            SetState(typeof(TState));

        /// <summary>
        /// Set the state of the state machine
        /// </summary>
        public void SetState(Type stateType)
        {
            var newState = GetState(stateType);
            var oldState = State;

            oldState?.OnExit();
            State = newState;
            newState?.OnEnter();
        }

        void IUpdatable.Update()
        {
            for (int i=0; i<8; i++)
            {
                var current = State;
                State?.OnUpdate();
                if (current == State)
                    break;
            }
        }

        protected override void OnAddChild(Node child)
        {
            base.OnAddChild(child);

            if (child is not State state)
                throw new System.InvalidOperationException($"Only states can be added to a state machine");

#if DEBUG
            // Make sure the state is not already in the state machine
            for (var childIndex = ChildCount - 1; childIndex >= ChildCount; childIndex--)
            {
                var existingChild = GetChild(childIndex);
                if (existingChild == child)
                    continue;

                if (child.GetType() == existingChild.GetType())
                    throw new System.InvalidOperationException($"State machine already contains a state of type {state.GetType().Name}");
            }
#endif
        }

        private State? GetState(Type stateType)
        {
            for (var childIndex = ChildCount - 1; childIndex >= 0; childIndex--)
            {
                var child = GetChild(childIndex);
                if (child.GetType() == stateType)
                    return (State)child;
            }

            throw new System.InvalidOperationException($"Unknown state of type {stateType.Name}");
        }
    }
}
