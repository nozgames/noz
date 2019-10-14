

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

using System;

namespace NoZ
{
    public enum TextOverflowHorizontal
    {
        Wrap,
        Clip,
        Overflow,
        Ellipsis
    };

    public enum TextOverflowVertical
    {
        Clip,
        Overflow
    };

    public enum TextAlignment
    {
        Left,
        Center,
        Right,
        Justified
    }

    public class Label : Node
    {
        /// <summary>
        /// Structure which represents a single line of characters within the label
        /// </summary>
        protected struct Line
        {
            public Rect bounds;
            public uint charMin;
            public uint charMax;
        }

        /// <summary>
        /// Structure which represents a single character within the label
        /// </summary>
        protected struct Character
        {
            /// Font glyph to render the character with
            public Font.Glyph glyph;

            /// Index of the line the character is on
            public int line;

            /// Bounds of the character in local coordinates
            public Rect bounds;

            /// <summary>
            /// Index of the character within the source text
            /// </summary>
            public int index;
        };

        protected Line[] _lines;
        protected Character[] _characters;
        private TextAlignment _textAlignment;
        private TextOverflowHorizontal _horizontalOverflow;
        private TextOverflowVertical _verticalOveflow;
        private float _fontSize;
        private string _text = "";
        private Font _font;
        private Quad[] _quads;
        private int _quadCount;
        private Vector2 _textSize;

        public int SortOrder { get; set; }

        public int SortLayer { get; set; }

        public MaskMode MaskMode { get; set; } = MaskMode.Inside;

        public Font Font {
            get { return _font; }
            set {
                if (_font != value)
                {
                    _font = value;
                    InvalidateMesh();
                }
            }
        }

        public float FontSize {
            get { return _fontSize; }
            set {
                if (_fontSize != value)
                {
                    _fontSize = value;
                    InvalidateMesh();
                }
            }
        }

        public string Text {
            get { return _text; }
            set {
                if (_text != value)
                {
                    _text = value;
                    if (_text == null)
                        _text = "";
                    InvalidateRect();
                    InvalidateMesh();
                    IsDrawable = Text != null;
                }
            }
        }

        public Color Color { get; set; } = Color.White;

        public ColorMode ColorMode { get; set; }

        public TextAlignment TextAlignment {
            get { return _textAlignment; }
            set {
                if (_textAlignment != value)
                {
                    _textAlignment = value;
                    InvalidateMesh();
                }
            }
        }

        public TextOverflowHorizontal HorizontalOverflow {
            get { return _horizontalOverflow; }
            set {
                if (_horizontalOverflow != value)
                {
                    _horizontalOverflow = value;
                    InvalidateMesh();
                }
            }
        }

        public TextOverflowVertical VerticalOverflow {
            get { return _verticalOveflow; }
            set {
                if (_verticalOveflow != value)
                {
                    _verticalOveflow = value;
                    InvalidateMesh();
                }
            }
        }

        public Label()
        {
            _textAlignment = TextAlignment.Left;
            _horizontalOverflow = TextOverflowHorizontal.Clip;
            _verticalOveflow = TextOverflowVertical.Clip;
            ColorMode = ColorMode.Override;
            _fontSize = 32f;
            _quadCount = -1;
        }

        private void InvalidateMesh()
        {
            _quadCount = -1;
            _textSize = Vector2.Zero;
        }

