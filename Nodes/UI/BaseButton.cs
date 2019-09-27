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
using System.Text;

namespace NoZ.UI
{
    public enum ClickMode
    {
        /// <summary>
        /// Issue a Click on mouse down
        /// </summary>
        Press,

        /// <summary>
        /// Issue a click on mouse up
        /// </summary>
		Release
    };

    public class BaseButton : UINode
    {
        public static readonly Event<BaseButton> PressedEvent = new Event<BaseButton>();
        public static readonly Event<BaseButton> ReleaseEvent = new Event<BaseButton>();
        public static readonly Event<BaseButton> ClickedEvent = new Event<BaseButton>();

        public BaseButton()
        {
            IsInteractive = true;
        }

        public ClickMode ClickMode { get; set; } = ClickMode.Release;
    }
}
