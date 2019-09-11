using System;
using System.Collections.Generic;
using System.Text;

namespace NoZ
{
    public abstract class Window
    {
        public abstract IntPtr GetNativeHandle();

        public abstract void BeginFrame();
        public abstract void EndFrame();
    }
}
