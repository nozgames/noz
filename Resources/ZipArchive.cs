/*
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

    public class ZipArchive : ResourceArchive
    {
        private System.IO.Compression.ZipArchive _archive;
        private string _path;

        /// <summary>
        /// Constructs a zip archive using the given path in ReadOnly mode.
        /// </summary>
        /// <param name="path"></param>
        public ZipArchive(string path) : this(path, ResourceArchiveMode.ReadOnly)
        {
            _path = path;
        }

        public ZipArchive(string path, ResourceArchiveMode mode) : base(mode)
        {
            switch (mode)
            {
                case ResourceArchiveMode.ReadOnly:
                {
                    _archive = new System.IO.Compression.ZipArchive(File.OpenRead(path), System.IO.Compression.ZipArchiveMode.Read);
                    break;
                }

                case ResourceArchiveMode.ReadWrite:
                case ResourceArchiveMode.WriteOnly:
                {
                    _archive = new System.IO.Compression.ZipArchive(new FileStream(path, FileMode.Create, FileAccess.ReadWrite), System.IO.Compression.ZipArchiveMode.Update);
                    break;
                }
            }
        }

        public override Stream OpenRead(string name)
        {
            var entry = _archive.GetEntry(name);
            if (null == entry)
                return null;

            using (var stream = entry.Open())
            {
                var mem = new MemoryStream((int)entry.Length);
                stream.CopyTo(mem);
                mem.Position = 0;
                return mem;
            }
        }

#if false
        public override Stream OpenWrite(string name)
        {
            var entry = _archive.GetEntry(name);
            if (null == entry)
                entry = _archive.CreateEntry(name);

            return entry.Open();
        }
#endif

        public override void Dispose()
        {
            base.Dispose();

            _archive?.Dispose();
            _archive = null;
        }
    }
}
