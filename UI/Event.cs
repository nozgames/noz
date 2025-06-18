/*

    Copyright (c) 2024 NoZ Games, LLC. All rights reserved.

*/

namespace NoZ.UI
{
    public enum EventType
    {
        Layout,
        Render,
    }
    
    public struct Event
    {
        public EventType Type;
        public bool IsMouseButton;
        public int MouseButton;
        
        public static Event Current;

        // TODO: Remove Raylib_cs dependencies and implement SDL3 event logic as needed.
    }
}