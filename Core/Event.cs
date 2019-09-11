using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace NoZ {


    internal static class EventDelegate {
        private static Dictionary<MethodInfo, Delegate> _cache = new Dictionary<MethodInfo, Delegate>();

        public static Delegate Create(Delegate d) {
            if (d.Target == null)
                return d;

            if (_cache.TryGetValue(d.Method, out var cached))
                return cached;

            var parameters = d.Method.GetParameters();
            var parameterExpressions = new ParameterExpression[parameters.Length + 1];
            var callArguments = new Expression[parameters.Length];

            parameterExpressions[0] = Expression.Parameter(typeof(Object));

            for (int i = 0; i < callArguments.Length; i++) {
                parameterExpressions[i + 1] = Expression.Parameter(parameters[i].ParameterType);
                callArguments[i] = parameterExpressions[i + 1];
            }

            cached = Expression.Lambda(
                Expression.Call(
                    Expression.Convert(parameterExpressions[0], d.Method.DeclaringType),
                    d.Method,
                    callArguments),
                parameterExpressions
            ).Compile();

            _cache[d.Method] = cached;

            return cached;
        }
    }

    internal class EventHandler {
        public EventBase Event;
        public Delegate Delegate;
        public WeakReference<Object> Source;
        public WeakReference<Object> Target;
        public bool OneShot;
    }

    internal class EventHandlerList : IEnumerable<EventHandler> {
        public struct Enumerator : IEnumerator<EventHandler> {
            private EventHandlerList _list;
            private Item _current;
            private Item _previous;

            public EventHandler Current => _current.Handler;

            object IEnumerator.Current => throw new NotImplementedException();

            public Enumerator(EventHandlerList list) {
                _list = list;
                _current = null;
                _previous = null;
            }

            public void Dispose() { }

            public bool MoveNext() {
                _previous = _current;

                if (_current == null)
                    _current = _list._head;
                else
                    _current = _current.Next;

                if (null == _current)
                    return false;

                return true;
            }

            public void Remove() {
                if (_current == null)
                    return;

                if (_previous == null) {
                    _list._head = _current.Next;
                } else {
                    _previous.Next = _current.Next;
                }

                _current.Next = null;
                _current = _previous;
            }

            public void Reset() {
                _current = null;
                _previous = null;
            }
        }

        private class Item {
            public EventHandler Handler;
            public Item Next;
        }
        private Item _head;

        public void Add(in EventHandler handler) {
            if (Find(handler) != null)
                return;

            var item = new Item {
                Handler = handler,
                Next = _head
            };

            _head = item;
        }

        public void RemoveAllForTarget(Object target) {
            Item previous = null;
            for (var current = _head; current != null; current = current.Next) {
                var handler = current.Handler;
                if(handler.Target != null) {
                    if(handler.Target.TryGetTarget(out var handlerTarget)) {
                        if(handlerTarget == target) {
                            if (previous == null) {
                                _head = current.Next;
                            } else {
                                previous.Next = current.Next;
                            }
                            continue;
                        }
                    }
                }
                previous = current;
            }
        }

        public void Remove(EventHandler handler) {
            Item previous = null;
            for (var current = _head; current != null; current = current.Next) {
                if (current.Handler == handler) {
                    if (previous == null) {
                        _head = current.Next;
                    } else {
                        previous.Next = current.Next;
                    }
                    return;
                }
                previous = current;
            }
        }

        private Item Find(in EventHandler handler) {
            for (var item = _head; item != null; item = item.Next)
                if (item.Handler.Target == handler.Target &&
                    item.Handler.Event == handler.Event &&
                   item.Handler.Delegate == handler.Delegate)
                    return item;
            return null;
        }

        public Enumerator GetEnumerator() {
            return new Enumerator(this);
        }

        IEnumerator<EventHandler> IEnumerable<EventHandler>.GetEnumerator() {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            throw new NotImplementedException();
        }
    }

    public class EventBase { }

    public class Event : EventBase {
        public delegate void Delegate();

        internal void Raise(EventHandlerList list) {
            if (list == null)
                return;

            var enumerator = list.GetEnumerator();
            while (enumerator.MoveNext()) {
                var eh = enumerator.Current;
                if (eh.Event != this)
                    continue;

                if (eh.Target == null)
                    ((Delegate)eh.Delegate).Invoke();
                else if (eh.Target.TryGetTarget(out var target))
                    ((Action<Object>)eh.Delegate)(target);

                if (eh.OneShot)
                    enumerator.Remove();
            }
        }
    }

    public class Event<Arg1> : EventBase {
        public delegate void Delegate(Arg1 arg1);

        internal void Raise(EventHandlerList list, Arg1 arg1) {
            if (list == null)
                return;

            var enumerator = list.GetEnumerator();
            while (enumerator.MoveNext()) {
                var eh = enumerator.Current;
                if (eh.Event != this)
                    continue;

                if (eh.Target == null)
                    ((Delegate)eh.Delegate).Invoke(arg1);
                else if (eh.Target.TryGetTarget(out var target))
                    ((Action<Object, Arg1>)eh.Delegate)(target, arg1);

                if (eh.OneShot)
                    enumerator.Remove();
            }
        }
    }


    public class Event<Arg1,Arg2> : EventBase {
        public delegate void Delegate(Arg1 arg1, Arg2 arg2);

        internal void Raise(EventHandlerList list, Arg1 arg1, Arg2 arg2) {
            if (list == null)
                return;

            var enumerator = list.GetEnumerator();
            while (enumerator.MoveNext()) {
                var eh = enumerator.Current;
                if (eh.Event != this)
                    continue;

                if (eh.Target == null)
                    ((Delegate)eh.Delegate).Invoke(arg1,arg2);
                else if (eh.Target.TryGetTarget(out var target))
                    ((Action<Object, Arg1, Arg2>)eh.Delegate)(target, arg1, arg2);

                if (eh.OneShot)
                    enumerator.Remove();
            }
        }
    }

    public class Event<Arg1,Arg2,Arg3> : EventBase {
        public delegate void Delegate(Arg1 arg1, Arg2 arg2, Arg3 arg3);

        internal void Raise(EventHandlerList list, Arg1 arg1, Arg2 arg2, Arg3 arg3) {
            if (list == null)
                return;

            var enumerator = list.GetEnumerator();
            while (enumerator.MoveNext()) {
                var eh = enumerator.Current;
                if (eh.Event != this)
                    continue;

                if (eh.Target == null)
                    ((Delegate)eh.Delegate).Invoke(arg1,arg2,arg3);
                else if (eh.Target.TryGetTarget(out var target))
                    ((Action<Object, Arg1, Arg2, Arg3>)eh.Delegate)(target, arg1, arg2, arg3);

                if (eh.OneShot)
                    enumerator.Remove();
            }
        }
    }

    public class Event<Arg1, Arg2, Arg3, Arg4> : EventBase {
        public delegate void Delegate(Arg1 arg1, Arg2 arg2, Arg3 arg3, Arg4 arg4);

        internal void Raise(EventHandlerList list, Arg1 arg1, Arg2 arg2, Arg3 arg3, Arg4 arg4) {
            if (list == null)
                return;

            var enumerator = list.GetEnumerator();
            while (enumerator.MoveNext()) {
                var eh = enumerator.Current;
                if (eh.Event != this)
                    continue;

                if (eh.Target == null)
                    ((Delegate)eh.Delegate).Invoke(arg1,arg2,arg3,arg4);
                else if (eh.Target.TryGetTarget(out var target))
                    ((Action<Object, Arg1, Arg2, Arg3, Arg4>)eh.Delegate)(target, arg1, arg2, arg3, arg4);

                if (eh.OneShot)
                    enumerator.Remove();
            }
        }
    }

}
