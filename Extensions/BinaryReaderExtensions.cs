﻿using System;
using System.IO;
using System.Runtime.InteropServices;

namespace NoZ
{
    public static partial class Extensions
    {
        public static Thickness ReadThickness(this BinaryReader reader)
        {
            return new Thickness(
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle()
                );
        }

        public static Rect ReadRect(this BinaryReader reader)
        {
            return new Rect(
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle()
                );
        }

        public static Vector2 ReadVector2 (this BinaryReader reader)
        {
            return new Vector2(reader.ReadSingle(), reader.ReadSingle());
        }

        public static short[] ReadShorts(this BinaryReader reader, int count)
        {
            byte[] bytes = reader.ReadBytes(count * 2);
            if (bytes.Length == 0)
                return new short[0] { };

            var shorts = new short[count];
            Buffer.BlockCopy(bytes, 0, shorts, 0, bytes.Length);
            return shorts;
        }

        public static void ReadStruct<T> (this BinaryReader reader, out T value) where T : struct
        {
            value = new T();
            byte[] bytes = reader.ReadBytes(Marshal.SizeOf(typeof(T)));
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            value = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();
        }
    }
}
