/*

    Copyright (c) 2024 NoZ Games, LLC. All rights reserved.

*/

#if false
using NoZ.ECS;

namespace NoZ.Renderer
{
    public struct Animator : IComponent<Animator>
    {
        public int Frame;
        public float Elapsed;
        public bool Loop;
        public bool Playing;
        public float Speed;
        public Animation Animation;
        public int Event;
    }
}


#endif