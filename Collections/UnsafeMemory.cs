/*

    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

namespace NoZ.Collections
{
    internal unsafe struct UnsafeMemory<T> : IDisposable where T : unmanaged
    {
        private T* _data;
        private int _length;

        public T* Pointer => _data;

        public int Length => _length;

        public UnsafeMemory(int size)
        {
            _data = (T*)System.Runtime.InteropServices.Marshal.AllocHGlobal(sizeof(T));
            _length = size;
        }

        public void Dispose()
        {
            if (_data == null)
                return;

            System.Runtime.InteropServices.Marshal.FreeHGlobal((IntPtr)_data);

            _data = null;
            _length = 0;
        }
    }
}
