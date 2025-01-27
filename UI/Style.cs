/*

    Copyright (c) 2024 NoZ Games, LLC. All rights reserved.

*/

using System.Runtime.CompilerServices;
using Raylib_cs;

namespace NoZ.UI
{
    public enum LengthUnit
    {
        Fixed,
        Percent,
        Flex
    }

    public enum StyleKeyword
    {
        Inherit,
        Overwrite,
        Auto
    }

    public struct StyleLength
    {
        public static readonly StyleLength Auto = new() { Unit = LengthUnit.Fixed, Value = 0, Keyword = StyleKeyword.Auto };
        public static readonly StyleLength Stretch = new() { Unit = LengthUnit.Flex, Value = 1, Keyword = StyleKeyword.Inherit };

        public StyleKeyword Keyword;
        public LengthUnit Unit;
        public float Value;
        
        public bool IsFlex => Unit == LengthUnit.Flex;

        public bool IsAuto => Keyword == StyleKeyword.Auto;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator StyleLength(float value) => Fixed(value); 
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StyleLength Percent(float value) => new() { Unit = LengthUnit.Percent, Value = value, Keyword = StyleKeyword.Overwrite};
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StyleLength Fixed(float value) => new() { Unit = LengthUnit.Fixed, Value = value, Keyword = StyleKeyword.Overwrite };
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StyleLength Flex(float value) => new() { Unit = LengthUnit.Flex, Value = value, Keyword = StyleKeyword.Overwrite };

        public float Evaluate(float parentValue)
        {
            if (IsAuto)
                return parentValue;

            switch (Unit)
            {
                case LengthUnit.Fixed:
                    return Value;
                
                case LengthUnit.Percent:
                    return parentValue * Value;
                
                default:
                    return 0;
            }
        }
    }

    public enum TextAlignment
    {
        TopLeft,
        TopMiddle,
        MiddleLeft,
        BottomLeft,
        Middle,
        TopRight,
        MiddleRight,
        BottomRight,
    }

    public enum ScaleMode
    {
        ScaleToFit,
        StretchToFill,
        ScaleAndCrop,
    }

    public struct Style
    {
        public StyleLength Width;
        public StyleLength Height;
        public Color BackgroundColor;
        public Color Color;
        public float FontSize;
        public float BorderRadius;
        public float BorderWidth;
        public Color BorderColor;
        public int Font;
        public Color ImageTint;
        public ScaleMode ImageScaleMode;
        public bool ImageFlip;
        public TextAlignment TextAlignment;
        public float TextOutlineWidth;
        public Color TextOutlineColor;
        public StyleLength MarginTop;
        public StyleLength MarginLeft;
        public StyleLength MarginBottom;
        public StyleLength MarginRight;
        
        public static Style Default => new Style
        {
            Width = StyleLength.Fixed(0),
            Height = StyleLength.Fixed(0),
            MarginLeft = StyleLength.Fixed(0),
            MarginTop = StyleLength.Fixed(0),
            MarginRight = StyleLength.Fixed(0),
            MarginBottom = StyleLength.Fixed(0),
            ImageTint = Color.White,
        };
        
        public static readonly Style StretchToFit = new Style
        {
            Width = StyleLength.Flex(1),
            Height = StyleLength.Flex(1)
        };

        public void Apply(in Style style)
        {
            if (style.MarginTop.Keyword == StyleKeyword.Overwrite)
                MarginTop = style.MarginTop;
            if (style.MarginLeft.Keyword == StyleKeyword.Overwrite)
                MarginLeft = style.MarginLeft;
            if (style.MarginBottom.Keyword == StyleKeyword.Overwrite)
                MarginBottom = style.MarginBottom;
            if (style.MarginRight.Keyword == StyleKeyword.Overwrite)
                MarginRight = style.MarginRight;
            if (style.Width.Keyword == StyleKeyword.Overwrite)
                Width = style.Width;
            if (style.Height.Keyword == StyleKeyword.Overwrite)
                Height = style.Height;
        }
    }

    public static class StyleExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Style Width(this Style s, float value)
        {
            s.Width = StyleLength.Fixed(value);
            return s;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Style Width(this Style s, in StyleLength value)
        {
            s.Width = value;
            return s;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Style Height(this Style s, float value)
        {
            s.Height = StyleLength.Fixed(value);
            return s;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Style Height(this Style s, in StyleLength height)
        {
            s.Height = height;
            return s;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Style MarginTop(this Style s, in StyleLength value)
        {
            s.MarginTop = value;
            return s;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Style MarginLeft(this Style s, in StyleLength value)
        {
            s.MarginLeft = value;
            return s;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Style MarginBottom(this Style s, in StyleLength value)
        {
            s.MarginBottom = value;
            return s;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Style MarginRight(this Style s, in StyleLength value)
        {
            s.MarginRight = value;
            return s;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Style Color(this Style s, in Color value)
        {
            s.Color = value;
            return s;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Style BackgroundColor(this Style s, in Color value)
        {
            s.BackgroundColor = value;
            return s;
        }
        
        public static Style ImageTint(this Style s, in Color value)
        {
            s.ImageTint = value;
            return s;
        }
        
        public static Style Margin(this Style s, int size) => s.MarginTop(size).MarginLeft(size).MarginBottom(size).MarginRight(size);

        public static Style Margin(this Style style, in StyleLength length) => style
            .MarginTop(length)
            .MarginLeft(length)
            .MarginBottom(length)
            .MarginRight(length);

        public static Style FontSize(this Style style, int size)
        {
            style.FontSize = size;
            return style;
        }
    }
}
