/*

    Copyright (c) 2024 NoZ Games, LLC. All rights reserved.

*/

using System.Numerics;
using Raylib_cs;

namespace NoZ.UI
{
    public static partial class UI
    {
        private const float BorderSegmentsPerUnit = 0.75f;
        
        private static void Render(Span<Element> elements)
        {
            foreach (var element in elements)
                Render(element);
        }
        
        private static void Render(in Element element)
        {
            var scaledRect = new Rect(
                element.Bounds.X * Scale,
                element.Bounds.Y * Scale,
                element.Bounds.Width * Scale,
                element.Bounds.Height * Scale);

            if (element.Style.BackgroundColor.A > 0)
            {
                if (element.Style.BorderRadius > float.Epsilon)
                {
                    var radius = element.Style.BorderRadius * Scale;
                    var width = scaledRect.Width;
                    var height = scaledRect.Height;
                    var roundness = radius / (width > height ? height : width);

                    Raylib.DrawRectangleRounded(
                        scaledRect,
                        roundness,
                        (int)(radius * BorderSegmentsPerUnit),
                        element.Style.BackgroundColor);
                }
                else
                    Raylib.DrawRectangleRec(scaledRect, element.Style.BackgroundColor);
            }

            if (element.Style.BorderColor.A > 0 && element.Style.BorderWidth > float.Epsilon)
            {
                Raylib.DrawRectangleLinesEx(
                    scaledRect,
                    element.Style.BorderWidth * Scale,
                    element.Style.BorderColor);
            }
            
            switch(element.ElementType)
            {
                case ElementType.Label:
                    DrawText(element, scaledRect);
                    break;
                
                case ElementType.Image:
                    DrawImage(element, scaledRect);
                    break;
                
                case ElementType.Horizontal:
                case ElementType.Vertical:
                case ElementType.Element:
                    break;
            }
        }
        
        private static void DrawImage(in Element element, in Rect rect)
        {
            var image = _images[element.Resource - 1];
            var dest = new Rect(rect.X, rect.Y, rect.Width, rect.Height);

            var texture = image.Texture ?? image.Sprite.Texture;
            var source = image.Texture != null
                ? new Rect(0, 0, texture.Width, texture.Height)
                : image.Sprite.SourceRect;

            if (element.Style.ImageScaleMode == ScaleMode.ScaleToFit)
            {
                if (source.Width > source.Height)
                {
                    var aspect = (float)source.Height / source.Width;
                    var height = aspect * dest.Width;
                    dest.Y += (dest.Height - height) * 0.5f;
                    dest.Height = height;
                }
                else
                {
                    var aspect = (float)source.Width / source.Height;
                    var width = aspect * dest.Height;
                    dest.X += (dest.Width - width) * 0.5f;
                    dest.Width = width;
                }
            }

            if (element.Style.ImageFlip)
                source.Width = -source.Width;
            
            Raylib.DrawTexturePro(texture._texture, source, dest, new Vector2(0, 0), 0, element.Style.ImageTint);
        }

        private static bool TryMeasureElement(in Element element, in Vector2 size, out Vector2 measuredSized)
        {
            switch(element.ElementType)
            {
                case ElementType.Label:
                    // TODO: use size to handle wrapping
                    measuredSized = MeasureText(element);
                    return true;

                case ElementType.Image:
                    measuredSized = MeasureImage(element);
                    return true;

                case ElementType.Horizontal:
                case ElementType.Vertical:
                case ElementType.Element:
                    measuredSized = Vector2.Zero;
                    return false;

                default:
                    throw new NotImplementedException();
            }
        }

        private static Vector2 MeasureImage(in Element element)
        {
            var rect = _images[element.Resource - 1].Source;
            return new Vector2(rect.Width, rect.Height);
        }

        private static Vector2 MeasureText(in Element element)
        {
            var text = _text[element.Resource - 1];
            var font = GetFont(element.Style.Font);
            var fontSize = element.Style.FontSize * Scale;
            return Raylib.MeasureTextEx(font._font, text, fontSize, 0);
        }

