using System.Collections.Generic;

namespace NoZ
{
    public class Scene : Node
    {
        public Vector2 Size { get; set; }

        protected override Rect CalculateFrame() => new Rect(Position, Size);
    }
}
