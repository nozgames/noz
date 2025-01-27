/*

    Copyright (c) 2024 NoZ Games, LLC. All rights reserved.

*/

using System.Numerics;
using System.Runtime.CompilerServices;
using Raylib_cs;

namespace NoZ.UI
{
    public static partial class UI
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ResolveFlexMargins(in StyleLength marginMin, in StyleLength marginMax, ref float min, ref float max, float size)
        {
            var contrib = 0.0f;
            if (marginMin.IsFlex)
                contrib += marginMin.Value;
            if (marginMax.IsFlex)
                contrib += marginMax.Value;

            if (contrib < float.Epsilon)
                return;

            var amount = (max - min) - size;
            var contribInv = 1.0f / contrib;
            if (marginMin.IsFlex)
                min += amount * marginMin.Value * contribInv;
            if (marginMax.IsFlex)
                max -= amount * marginMax.Value * contribInv;
        }
        
        private static void UpdateElementLayout()
        {
            if (_stack.Count == 0)
                return;

            ref var parent = ref _elements[_stack.Peek()];
            ref var element = ref _elements[_currentElement];
                
            var pbounds = parent.LayoutBounds;
            var pl = pbounds.X;
            var pt = pbounds.Y;
            var pr = pbounds.X + pbounds.Width;
            var pb = pbounds.Y + pbounds.Height;
            
            var ml = element.Style.MarginLeft.Evaluate(pbounds.Width);
            var mt = element.Style.MarginTop.Evaluate(pbounds.Height);
            var mr = element.Style.MarginRight.Evaluate(pbounds.Width);
            var mb = element.Style.MarginBottom.Evaluate(pbounds.Height);

            pl += ml;
            pt += mt;
            pr -= mr;
            pb -= mb;

            var ew = element.Style.Width.Evaluate(pr - pl);
            var eh = element.Style.Height.Evaluate(pb - pt);

            // Special case for automatic calculation of images and labels
            if (element.Style.Width.IsAuto || element.Style.Height.IsAuto)
            {
                if (TryMeasureElement(element, new Vector2(ew,eh), out var measuredSize))
                {
                    if (element.Style.Width.IsAuto)                        
                        ew = MathF.Min(measuredSize.X, ew);

                    if (element.Style.Height.IsAuto)
                        eh = MathF.Min(measuredSize.Y, eh);
                }
            }

            var marginLeft = parent.ElementType == ElementType.HorizontalReverse
                ? StyleLength.Flex(1)
                : element.Style.MarginLeft;
            ResolveFlexMargins(marginLeft, element.Style.MarginRight, ref pl, ref pr, ew);
            ResolveFlexMargins(element.Style.MarginTop, element.Style.MarginBottom, ref pt, ref pb, eh);

            if (element.Style.Width.IsFlex)
                ew = pr - pl;
            
            if (element.Style.Height.IsFlex)
                eh = pb - pt;
            
            element.Bounds = new Rect(pl, pt, ew, eh);
            element.BoundsWithMargins = new Rect(pl - ml, pt - mt, ew + ml + mr, eh + mt + mb);
            element.LayoutBounds = element.Bounds;
        }
    }
}
