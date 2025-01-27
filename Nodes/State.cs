
namespace NoZ.Nodes
{
    public abstract class State : Node
    {
        public StateMachine? StateMachine { get; private set; }

        protected override void OnEnterWorld()
        {
            base.OnEnterWorld();

            // Cache the parent state machine.
            StateMachine = GetParent<StateMachine>();
        }

        protected void SetState<TState>() where TState : State =>
            SetState(typeof(TState));

        protected void SetState(Type stateType) =>
            StateMachine?.SetState(stateType);

        protected internal virtual void OnEnter() { }
        protected internal virtual void OnUpdate() { }
        protected internal virtual void OnExit() { }
    }

    public abstract class State<TOwner> : State where TOwner : Node
    {
        protected TOwner Owner { get; private set; }

        public State(TOwner owner)
        {
            Owner = owner;
        }
    }
}
