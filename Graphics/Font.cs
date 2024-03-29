﻿/*
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

using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace NoZ
{
    public class Font : Resource
    { 
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class Glyph
        {
            public Vector2 Bearing { get; private set; }
            public Vector2 Size { get; private set; }
            public Vector2 S { get; private set; }
            public Vector2 T { get; private set; }
            public char Ascii { get; private set; }
            public float Advance { get; private set; }

            public Glyph(char ascii, float advance, in Vector2 bearing, in Vector2 size, in Vector2 s, in Vector2 t)
            {
                Advance = advance;
                Ascii = ascii;
                Bearing = bearing;
                Size = size;
                S = s;
                T = t;
            }
        }

        private Glyph[] _glyphs;
        private Dictionary<ushort, float> _kerning;

        public int Resolution { get; private set; }

        public float Height { get; private set; }

        public float Ascent { get; private set; }

        public Image Image { get; private set; }

        protected Font(string name) : base(name)
        {
        }

        public Glyph GetGlyph(char c)
        {
            return _glyphs[c];
        }

        public float GetAdvance(char from, char to)
        {
            var glyph = GetGlyph(from);
            if (glyph == null)
            {
                return 0f;
            }

            float advance = glyph.Advance;
            if (to != 0 && _kerning != null)
            {
                _kerning.TryGetValue((ushort)((from << 8) + to), out var kern);
                advance += kern;
            }

            return advance;
        }


        public void Save(BinaryWriter writer)
        {
            Image.Save(writer);
            writer.Write(Ascent);
            writer.Write(Height);
            writer.Write(Resolution);

            if (_kerning != null)
            {
                writer.Write(_kerning.Count);
                foreach (var kern in _kerning)
                {
                    writer.Write(kern.Key);
                    writer.Write(kern.Value);
                }
            }
            else
            {
                writer.Write(0);
            }

            for (int i = 0; i < _glyphs.Length; i++)
            {
                if (_glyphs[i] == null) continue;
                writer.Write((byte)i);
                writer.Write(_glyphs[i].Advance);
                writer.Write(_glyphs[i].Bearing);
                writer.Write(_glyphs[i].Size);
                writer.Write(_glyphs[i].S);
                writer.Write(_glyphs[i].T);
            }

            writer.Write(0xFF);
        }

        /// <summary>
        /// Create a named font from a stream
        /// </summary>
        /// <param name="name">Name of font</param>
        /// <param name="reader">Stream to read from</param>
        /// <returns>Created image</returns>
        public static Font Create(string name, BinaryReader reader)
        {
            // Create the image
            var font = new Font(name);

            font.Image = Image.Create(null, reader);
            font.Ascent = reader.ReadSingle();
            font.Height = reader.ReadSingle();
            font.Resolution = reader.ReadInt32();
            

            int kernCount = reader.ReadInt32();
            if (kernCount > 0)
            {
                font._kerning = new Dictionary<ushort, float>();
                for (int kern = 0; kern < kernCount; kern++)
                {
                    ushort key = reader.ReadUInt16();
                    font._kerning[key] = reader.ReadSingle();
                }
            }

            font._glyphs = new Glyph[255];

            while (true)
            {
                int index = reader.ReadByte();
                if (index == 0xFF)
                    break;

                var advance = reader.ReadSingle();
                var bearing = reader.ReadVector2();
                var size = reader.ReadVector2();
                var s = reader.ReadVector2();
                var t = reader.ReadVector2();

                font._glyphs[index] = new Glyph((char)index, advance, bearing, size, s, t);
            }

            return font;
        }

        public static Font Create(int resolution, float height, float ascent, Image image, Glyph[] glyphs, Dictionary<ushort, float> kerning)
        {
            var font = new Font(null);
            font.Resolution = resolution;
            font.Height = height;
            font.Ascent = ascent;
            font.Image = image;
            font._glyphs = new Glyph[255];
            font._kerning = kerning;

            foreach (var glyph in glyphs)
                font._glyphs[glyph.Ascii] = glyph;

            return font;
        }

#if false
        void ISerializedType.Deserialize(BinaryDeserializer reader) {
            Ascent = reader.ReadSingle();
            Height = reader.ReadSingle();
            Resolution = reader.ReadInt32();
            Image = (Image)reader.ReadObject();

            int kernCount = reader.ReadInt32();
            if (kernCount > 0) {
                _kerning = new Dictionary<ushort, float>();
                for (int kern = 0; kern < kernCount; kern++) {
                    ushort key = reader.ReadUInt16();
                    _kerning[key] = reader.ReadSingle();
                }
            }

            _glyphs = new Glyph[255];

            while(true) {
                int index = reader.ReadByte();
                if (index == 0xFF)
                    break;

                var advance = reader.ReadSingle();
                var bearing = reader.ReadVector2();
                var size = reader.ReadVector2();
                var s = reader.ReadVector2();
                var t = reader.ReadVector2();

                _glyphs[index] = new Glyph((char)index, advance, bearing, size, s, t);
            }
        }

        private Font () { }
        

#endif
    }
}
