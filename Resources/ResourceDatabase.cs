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
    public static class ResourceDatabase
    {
        /// <summary>
        /// Cached resources by resource name
        /// </summary>
        private static Dictionary<string, Resource> _cache;

        /// <summary>
        /// List of archives that supply resources to the database.
        /// </summary>
        private static ResourceArchive[] _archives;

        /// <summary>
        /// Stopwatched used to time loading of resources.
        /// </summary>
        private static Stopwatch _stopwatch;

        private static Dictionary<string, MethodInfo> _createMethods;


        static ResourceDatabase ()
        {
            _stopwatch = new Stopwatch();
            _cache = new Dictionary<string, Resource>();
            _createMethods = new Dictionary<string, MethodInfo>();
            _archives = new ResourceArchive[] { };
        }

        /// <summary>
        /// Add a new archive to the resource database
        /// </summary>
        /// <param name="archive"></param>
        public static void AddArchive (ResourceArchive archive)
        {
            var list = new List<ResourceArchive>(_archives);
            list.Add(archive);
            _archives = list.ToArray();
        }

        /// <summary>
        /// Opens a read-only stream from for the given name from the first archive 
        /// that contains the named resource.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static Stream OpenRead(string name, FieldInfo info)
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
        /// Load a named asseet
        /// </summary>
        /// <typeparam name="T">Type of asset expected</typeparam>
        /// <param name="name">Name of asset</param>
        public static T Load<T>(string name) where T : Resource
        {
            // Cached?
            if(_cache.TryGetValue(name, out var cached))
            {
                var result = cached as T;
                if(null == result)
                    throw new InvalidOperationException($"{name}: type mismatch '{typeof(T).FullName}' / '{result.GetType().FullName}'");

                return result;
            }

            // Open a stream to the input file.
            using (var stream = OpenRead(name, null))
            using (var reader = new BinaryReader(stream))
            {
                var typeName = reader.ReadString();
                if (typeof(T).FullName != typeName)
                    throw new InvalidOperationException($"{name}: type mismatch '{typeof(T).FullName}' / '{typeName}'");

                var resource = CreateResource(name, typeName, reader);
                _cache.Add(name, resource);

                return resource as T;
            }
        }

        /// <summary>
        /// Load assets from static fields in the given type
        /// </summary>
        /// <param name="type"></param>
        public static void Load(Type type)
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

        private static Resource CreateResource(string name, string typeName, BinaryReader reader)
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

