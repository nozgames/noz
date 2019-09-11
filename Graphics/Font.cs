using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

//using NoZ.Serialization;

namespace NoZ {
    //[SharedResource]
    //[Version(1)]
    public class Font { // : IResource, ISerializedType {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class Glyph {
            public Vector2 Bearing { get; private set; }
            public Vector2 Size { get; private set; }
            public Vector2 S { get; private set; }
            public Vector2 T { get; private set; }
            public char Ascii { get; private set; }
            public float Advance { get; private set; }

            public Glyph (char ascii, float advance, in Vector2 bearing, in Vector2 size, in Vector2 s, in Vector2 t) {
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

#if false
        Resource IResource.Resource { get; set; }
#endif

        public Glyph GetGlyph (char c) {
            return _glyphs[c];
        }

        public float GetAdvance (char from, char to) {
            var glyph = GetGlyph(from);
            if(glyph == null) {
                return 0f;
            }

            float advance = glyph.Advance;
            if (to != 0 && _kerning != null) {
                _kerning.TryGetValue((ushort)((from << 8) + to), out var kern);
                advance += kern;
            }

            return advance;
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

        void ISerializedType.Serialize (BinarySerializer writer) {
            writer.WriteSingle(Ascent);
            writer.WriteSingle(Height);
            writer.WriteInt32(Resolution);
            writer.WriteObject(Image);

            if (_kerning != null) {
                writer.WriteInt32(_kerning.Count);
                foreach (var kern in _kerning) {
                    writer.WriteUInt16(kern.Key);
                    writer.WriteSingle(kern.Value);
                }
            } else {
                writer.WriteInt32(0);
            }

            for (int i = 0; i < _glyphs.Length; i++) {
                if (_glyphs[i] == null) continue;
                writer.WriteByte((byte)i);
                writer.WriteSingle(_glyphs[i].Advance);
                writer.WriteVector2(_glyphs[i].Bearing);
                writer.WriteVector2(_glyphs[i].Size);
                writer.WriteVector2(_glyphs[i].S);
                writer.WriteVector2(_glyphs[i].T);
            }

            writer.WriteByte(0xFF);
        }

        private Font () { }
        
        public static Font Create (int resolution, float height, float ascent, Image image, Glyph[] glyphs, Dictionary<ushort,float> kerning) {
            var font = new Font();
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
#endif
    }
}
