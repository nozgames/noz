/*

    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

namespace NoZ.Collections
{
    public unsafe struct UnsafeSpan<T> where T : unmanaged
    {
        private readonly T* _data;
        private readonly int _length;

        public T* Pointer => _data;

        public int Length => _length;

        public T this[int i]
        {
            get => _data[i];
            set => _data[i] = value;
        }

        public UnsafeSpan(in UnsafeMemory<T> memory, int offset, int count)
        {
            _data = memory.Pointer + offset;
            _length = count;
        }

        public UnsafeSpan(in UnsafeSpan<T> span, int offset, int count)
        {
            _data = span.Pointer + offset;
            _length = count;
        }

        public UnsafeSpan(T* pointer, int length)
        {
            _data = pointer;
            _length = length;
        }

        public static implicit operator T* (UnsafeSpan<T> span) => span.Pointer;

        public readonly Span<T> AsSpan() =>
            new Span<T>(_data, _length);

        public UnsafeSpan<T> Slice(int start, int length) =>
            new UnsafeSpan<T>(_data + start, length);

        public UnsafeSpan<T> Slice(int start) =>
            new UnsafeSpan<T>(_data + start, _length - start);
    }
}
