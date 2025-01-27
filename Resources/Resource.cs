/*

    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

using System.Diagnostics;

namespace NoZ
{
    public abstract class Resource : IDisposable
    {
        public string Path { get; internal set; }

        public int Refs { get; internal set; } = 1;

        public abstract void Dispose();

        protected internal abstract void Unload();
    }

    public abstract class Resource<T> : Resource where T : Resource
    {
        internal delegate void UnloadDelegate(T value);
        
        private static readonly Dictionary<string, Resource> s_loaded = new();
        private static readonly Dictionary<string, Resource> s_unused = new();
                       
        public static T Register(string path, T resource)
        {
            Debug.Assert(!s_loaded.ContainsKey(path));
            s_loaded[path] = resource;
            resource.Path = path;
            return resource;
        }

        public override void Dispose()
        {
            Refs--;
            Debug.Assert(Refs >= 0);

            if (Refs > 0)
                return;

            if (Path != null)
            {
                Debug.Assert(!s_unused.ContainsKey(Path));
                s_unused[Path] = this;
            }
            else
                Unload();
        }

        internal static void Shutdown()
        {
            foreach (var resource in s_loaded.Values)
            {
                resource.Refs = 0;
                resource.Dispose();
            }

            s_unused.Clear();
            s_loaded.Clear();
        }

        internal static void UnloadUnused()
        {
            foreach (var resource in s_unused.Values)
            {
                resource.Unload();
                s_loaded.Remove(resource.Path);
            }

            s_unused.Clear();
        }

        internal static bool TryGet(string path, out T? resource)
        {
            if (!s_loaded.TryGetValue(path, out var loaded))
            {
                resource = default;
                return false;
            }

            if (loaded.Refs == 0)
                s_unused.Remove(loaded.Path);

            loaded.Refs++;
            resource = (T)loaded;

            return true;
        }
    }

    public static class ResourceExtensions
    {
        public static TResource? GetRef<TResource>(this TResource? resource) where TResource : Resource
        {
            if (null == resource)
                return null;

            resource.Refs++;
            return resource;
        }
    }
}
