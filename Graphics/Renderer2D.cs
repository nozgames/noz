/*

    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

using System.Numerics;
using System.Runtime.CompilerServices;

namespace NoZ.Graphics
{
    /// <summary>
    /// Track and render all sprites in the game each frame
    /// </summary>
    public static unsafe class Renderer2D
    {
        private static List<Material>? _materials;
        private static RenderObject2D[]? _objects;
        private static int _count;
        private static Texture2D _emptyTexture;
        private static Vector2[] _quad = new Vector2[4];
        private static Vector2* _quadPtr;
        private static Shader? _spriteShader;
        private static Shader? _solidShader;

        public static Shader SpriteShader => _spriteShader!;

        public static Shader SolidShader => _solidShader!;

        public static void Initialize(int maxSprites)
        {
            _spriteShader = ResourceDatabase.LoadShader("shaders/sprite", "shaders/sprite");
            _solidShader = ResourceDatabase.LoadShader("shaders/solid", "shaders/solid");

            _count = 0;
            _materials = new List<Material>(32);
            _objects = new RenderObject2D[maxSprites];

            // TODO: Remove Raylib_cs dependencies and implement SDL3 rendering logic as needed.
            _emptyTexture = default;

            _quadPtr = (Vector2*)Unsafe.AsPointer(ref _quad[0]);
        }

        public static void Shutdown()
        {
            _materials?.Clear();

            _objects = null;
            _materials = null;
            _count = 0;
        }

        public static void Update()
        {
            _count = 0;
        }

        private static int SortSprites(RenderObject2D a, RenderObject2D b) =>
            a.SortOrder - b.SortOrder;
        
        public static void Render()
        {
            // TODO: Remove Raylib_cs dependencies and implement SDL3 rendering logic as needed.
        }

        internal static void Render(in Mesh mesh, Material material, in Matrix3x2 transform, int sortOrder=0)
        {
            if (_count >= _objects!.Length)
                return;

            var materialIndex = _materials!.Count - 1;
            if (_materials.Count == 0 || _materials[materialIndex] != material)
            {
                _materials.Add(material);
                materialIndex = _materials.Count - 1; 
            }

            _objects[_count++] = new RenderObject2D
            {
                Mesh = mesh,
                MaterialIndex = materialIndex,
                SortOrder = sortOrder,
                Transform = transform
            };
        }

        public static void Render(in Sprite sprite, in Vector3 position, Color color, float scale = 1.0f, int sortOrder=0)
        {
            // TODO: Remove Raylib_cs dependencies and implement SDL3 rendering logic as needed.
        }
    }
}
