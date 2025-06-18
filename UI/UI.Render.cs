/*

    Copyright (c) 2024 NoZ Games, LLC. All rights reserved.

*/

using System.Numerics;
using NoZ.Graphics;

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

                    // Stubbed logic for SDL3 migration.
                }
                else
                {
                    // Stubbed logic for SDL3 migration.
                }
            }

            if (element.Style.BorderColor.A > 0 && element.Style.BorderWidth > float.Epsilon)
            {
                // Stubbed logic for SDL3 migration.
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
            
            // Stubbed logic for SDL3 migration.
        }

        private static bool TryMeasureElement(in Element element, in Vector2 size, out Vector2 measuredSized)
        {
            switch(element.ElementType)
            {
                case ElementType.Label:
                    // Stubbed logic for SDL3 migration.
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
            // Stubbed logic for SDL3 migration.
            return Vector2.Zero;
        }

        private static void DrawText(in Element element, in Rect rect)
        {
            var text = _text[element.Resource - 1];
            var font = GetFont(element.Style.Font);
            var fontSize = element.Style.FontSize * Scale;
            var textSize = Vector2.Zero; // Stubbed logic for SDL3 migration.
            var color = element.Style.Color;
            var textOutlineWidth = element.Style.TextOutlineWidth * UI.Scale;
            var textOutlineColor = element.Style.TextOutlineColor;

            // Stubbed logic for SDL3 migration.
            
            switch (element.Style.TextAlignment)
            {
                case TextAlignment.TopLeft:
                    // Stubbed logic for SDL3 migration.
                    break;
                
                case TextAlignment.Middle:
                    // Stubbed logic for SDL3 migration.
                    break;
                
                case TextAlignment.MiddleLeft:
                    // Stubbed logic for SDL3 migration.
                    break;

                case TextAlignment.BottomLeft:
                    // Stubbed logic for SDL3 migration.
                    break;

                case TextAlignment.BottomRight:
                    // Stubbed logic for SDL3 migration.
                    break;                

                case TextAlignment.TopMiddle:
                    // Stubbed logic for SDL3 migration.
                    break;                
                
            }
            
            // Stubbed logic for SDL3 migration.
        }

        private static void DrawText(Font font, string text, Vector2 position, float fontSize, Color color, float outlineWidth, Color outlineColor)
        {
            if (outlineWidth > 0)
            {
                // Stubbed logic for SDL3 migration.
            }

            // Stubbed logic for SDL3 migration.
        }
    }
}