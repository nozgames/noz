/*

    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

namespace NoZ
{
    /// <summary>
    /// Defines an object as updatable which ensures it's Update method is called
    /// when the object is in the world and enabled
    /// </summary>
    public interface IUpdatable
    {
        public void Update();
    }
}
