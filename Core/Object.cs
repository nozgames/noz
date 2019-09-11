using System;
using System.IO;
//using NoZ.Serialization;

namespace NoZ {

    public class Object : IDisposable {
        /// <summary>
        /// List of all subscribers for events raised on this object.
        /// </summary>
        internal EventHandlerList _subscribers;

        /// <summary>
        /// List of all event subscriptions.
        /// </summary>
        internal EventHandlerList _subscriptions;

        /// <summary>
        /// Subscribe to an event raised by the object.
        /// </summary>
        /// <param name="e">Event to subscribe to</param>
        /// <param name="d">Delegate to call when event is raised.</param>
        /// <param name="oneShot">True if the the subscription should auto-matically unsubscribe when the event is raised.</param>
        public void Subscribe(Event e, Event.Delegate d, bool oneShot = false) => EventManager.Subscribe(this, e, d, oneShot);

        /// <summary>
        /// Subscribe to an event raised by the object.
        /// </summary>
        /// <param name="e">Event to subscribe to</param>
        /// <param name="d">Delegate to call when event is raised.</param>
        /// <param name="oneShot">True if the the subscription should auto-matically unsubscribe when the event is raised.</param>
        public void Subscribe<Arg1>(Event<Arg1> e, Event<Arg1>.Delegate d, bool oneShot=false) => EventManager.Subscribe(this, e, d, oneShot);

        public void Subscribe<Arg1,Arg2>(Event<Arg1,Arg2> e, Event<Arg1,Arg2>.Delegate d, bool oneShot = false) => EventManager.Subscribe(this, e, d, oneShot);
        public void Subscribe<Arg1,Arg2,Arg3>(Event<Arg1,Arg2,Arg3> e, Event<Arg1,Arg2,Arg3>.Delegate d, bool oneShot = false) => EventManager.Subscribe(this, e, d, oneShot);
        public void Subscribe<Arg1,Arg2,Arg3,Arg4>(Event<Arg1,Arg2,Arg3,Arg4> e, Event<Arg1,Arg2,Arg3,Arg4>.Delegate d, bool oneShot = false) => EventManager.Subscribe(this, e, d, oneShot);

        /// <summary>
        /// Unsubscribe from all events on the this object for the given target
        /// </summary>
        /// <param name="target"></param>
        public void Unsubscribe(Object target) => EventManager.Unsubscribe(this, target);

        /// <summary>
        /// Unsubscribe from all events
        /// </summary>
        protected void UnsubscribeAll () => EventManager.Unsubscribe(this);

        /// <summary>
        /// Raise an event to all subscribers.
        /// </summary>
        /// <param name="e">Event to raise</param>
        protected void Raise(Event e) => EventManager.Raise(this, e);

        /// <summary>
        /// Raise an event to all subscribers
        /// </summary>
        /// <typeparam name="T">Type of first argument</typeparam>
        /// <param name="e">Event to raise.</param>
        /// <param name="arg1">Event argument</param>
        protected void Raise<Arg1>(Event<Arg1> e, Arg1 arg1) => EventManager.Raise(this, e, arg1);
        protected void Raise<Arg1, Arg2>(Event<Arg1, Arg2> e, Arg1 arg1, Arg2 arg2) => EventManager.Raise(this, e, arg1, arg2);
        protected void Raise<Arg1, Arg2, Arg3>(Event<Arg1, Arg2, Arg3> e, Arg1 arg1, Arg2 arg2, Arg3 arg3) => EventManager.Raise(this, e, arg1, arg2, arg3);
        protected void Raise<Arg1, Arg2, Arg3, Arg4>(Event<Arg1, Arg2, Arg3, Arg4> e, Arg1 arg1, Arg2 arg2, Arg3 arg3, Arg4 arg4) => EventManager.Raise(this, e, arg1, arg2, arg3, arg4);


#if false
        /// <summary>
        /// Clone the given object using serialziation.
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public Object Clone() {
            using (var stream = new MemoryStream(1024)) {
                BinarySerializer.Serialize(stream, this);
                stream.Position = 0;
                return BinaryDeserializer.Deserialize(stream) as Object;
            }
        }

        /// <summary>
        /// Clone the given object using serialziation.
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public T Clone<T>() where T : Object => Clone() as T;
#endif

        public virtual void Dispose() {
            UnsubscribeAll();
        }
    }

}
