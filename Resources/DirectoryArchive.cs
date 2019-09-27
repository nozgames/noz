/*
  NoZ Game Engine

  Copyright(c) 2017 NoZ Games, LLC

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
using System.Reflection;
using System.IO;

namespace NoZ
{
    public class DirectoryArchive : ResourceArchive
    {
        public DirectoryInfo Directory { get; private set; }

        public DirectoryArchive(string path, ResourceArchiveMode mode) : base(mode)
        {
            // Make sure the directory exists.
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);

            Directory = new DirectoryInfo(path);
        }

        public override Stream OpenRead(string name, FieldInfo fieldInfo)
        {
            return File.OpenRead(Path.Combine(Directory.FullName, name));
        }
    }
}
