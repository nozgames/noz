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
    internal class EventHandler
    {
        /// <summary>
        /// Used to compare event handlers when sorting.  Handlers are sorted first by
        /// Their event identifier and then by the handler identifier.
        /// </summary>
        public class Comparer : IComparer<EventHandler>
        {
            public static Comparer Instance = new Comparer();

            public int Compare(EventHandler lhs, EventHandler rhs)
            {
                // Sort first by event id 
                var diff = lhs._event.Id - rhs._event.Id;

                // Then sort
                return diff == 0 ? (lhs._id - rhs._id) : diff;
            }
        }

        /// <summary>
        /// Unique identifier of the handler
        /// </summary>
        internal int _id;

        /// <summary>
        /// Event that owns this handler
        /// </summary>
        internal EventBase _event = null;

        /// <summary>
        /// Delegate to call when the event fires
        /// </summary>
        internal Delegate _delegate;

        /// <summary>
        /// Reference to source object for handler
        /// </summary>
        internal WeakReference<Object> _source = null;

        /// <summary>
        /// Reference to target object for handler
        /// </summary>
        internal WeakReference<Object> _target = null;

        /// <summary>
        /// True if the handler should be removed after the first time it is called
        /// </summary>
        internal bool _isOneShot;

        /// <summary>
        /// Return true if the handler is still valid
        /// </summary>
        public bool IsValid => (_source == null || _source.TryGetTarget(out var source)) && 
                               (_target != null && _target.TryGetTarget(out var target));
    }
}