        protected override Vector2 MeasureOverride (in Vector2 available)
        {
            if (_font == null)
                return Vector2.Zero;

            // Cached text size?
            if (_textSize != Vector2.Zero)
                return _textSize;

            var scale = _fontSize / _font.Resolution;
            var height = _font.Height * scale;

            // Calcuate the text size
            _textSize = new Vector2(0, height);

            // Update the measure size
            float line_w = 0;
            float lineX = 0;
            int line_count = 1;
            char char_last = '\0';

            for (int i = 0; i < _text.Length; i++)
            {
                // Handle new line.
                if (_text[i] == '\n')
                {
                    _textSize.x = MathEx.Max(line_w, _textSize.x);
                    _textSize.y += height;
                    line_w = 0.0f;
                    lineX = 0.0f;
                    line_count++;
                    char_last = '\0';
                    continue;
                }

                // Skip the character if there is no glyph for it.
                var glyph = _font.GetGlyph(_text[i]);
                if (null == glyph)
                    continue;

                // Calculate the advance from the previous character to the 
                // current character.
                lineX += _font.GetAdvance(char_last, _text[i]) * scale;

                // Adjust line width
                line_w = lineX + (glyph.Bearing.x + glyph.Size.x) * scale;

                // Advance to the next character position
                char_last = _text[i];
            }

            if (line_w != 0f)
                _textSize.x = MathEx.Max(line_w, _textSize.x);

            return _textSize;
        }

