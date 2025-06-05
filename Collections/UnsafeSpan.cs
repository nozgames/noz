/*

    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

namespace NoZ.Collections
{
    internal unsafe struct UnsafeSpan<T> where T : unmanaged
    {
        private readonly T* _data;
        private readonly int _length;

        public T* Pointer => _data;

        public int Length => _length;

        public UnsafeSpan(in UnsafeMemory<T> memory, int offset, int count)
        {
            _data = memory.Pointer + offset;
            _length = count;
        }

        public static implicit operator T* (UnsafeSpan<T> span) => span.Pointer;

        public readonly Span<T> AsSpan() =>
            new Span<T>(_data, _length);
    }
}
