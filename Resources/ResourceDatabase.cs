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
using System.Reflection;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace NoZ
{
    public class ResourceDatabase
    {
        /// <summary>
        /// Cached resources by resource name
        /// </summary>
        private Dictionary<string, Resource> _cache;

        /// <summary>
        /// List of archives that supply resources to the database.
        /// </summary>
        private ResourceArchive[] _archives;

        /// <summary>
        /// Stopwatched used to time loading of resources.
        /// </summary>
        private Stopwatch _stopwatch;

        private Dictionary<string, MethodInfo> _createMethods;


        public ResourceDatabase(ResourceArchive[] archives)
        {
            _stopwatch = new Stopwatch();
            _cache = new Dictionary<string, Resource>();
            _createMethods = new Dictionary<string, MethodInfo>();
            _archives = archives;
        }

        /// <summary>
        /// Opens a read-only stream from for the given name from the first archive 
        /// that contains the named resource.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private Stream OpenRead(string name, FieldInfo info)
        {
            foreach (var archive in _archives)
            {
                var stream = archive.OpenRead(name, info);
                if (null != stream)
                    return stream;
            }

            return null;
        }

        /// <summary>
        /// Load assets from static fields in the given type
        /// </summary>
        /// <param name="type"></param>
        public void Load(Type type)
        {
            // Only static classes allowed
            if (!type.IsClass || !(type.IsAbstract && type.IsSealed))
                return;

            // Only check for static fields
            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                var import = field.GetCustomAttribute<ImportAttribute>();
                if (null == import)
                    continue;

                if (null != field.GetValue(null))
                    throw new InvalidOperationException($"resource '{field.Name}' already loaded");

                // Open a stream to the input file.
                using (var stream = OpenRead(import.Name, field))
                using (var reader = new BinaryReader(stream)) 
                {
                    var typeName = reader.ReadString();
                    if (field.FieldType.FullName!= typeName)
                        throw new InvalidOperationException($"{field.Name}: type mismatch '{field.FieldType.FullName}' / '{typeName}'");

                    var resource = CreateResource(import.Name, typeName, reader);
                    _cache.Add(import.Name, resource);
                    field.SetValue(null, resource);
                }
            }
        }

        private Resource CreateResource(string name, string typeName, BinaryReader reader)
        {
            if (!_createMethods.TryGetValue(typeName, out var method))
            {
                var type = Type.GetType(typeName);
                method = type.GetMethod("Create", new Type[] { typeof(string), typeof(BinaryReader) } );
                if(null == method || method.ReturnType.FullName != typeName || !method.IsStatic)
                    throw new InvalidOperationException($"Missing method with signature 'static TypeName {typeName}.Create(string,BinaryReader)");

                _createMethods.Add(typeName, method);
            }

            return method.Invoke(null, new object[] { name, reader }) as Resource;
        }
    }
}

