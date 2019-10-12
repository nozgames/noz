using System;
using System.IO;

namespace NoZ
{
    public static partial class Extensions
    {
        public static void Write(this BinaryWriter writer, in Thickness thickness)
        {
            writer.Write(thickness.left);
            writer.Write(thickness.top);
            writer.Write(thickness.right);
            writer.Write(thickness.bottom);
        }

        public static void Write(this BinaryWriter writer, in Vector2 value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
        }

        public static void Write(this BinaryWriter writer, in Rect value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
            writer.Write(value.width);
            writer.Write(value.height);
        }

        public static void Write(this BinaryWriter writer, short[] value)
        {
            byte[] bytes = new byte[value.Length * 2];
            Buffer.BlockCopy(value, 0, bytes, 0, bytes.Length);
            writer.Write(bytes);
        }
    }
}
