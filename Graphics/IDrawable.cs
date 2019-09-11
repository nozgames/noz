using System;
using System.Collections.Generic;
using System.Text;

using NoZ.Graphics;

namespace NoZ {

    public interface IDrawable {
        bool Draw(GraphicsContext context);

        /// <summary>
        /// Returns the sort order within the current layer.  
        /// </summary>
        int SortOrder { get; }
    }

}
