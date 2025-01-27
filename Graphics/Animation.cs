/*

    Copyright (c) 2024 NoZ Games, LLC. All rights reserved.

*/


using System.Runtime.InteropServices;

namespace NoZ.Graphics
{   
    public struct KeyFrame
    {
        public Sprite Sprite;
        public int Hold;
        public int Event;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Animation
    {
        public readonly static int FrameOffset = Marshal.OffsetOf<Animation>("Frame1").ToInt32();

        public int FrameCount;
        public float Length;
        public float Speed;
        public KeyFrame Frame1;
        public KeyFrame Frame2;
        public KeyFrame Frame3;
        public KeyFrame Frame4;
        public KeyFrame Frame5;
        public KeyFrame Frame6;
        public KeyFrame Frame7;
        public KeyFrame Frame8;
    }
}
