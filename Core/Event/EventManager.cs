/*
  NoZ Game Engine

  Copyright(c) 2019 NoZ Games, LLC

  Permission is hereby granted, free of charge, to any person obtaining a copy
  of this software and associated documentation files(the "Software"), to deal
  in the Software without restriction, including without limitation the rights
  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
  copies of the Software, and to permit persons to whom the Software is
  furnished to do so, subject to the following conditions :

  The above copyright notice and this permission notice shall be included in all
  copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
  SOFTWARE.
*/

using System;
using System.Collections.Generic;

namespace NoZ
{
#if false

    public static class EventManager
    {
        private static Dictionary<EventBase, EventHandlerList> _globals =
            new Dictionary<EventBase, EventHandlerList>(ReferenceEqualityComparer<EventBase>.Instance);

        private static void AddHandler(Object source, EventBase e, Delegate d, bool oneShot)
        {
            var target = d.Target as Object;

            // Create the event handler.
            var handler = new EventHandler
            {
                Event = e,
                Delegate = EventDelegate.Create(d),
                Target = target != null ? new WeakReference<Object>(target) : null,
                Source = source != null ? new WeakReference<Object>(source) : null,
                OneShot = oneShot
            };

            // Add to the subscriber list of this object
            EventHandlerList handlerList = null;
            if (null == source)
            {
                if (!_globals.TryGetValue(e, out handlerList))
                {
                    handlerList = new EventHandlerList();
                    _globals[e] = handlerList;
                }
            }
            else
            {
                if (null == source._subscribers)
                    source._subscribers = new EventHandlerList();
                handlerList = source._subscribers;
            }

            handlerList.Add(handler);

            // If the target is another Object then add to its subscriptions list
            if (null != target)
            {
                if (null == target._subscriptions)
                    target._subscriptions = new EventHandlerList();
                target._subscriptions.Add(handler);
            }
        }

        /// <summary>
        /// Subscribe to an event raised by the object.
        /// </summary>
        /// <param name="e">Event to subscribe to</param>
        /// <param name="d">Delegate to call when event is raised.</param>
        /// <param name="oneShot">True if the the subscription should auto-matically unsubscribe when the event is raised.</param>
        public static void Subscribe(Object source, Event e, Event.Delegate d, bool oneShot = false) => AddHandler(source, e, d, oneShot);

        /// <summary>
        /// Subscribe to an event raised by the object.
        /// </summary>
        /// <param name="e">Event to subscribe to</param>
        /// <param name="d">Delegate to call when event is raised.</param>
        /// <param name="oneShot">True if the the subscription should auto-matically unsubscribe when the event is raised.</param>
        public static void Subscribe<Arg1>(Object source, Event<Arg1> e, Event<Arg1>.Delegate d, bool oneShot = false) => AddHandler(source, e, d, oneShot);

        public static void Subscribe<Arg1, Arg2>(Object source, Event<Arg1, Arg2> e, Event<Arg1, Arg2>.Delegate d, bool oneShot = false) => AddHandler(source, e, d, oneShot);
        public static void Subscribe<Arg1, Arg2, Arg3>(Object source, Event<Arg1, Arg2, Arg3> e, Event<Arg1, Arg2, Arg3>.Delegate d, bool oneShot = false) => AddHandler(source, e, d, oneShot);
        public static void Subscribe<Arg1, Arg2, Arg3, Arg4>(Object source, Event<Arg1, Arg2, Arg3, Arg4> e, Event<Arg1, Arg2, Arg3, Arg4>.Delegate d, bool oneShot = false) => AddHandler(source, e, d, oneShot);


        public static void Unsubscribe(Object source, EventBase e, Object target)
        {
            if (null == target._subscriptions)
                return;

            var enumerator = target._subscriptions.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var eh = enumerator.Current;
                if (e != null && eh.Event != e)
                    continue;

                if (source != null)
                {
                    Object handlerSource = null;
                    if (eh.Source != null)
                    {
                        if (!eh.Source.TryGetTarget(out handlerSource))
                            continue;
                    }

                    if (handlerSource == source)
                        continue;
                }
                else if (eh.Source != null)
                    continue;

                enumerator.Remove();

                // Remove the handler from the actual source.
                if (source == null)
                {
                    if (_globals.TryGetValue(e, out var globalHandlers))
                        globalHandlers.Remove(eh);
                }
                else
                {
                    source._subscribers.Remove(eh);
                }
            }
        }

        public static void Unsubscribe(Object target)
        {
            if (null == target._subscriptions)
                return;

            var enumerator = target._subscriptions.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var eh = enumerator.Current;

                enumerator.Remove();

                // Remove the handler from the actual source.
                if (eh.Source == null)
                {
                    if (_globals.TryGetValue(eh.Event, out var globalHandlers))
                        globalHandlers.Remove(eh);
                }
                else if (eh.Source.TryGetTarget(out var source))
                {
                    source._subscribers.Remove(eh);
                }
            }
        }

        public static void Unsubscribe(Object source, Object target) => Unsubscribe(source, null, target);

        /// <summary>
        /// Raise an event to all subscribers.
        /// </summary>
        /// <param name="e">Event to raise</param>
        public static void Raise(Object sender, Event e)
        {
            // Raise to the global handlers first
            if (_globals.TryGetValue(e, out var globalHandlers))
                e.Raise(globalHandlers);

            // Now to the specific sender handlers
            if (sender != null)
                e.Raise(sender._subscribers);
        }

        /// <summary>
        /// Raise an event to all subscribers
        /// </summary>
        /// <typeparam name="T">Type of first argument</typeparam>
        /// <param name="e">Event to raise.</param>
        /// <param name="arg1">Event argument</param>
        public static void Raise<Arg1>(Object sender, Event<Arg1> e, Arg1 arg1)
        {
            // Raise to the global handlers first
            if (_globals.TryGetValue(e, out var globalHandlers))
                e.Raise(globalHandlers, arg1);

            if (sender != null)
                e.Raise(sender._subscribers, arg1);
        }

        public static void Raise<Arg1, Arg2>(Object sender, Event<Arg1, Arg2> e, Arg1 arg1, Arg2 arg2)
        {
            // Raise to the global handlers first
            if (_globals.TryGetValue(e, out var globalHandlers))
                e.Raise(globalHandlers, arg1, arg2);

            if (sender != null)
                e.Raise(sender._subscribers, arg1, arg2);
        }

        public static void Raise<Arg1, Arg2, Arg3>(Object sender, Event<Arg1, Arg2, Arg3> e, Arg1 arg1, Arg2 arg2, Arg3 arg3)
        {
            // Raise to the global handlers first
            if (_globals.TryGetValue(e, out var globalHandlers))
                e.Raise(globalHandlers, arg1, arg2, arg3);

            if (sender != null)
                e.Raise(sender._subscribers, arg1, arg2, arg3);
        }

        public static void Raise<Arg1, Arg2, Arg3, Arg4>(Object sender, Event<Arg1, Arg2, Arg3, Arg4> e, Arg1 arg1, Arg2 arg2, Arg3 arg3, Arg4 arg4)
        {
            // Raise to the global handlers first
            if (_globals.TryGetValue(e, out var globalHandlers))
                e.Raise(globalHandlers, arg1, arg2, arg3, arg4);

            if (sender != null)
                e.Raise(sender._subscribers, arg1, arg2, arg3, arg4);
        }
    }
#endif
}