        private static void DrawText(in Element element, in Rect rect)
        {
            var text = _text[element.Resource - 1];
            var font = GetFont(element.Style.Font);
            var fontSize = element.Style.FontSize * Scale;
            var textSize = Raylib.MeasureTextEx(font._font, text, fontSize, 0);
            var color = element.Style.Color;
            var textOutlineWidth = element.Style.TextOutlineWidth * UI.Scale;
            var textOutlineColor = element.Style.TextOutlineColor;

            Raylib.BeginShaderMode(_textShader!._shader);
            
            switch (element.Style.TextAlignment)
            {
                case TextAlignment.TopLeft:
                    Raylib.DrawTextEx(font._font, text, new Vector2(rect.X, rect.Y), fontSize, 0, color);
                    break;
                
                case TextAlignment.Middle:
                    Raylib.DrawTextEx(
                        font._font,
                        text,
                        new Vector2(
                            rect.X + (rect.Width - textSize.X) * 0.5f,
                            rect.Y + (rect.Height - textSize.Y) * 0.5f),
                        fontSize,
                        0,
                        color);
                    
                    break;
                
                case TextAlignment.MiddleLeft:
                    Raylib.DrawTextEx(
                        font._font,
                        text,
                        new Vector2(
                            rect.X,
                            rect.Y + (rect.Height - textSize.Y) * 0.5f),
                        fontSize,
                        0,
                        color);
                    
                    break;

                case TextAlignment.BottomLeft:
                    Raylib.DrawTextEx(
                        font._font,
                        text,
                        new Vector2(
                            rect.X,
                            rect.Y + rect.Height - textSize.Y),
                        fontSize,
                        0,
                        color);
                    break;

                case TextAlignment.BottomRight:
                    Raylib.DrawTextEx(
                        font._font,
                        text,
                        new Vector2(
                            rect.X + rect.Width - textSize.X,
                            rect.Y + rect.Height - textSize.Y),
                        fontSize,
                        0,
                        color);
                    break;                

                case TextAlignment.TopMiddle:
                    DrawText(
                        font._font,
                        text,
                        new Vector2(
                            rect.X + (rect.Width - textSize.X) * 0.5f - 5 * UI.Scale,
                            rect.Y),
                        fontSize,
                        color,
                        textOutlineWidth,
                        textOutlineColor);
                    break;                
                
            }
            
            Raylib.EndShaderMode();
        }

        private static void DrawText(Font font, string text, Vector2 position, float fontSize, Color color, float outlineWidth, Color outlineColor)
        {
            if (outlineWidth > 0)
            {
                Raylib.DrawTextEx(font, text, new Vector2(position.X - outlineWidth, position.Y), fontSize, 0, outlineColor);
                Raylib.DrawTextEx(font, text, new Vector2(position.X + outlineWidth, position.Y), fontSize, 0, outlineColor);
                Raylib.DrawTextEx(font, text, new Vector2(position.X, position.Y - outlineWidth), fontSize, 0, outlineColor);
                Raylib.DrawTextEx(font, text, new Vector2(position.X, position.Y + outlineWidth), fontSize, 0, outlineColor);

                Raylib.DrawTextEx(font, text, new Vector2(position.X - outlineWidth, position.Y - outlineWidth), fontSize, 0, outlineColor);
                Raylib.DrawTextEx(font, text, new Vector2(position.X + outlineWidth, position.Y - outlineWidth), fontSize, 0, outlineColor);
                Raylib.DrawTextEx(font, text, new Vector2(position.X - outlineWidth, position.Y + outlineWidth), fontSize, 0, outlineColor);
                Raylib.DrawTextEx(font, text, new Vector2(position.X + outlineWidth, position.Y + outlineWidth), fontSize, 0, outlineColor);
            }

            Raylib.DrawTextEx(font, text, position, fontSize, 0, color);
        }
    }
}