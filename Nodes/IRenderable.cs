/*

    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

namespace NoZ
{
    /// <summary>
    /// Defines an object as renderable which ensures it's Render method is called
    /// when the object is in the world and enabled
    /// </summary>
    public interface IRenderable
    {
        public void Render();
    }
}