        protected void UpdateMesh(Rect nrect)
        {
            if (_quadCount != -1)
                return;

            // Empty?
            if (_font == null)
            {
                _quadCount = 0;
                return;
            }

            // Cache ascent and height
            float scale = _fontSize / _font.Resolution;
            float height = _font.Height * scale;
            float ascent = _font.Ascent * scale;
            float sdf = 8.0f * scale; //  _font.Image.SignedDistanceFieldRange * scale;

            // If horizontal overflow is ellipsis we need to calculate the ellipsis size
            float ellipsisWidth = 0.0f;
            Font.Glyph dotGlyph = null;
            float dotAdvance = 0.0f;
            if (_horizontalOverflow == TextOverflowHorizontal.Ellipsis)
            {
                dotGlyph = _font.GetGlyph('.');
                dotAdvance = _font.GetAdvance('.', '.') * scale;
                if (null != dotGlyph)
                {
                    ellipsisWidth += dotAdvance * 2.0f;
                    ellipsisWidth += (dotGlyph.Bearing.x + dotGlyph.Size.x) * scale;
                }
            }

            //////////////////////////////////////////////////////////////////////////////
            // PASS1: Calculate the number of valid characters and lines in the text
            //////////////////////////////////////////////////////////////////////////////

            int characterCount = 0;
            int line_count = 1;
            int i;
            for (i = 0; i < _text.Length; i++)
            {
                // Special case new line..
                if (_text[i] == '\n')
                {
                    line_count++;
                    continue;
                }

                // Skip characters with no glyph in the font.
                var glyph = _font.GetGlyph(_text[i]);
                if (glyph == null)
                    continue;

                // Include this character.
                characterCount++;
            }

            // Add a character for each line for the EOL
            characterCount += line_count;

            // If horizontal overflow is an ellipsis then add additional characters to account for 
            // possible dots on every line (two per line since we know at least one character will be removed
            // and replaced with the ellipsis).
            if (_horizontalOverflow == TextOverflowHorizontal.Ellipsis)
                characterCount += (2 * line_count);

            //////////////////////////////////////////////////////////////////////////////
            // PASS2: Populate the Characters and handle horizontal clipping
            //////////////////////////////////////////////////////////////////////////////
            int lineIndex = 0;
            float lineX = 0.0f;
            float lineY = 0.0f;
            bool lineSkip = false;

            _characters = new Character[characterCount];
            characterCount = 0;

            // Ensure a single line will fit if clipping vertically..
            if (_verticalOveflow == TextOverflowVertical.Clip && height > nrect.height)
            {
                i = _text.Length;
            }
            else
            {
                i = 0;
            }

            char char_last = '\0';
            int char_render_count = 0;
            for (; i < _text.Length; i++)
            {
                // Special case newline.
                if (_text[i] == '\n')
                {
                    // Add EOL character
                    ref var eol = ref _characters[characterCount++];
                    eol.glyph = null;
                    eol.bounds.x = lineX + _font.GetAdvance(char_last, ' ') * scale;
                    eol.line = lineIndex;
                    eol.index = (int)i;

                    lineX = 0.0f;
                    lineY += height;

                    // We are done if already overflowed.
                    if (_verticalOveflow == TextOverflowVertical.Clip && lineY > nrect.height)
                        break;

                    lineIndex++;

                    lineSkip = false;
                    char_last = '\0';
                    continue;
                }

                // Are we skipping the remaining characters on the line?
                if (lineSkip)
                    continue;

                // Get character glyph
                var glyph = _font.GetGlyph(_text[i]);

                // Skip the glyph if it cant be renderered.
                if (glyph == null)
                    continue;

                // Calculate the advance from the previous character to the current.  We wait
                // to do this now because the line may have changed or a missing character
                // may have been found.
                float advance_x = _font.GetAdvance(char_last, _text[i]) * scale;

                // Did the glyph overflow?
                if (_horizontalOverflow != TextOverflowHorizontal.Overflow)
                {
                    if ((lineX + advance_x + (glyph.Bearing.x + glyph.Size.x) * scale) - nrect.width > 0.0001f)
                    {
                        switch (_horizontalOverflow)
                        {
                            case TextOverflowHorizontal.Wrap:
                            {
                                int cc = 0;
                                for (cc = characterCount; cc > 0 && _characters[cc - 1].line == lineIndex; cc--)
                                    if (_text[_characters[cc - 1].index] == ' ')
                                        break;

                                cc--;

                                if (_characters[cc].line == lineIndex && _text[_characters[cc].index] == ' ')
                                {
                                    // Add EOL character
                                    ref var eol = ref _characters[characterCount++];
                                    eol.glyph = null;
                                    eol.bounds.x = lineX + _font.GetAdvance(char_last, ' ') * scale;
                                    eol.line = lineIndex;
                                    eol.index = i;

                                    lineX = 0.0f;
                                    lineY += height;

                                    // We are done if already overflowed.
                                    if (_verticalOveflow == TextOverflowVertical.Clip && lineY > nrect.height)
                                    {
                                        break;
                                    }

                                    lineIndex++;
                                    i = cc - 1;
                                    characterCount = cc;

                                    lineSkip = false;
                                    char_last = '\0';
                                    continue;
                                }
                                break;
                            }

                            case TextOverflowHorizontal.Clip:
                                // Skip all remaining characters on the line
                                lineSkip = true;
                                continue;

                            case TextOverflowHorizontal.Ellipsis:
                            {
                                // Skip remainder of line.
                                lineSkip = true;

                                // If the ellipsis doesnt even fit all by itself then just render a blank line
                                if (ellipsisWidth > nrect.width)
                                {
                                    while (characterCount > 0 && _characters[characterCount - 1].line == lineIndex)
                                        characterCount--;
                                    continue;
                                }

                                // Pop characters until the ellipsis fits
                                while (characterCount > 0 && _characters[characterCount - 1].line == lineIndex)
                                {
                                    ref var back = ref _characters[characterCount - 1];
                                    lineX = back.bounds.x;
                                    lineX += _font.GetAdvance(_text[back.index], '.') * scale;
                                    if (lineX + ellipsisWidth < nrect.width)
                                        break;
                                    characterCount--;
                                }

                                // If no characters left on the line then render only the ellipsis
                                if (characterCount == 0 || _characters[characterCount - 1].line != lineIndex)
                                {
                                    lineX = 0.0f;
                                }

                                // Add the ellipsis dots..
                                for (int e = 0; e < 3; e++)
                                {
                                    ref var ellipsis = ref _characters[characterCount++];
                                    ellipsis.glyph = dotGlyph;
                                    ellipsis.line = lineIndex;
                                    ellipsis.bounds.x = lineX;
                                    ellipsis.bounds.width = glyph.Size.x * scale;
                                    ellipsis.index = -1;
                                    lineX += dotAdvance;
                                }

                                continue;
                            }

                            default:
                                throw new NotImplementedException();
                        }
                    }
                }

                // Remember the last character added
                char_last = _text[i];

                // Add the advance since we are committed to this glyph now
                lineX += advance_x;

                // Add a character
                ref var add = ref _characters[characterCount++];
                add.glyph = glyph;
                add.line = lineIndex;
                add.bounds.x = lineX;
                add.bounds.width = glyph.Size.x * scale;
                add.index = i;

                // If the character has a width then mark it to render.
                if (add.bounds.width > 0.0f)
                    char_render_count++;
            }

            // Add an EOL if not already added
            if (characterCount == 0 || _characters[characterCount - 1].glyph != null)
            {
                ref var eol = ref _characters[characterCount++];
                eol.glyph = null;
                eol.line = lineIndex;
                eol.bounds.x = lineX + _font.GetAdvance(char_last, ' ') * scale;
            }

            //////////////////////////////////////////////////////////////////////////////
            // PASS3: Populate Lines 
            //////////////////////////////////////////////////////////////////////////////

            // Resize the lines.
            _lines = new Line[lineIndex + 1];

            for (uint l = 0, c = 0; l < _lines.Length; l++, c++)
            {
                ref var line = ref _lines[l];

                line.bounds.x = 0.0f;

                // Skip empty lines..
                if (_characters[c].line != l)
                {
                    line.bounds.y = lineY;
                    line.bounds.height = l * height;
                    line.bounds.width = 0.0f;
                    line.charMin = 0xFFFFFFFF;
                    line.charMax = 0xFFFFFFFF;
                    continue;
                }

                // Set first character in the line.      
                line.charMin = c;

                // Find the last character in the line
                for (; c + 1 < characterCount && _characters[c + 1].line == l; c++)
                    ;

                // Set the last character in the line 
                line.charMax = c;

                // Set line bounds.
                line.bounds.width = _characters[c].bounds.x;
                line.bounds.y = l * height;
                line.bounds.height = height;
            }

            // Remove the line spacing from the height of the last line.
            ref var lastLine = ref _lines[_lines.Length - 1];

            switch (_textAlignment)
            {
                case TextAlignment.Right:
                    for (uint li = 0; li < _lines.Length; li++)
                    {
                        ref var line = ref _lines[li];
                        line.bounds.x += (nrect.width - line.bounds.width);
                    }
                    break;

                case TextAlignment.Center:
                    for (uint li = 0; li < _lines.Length; li++)
                    {
                        ref var line = ref _lines[li];
                        line.bounds.x += (nrect.width - line.bounds.width) * 0.5f;
                    }
                    break;

                case TextAlignment.Left:
                    break;

                default:
                    throw new NotImplementedException();
            }

            //////////////////////////////////////////////////////////////////////////////
            // PASS4: Populate verts_ and tris_
            //////////////////////////////////////////////////////////////////////////////

            // Allocate a new mesh if there was no mesh or the counts change.
            int char_vertex_count = char_render_count * 4;
            int char_triangle_count = char_render_count * 2;

            // New mesh if there is no mesh or the size changed.
            if (null == _quads || char_render_count > _quads.Length)
                _quads = new Quad[char_render_count];

            _quadCount = 0;

            for (int li = 0; li < _lines.Length; li++)
            {
                ref var line = ref _lines[li];

                // Skip empty lines.
                if (line.charMin == 0xFFFFFFFF)
                    continue;

                // Add the parent rectangle into the line bounds.
                line.bounds.x += nrect.x;
                line.bounds.y += nrect.y;

                // Process each charcter in the line
                for (uint cc = line.charMin; cc <= line.charMax; cc++)
                {
                    ref var c = ref _characters[cc];

                    // Adjust the character bounds to be relative to the line
                    c.bounds.x += line.bounds.x;
                    c.bounds.y += line.bounds.y;
                    c.bounds.height = line.bounds.height;

                    if (c.glyph != null)
                    {
                        // Set glyph specific bounds values
                        c.bounds.x += c.glyph.Bearing.x * scale;
                        c.bounds.width = c.glyph.Size.x * scale;

                        // Calculate four corners for glyph quad
                        if (c.glyph.S != c.glyph.T)
                        {
                            float l = c.bounds.x - sdf;
                            float r = l + ((c.glyph.Size.x) * scale) + sdf * 2f;
                            float t = c.bounds.y + ascent - c.glyph.Bearing.y * scale - sdf;
                            float b = t + (c.glyph.Size.y * scale) + sdf * 2f;

                            _quads[_quadCount++] =
                                new Quad
                                {
                                    TL = new Vertex { XY = new Vector2(l, t), UV = new Vector2(c.glyph.S.x, c.glyph.S.y), Color = Color.White },
                                    TR = new Vertex { XY = new Vector2(r, t), UV = new Vector2(c.glyph.T.x, c.glyph.S.y), Color = Color.White },
                                    BL = new Vertex { XY = new Vector2(l, b), UV = new Vector2(c.glyph.S.x, c.glyph.T.y), Color = Color.White },
                                    BR = new Vertex { XY = new Vector2(r, b), UV = new Vector2(c.glyph.T.x, c.glyph.T.y), Color = Color.White }
                                };
                        }
                    }

                    // If there is a previous character then adjust the character bounds 
                    // to ensure they are both touching each other.  Do this by averaging 
                    // the whitespace between the two characters.
                    if (cc != line.charMin)
                    {
                        ref var p = ref _characters[cc - 1];
                        float p_max = p.bounds.x + p.bounds.width;
                        float c_min = c.bounds.x;
                        float avg = (p_max + c_min) * 0.5f;
                        p.bounds.width = avg - p.bounds.x;
                        c.bounds.width = (c.bounds.x + c.bounds.width) - avg;
                        c.bounds.x = avg;
                    }
                }
            }
        }

