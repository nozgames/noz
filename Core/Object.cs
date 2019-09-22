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
