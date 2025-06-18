/*

    Copyright (c) 2024 NoZ Games, LLC. All rights reserved.

*/

namespace NoZ.UI
{
    internal enum ElementType
    {
        Element,
        Image,
        Label,
        Horizontal,
        HorizontalReverse,
        Vertical
    }

    internal struct Element
    {
        public ElementType ElementType;
        public Rect Bounds;
        public Rect LayoutBounds;
        public Rect BoundsWithMargins;
        public Style Style;
        public int Resource;
        public int ControlId;
    }

    // TODO: Remove Raylib_cs dependencies and implement SDL3 rendering logic as needed.
}