        public int GetLineFromCharIndex(int index) => _characters[index].line;

        public int GetLastCharIndexFromLine(int line) => (int)_lines[line].charMax;

        public Vector2 GetPositionFromCharIndex(int index)
        {
            // If before the first character return the minimum position of the first line
            if (index < 0)
                return _lines[0].bounds.TopLeft;

            // If past the last character return the end of the last line
            if (index >= _characters.Length)
            {
                ref var line = ref _lines[_lines.Length - 1];
                return new Vector2(line.bounds.x + line.bounds.width, line.bounds.y);
            }

            // Return the character min      
            return _characters[index].bounds.TopLeft;
        }

        public int GetCharIndexFromPosition(in Vector2 pos)
        {
            // Find the matching line vertically
            uint l = 0;
            for (; l < _lines.Length; l++)
            {
                ref var temp = ref _lines[l];
                if (pos.y >= temp.bounds.y && pos.y <= temp.bounds.y + temp.bounds.height)
                    break;
            }

            // Return the last character which is EOL if past the last line.
            if (l >= _lines.Length)
                return _characters.Length - 1;

            ref var line = ref _lines[l];

            // If the chacter is before the line horizontally return the first character in the line
            if (pos.x <= line.bounds.x)
                return (int)line.charMin;

            // If the position is within the last character or after it then return the last character
            if (pos.x >= _characters[line.charMax].bounds.x)
                return (int)line.charMax + 1;

            // Find the character within the line.
            for (uint c = line.charMin; c < line.charMax; c++)
            {
                float min = _characters[c].bounds.x;
                float max = min + _characters[c].bounds.width;
                if (pos.x >= min && pos.x <= max)
                    return (int)c;
            }

            return (int)line.charMax;
        }

        public override void Draw (GraphicsContext gc)
        {
            UpdateMesh(Rect);
            if (_quadCount <= 0)
                return;

            gc.Color = Color;
            gc.Image = _font.Image;
            gc.MaskMode = MaskMode;
            gc.Draw(_quads, 0, _quadCount);
        }

        protected override void OnRectChanged(in Rect rect)
        {
            base.OnRectChanged(rect);
            InvalidateMesh();
        }
    }
}
