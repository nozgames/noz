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
using System.Collections;
using System.Collections.Generic;

namespace NoZ
{
    internal class EventHandlerList : IEnumerable<EventHandler>
    {
        public struct Enumerator : IEnumerator<EventHandler>
        {
            private EventHandlerList _list;
            private Item _current;
            private Item _previous;

            public EventHandler Current => _current.Handler;

            object IEnumerator.Current => throw new NotImplementedException();

            public Enumerator(EventHandlerList list)
            {
                _list = list;
                _current = null;
                _previous = null;
            }

            public void Dispose() { }

            public bool MoveNext()
            {
                _previous = _current;

                if (_current == null)
                    _current = _list._head;
                else
                    _current = _current.Next;

                if (null == _current)
                    return false;

                return true;
            }

            public void Remove()
            {
                if (_current == null)
                    return;

                if (_previous == null)
                {
                    _list._head = _current.Next;
                }
                else
                {
                    _previous.Next = _current.Next;
                }

                _current.Next = null;
                _current = _previous;
            }

            public void Reset()
            {
                _current = null;
                _previous = null;
            }
        }

        private class Item
        {
            public EventHandler Handler;
            public Item Next;
        }
        private Item _head;

        public void Add(in EventHandler handler)
        {
            if (Find(handler) != null)
                return;

            var item = new Item
            {
                Handler = handler,
                Next = _head
            };

            _head = item;
        }

        public void RemoveAllForTarget(Object target)
        {
            Item previous = null;
            for (var current = _head; current != null; current = current.Next)
            {
                var handler = current.Handler;
                if (handler._target != null)
                {
                    if (handler._target.TryGetTarget(out var handlerTarget))
                    {
                        if (handlerTarget == target)
                        {
                            if (previous == null)
                            {
                                _head = current.Next;
                            }
                            else
                            {
                                previous.Next = current.Next;
                            }
                            continue;
                        }
                    }
                }
                previous = current;
            }
        }

        public void Remove(EventHandler handler)
        {
            Item previous = null;
            for (var current = _head; current != null; current = current.Next)
            {
                if (current.Handler == handler)
                {
                    if (previous == null)
                    {
                        _head = current.Next;
                    }
                    else
                    {
                        previous.Next = current.Next;
                    }
                    return;
                }
                previous = current;
            }
        }

        private Item Find(in EventHandler handler)
        {
            for (var item = _head; item != null; item = item.Next)
                if (item.Handler._target == handler._target &&
                    item.Handler._delegate == handler._delegate)
                    return item;
            return null;
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<EventHandler> IEnumerable<EventHandler>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
