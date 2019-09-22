using System;

namespace NoZ
{
    public class Object : IDisposable
    {
        public void Subscribe(Event e, Event.EventDelegate d, bool oneShot = false) => e.Subscribe(d, oneShot);
        public void Subscribe<Arg1>(Event<Arg1> e, Event<Arg1>.EventDelegate d, bool oneShot = false) => e.Subscribe(d, oneShot);
        public void Subscribe<Arg1, Arg2>(Event<Arg1, Arg2> e, Event<Arg1, Arg2>.EventDelegate d, bool oneShot = false) => e.Subscribe(d, oneShot);
        public void Subscribe<Arg1, Arg2, Arg3>(Event<Arg1, Arg2, Arg3> e, Event<Arg1, Arg2, Arg3>.EventDelegate d, bool oneShot = false) => e.Subscribe(d, oneShot);

        public void Unsubscribe(Event e, Object target) => e.Unsubscribe(target);
        public void Unsubscribe<Arg1>(Event<Arg1> e, Object target) => e.Unsubscribe(target);
        public void Unsubscribe<Arg1,Arg2>(Event<Arg1, Arg2> e, Object target) => e.Unsubscribe(target);
        public void Unsubscribe<Arg1, Arg2, Arg3>(Event<Arg1, Arg2, Arg3> e, Object target) => e.Unsubscribe(target);

        public void UnsubscribeAll() => EventBase.UnsubscribeAllObservers(this);

        public void Broadcast(Event e) => e.Broadcast(this);
        public void Broadcast<Arg1>(Event<Arg1> e, Arg1 arg1) => e.Broadcast(this, arg1);
        public void Broadcast<Arg1,Arg2>(Event<Arg1,Arg2> e, Arg1 arg1, Arg2 arg2) => e.Broadcast(this, arg1, arg2);
        public void Broadcast<Arg1,Arg2,Arg3>(Event<Arg1,Arg2,Arg3> e, Arg1 arg1, Arg2 arg2, Arg3 arg3) => e.Broadcast(this, arg1, arg2, arg3);

        public virtual void Dispose()
        {
            UnsubscribeAll();
        }
    }

}
