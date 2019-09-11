using System;
using System.Collections.Generic;
using System.Text;

namespace NoZ {
    public abstract class AudioStream : Object {
        public abstract void SeekBegin();

        public abstract long Read(short[] data);
    }
}
