/*
  NozEngine Library

  Copyright(c) 2015 NoZ Games, LLC

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

namespace NoZ {

    public enum SystemCursor {
        None,
        Default,
        Arrow,
        IBeam
    }

#if false
    [SystemResource]
    [SerializedType(Allocator = typeof(Allocator))]
    public sealed class ArrowCursor : Cursor {
        private ArrowCursor() { }
        private static class Allocator {
            public static object CreateInstance() => Game.GraphicsDriver.CreateCursor(SystemCursor.Arrow);
        }
    }

    [SystemResource]
    [SerializedType(Allocator = typeof(Allocator))]
    public sealed class IBeamCursor : Cursor {
        private IBeamCursor() { }
        private static class Allocator {
            public static object CreateInstance() => Game.GraphicsDriver.CreateCursor(SystemCursor.IBeam);
        }
    }
#endif
}
