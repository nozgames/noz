using System;
using System.Collections.Generic;
using System.Text;

namespace NoZ
{
    public class SortGroup : Node
    {
        public SortGroup()
        {
            IsDrawable = true;
        }

        public int SortOrder { get; set; }

        public int SortLayer { get; set; }

        public override void Draw (GraphicsContext gc)
        {
            gc.PushSortGroup();
            gc.SortOrder = (short)SortOrder;
            gc.SortLayer = (byte)SortLayer;
        }

        public override void DrawEnd (GraphicsContext gc)
        {
            gc.PopSortGroup();
        }
    }
}
