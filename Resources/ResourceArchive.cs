﻿/*
  NoZ Game Engine

  Copyright(c) 2019 NoZ Games, LLC

  Permission is hereby granted, free of charge, to any person obtaining a copy
  of this software and associated documentation files(the "Software"), to deal
  in the Software without restriction, including without limitation the rights
  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
  copies of the Software, and to permit persons to whom the Software is
  furnished to do so, subject to the following conditions :

  The above copyright notice and this permission notice shall be included in all
  copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
  SOFTWARE.
*/

using System;
using System.IO;
using System.Reflection;

namespace NoZ
{

    public enum ResourceArchiveMode
    {
        /// <summary>
        /// Opens the archive in read only mode
        /// </summary>
        ReadOnly,

        /// <summary>
        /// Opens the archive in write only mode
        /// </summary>
        WriteOnly,

        /// <summary>
        /// Opens the archive in read/write mode
        /// </summary>
        ReadWrite
    }

    public abstract class ResourceArchive : IDisposable
    {
        public ResourceArchiveMode Mode { get; private set; }

        public ResourceArchive(ResourceArchiveMode mode)
        {
            Mode = mode;
        }

        public abstract Stream OpenRead(string name, FieldInfo fieldInfo);

        public virtual void Dispose() { }
    }
}