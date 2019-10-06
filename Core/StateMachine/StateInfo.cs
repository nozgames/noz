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
    /// <summary>
    /// Represents a state within a state machine
    /// </summary>
    internal class StateInfo
    {
        /// <summary>
        /// Name of the state
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Unique mask of the state
        /// </summary>
        public ulong Mask { get; internal set; }

        /// <summary>
        /// Delegate used to invoke the state update method
        /// </summary>
        public Reflection.OpenDelegate InvokeDelegate { get; internal set; }

        /// <summary>
        /// Delegate used to invoke the state updated method with an elapsed time
        /// </summary>
        public Reflection.OpenDelegate<float> InvokeWithTimeDelegate { get; internal set; }

        /// <summary>
        /// Ivoke the state method on the given target if the state has one
        /// </summary>
        public void Invoke (object target, float time)
        {
            if (InvokeWithTimeDelegate != null)
                InvokeWithTimeDelegate.Invoke(target, time);
            else if(InvokeDelegate != null)
                InvokeDelegate.Invoke(target);
        }
    }
}
