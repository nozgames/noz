﻿using System;
using System.Collections.Generic;
using System.Text;

namespace NoZ
{
    public interface IPhysicsDriver
    {
        IWorld CreateWorld();
    }
}

