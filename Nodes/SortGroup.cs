using System;
using System.Collections.Generic;
using System.Text;

namespace NoZ
{
    public class SortGroup : Node, ILayer
    {
        public int SortOrder { get; set; }

        public void BeginLayer(GraphicsContext gc)
        {
        }

        public void EndLayer(GraphicsContext gc)
        {
        }
    }
}
