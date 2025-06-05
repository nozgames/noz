/*

    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

using System.Numerics;
using System.Runtime.CompilerServices;
using Raylib_cs;

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
        //private static Shader _currentShader;
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

            var image = Raylib.GenImageColor(256, 256, Color.White);
            _emptyTexture = Raylib.LoadTextureFromImage(image);

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
            //_currentShader = default;

            // Sort the sprites by their sort order
            _objects.AsSpan(0, _count).Sort(SortSprites);

            Raylib.BeginBlendMode(BlendMode.Alpha);

            var ppuInv = Application.PixelsPerUnitInv;
            var renderObject = (RenderObject2D*)Unsafe.AsPointer(ref _objects![0]);
            var transform = Matrix4x4.Identity;

            for (var i = 0; i < _count; i++, renderObject++)
            {
#if false
                var renderWidth = renderSprite->AtlasRect.Width * ppuInv;
                var renderHeight = renderSprite->AtlasRect.Height * ppuInv;
                //var renderRect = new Rectangle(
                //    renderPosition.X,
                //    renderPosition.Y,
                //    renderWidth,
                //    renderHeight);
                var sourceRect = new Rectangle(
                    renderSprite->AtlasRect.X,
                    renderSprite->AtlasRect.Y,
                    renderSprite->AtlasRect.Width,
                    renderSprite->AtlasRect.Height
                );

                //var renderPivot = renderSprite->Sprite.Pivot;
                //if (renderSprite->Flip)
                //{
                //    renderPivot.X = 1.0f - renderPivot.X;
                //    sourceRect.Width = -sourceRect.Width;
                //}

                //renderPivot.X *= renderWidth;
                //renderPivot.Y *= renderHeight;

                //if (_currentShader.Id != renderSprite->Shader.Id ||
                //    renderSprite->TexelSizeLocation != 0 ||
                //    renderSprite->CustomShaderLocation1 != 0)
                //{
                //    if (_currentShader.Id != 0)
                //        Raylib.EndShaderMode();

                //    _currentShader = renderSprite->Shader;
                //    if (_currentShader.Id != 0)
                //        Raylib.BeginShaderMode(_currentShader);
                //}

                //if (renderSprite->TexelSizeLocation != 0)
                //{
                //    var texelSize = (1.0f / Application.PixelsPerUnit) / renderWidth;
                //    Raylib.SetShaderValue(_currentShader, renderSprite->TexelSizeLocation, texelSize, ShaderUniformDataType.Float);
                //}

                //if (renderSprite->CustomShaderLocation1 != 0)
                //{
                //    Raylib.SetShaderValue(_currentShader, renderSprite->CustomShaderLocation1, renderSprite->CustomShaderValue1, ShaderUniformDataType.Float);
                //}

                var renderPivot = renderSprite->Pivot;
                _quadPtr[0] = Vector2.Transform(new Vector2(-renderWidth * renderPivot.X, -renderHeight * renderPivot.Y), renderSprite->Transform);
                _quadPtr[1] = Vector2.Transform(new Vector2( renderWidth * renderPivot.X, -renderHeight * renderPivot.Y), renderSprite->Transform);
                _quadPtr[2] = Vector2.Transform(new Vector2(-renderWidth * renderPivot.X,  renderHeight * renderPivot.Y), renderSprite->Transform);
                _quadPtr[3] = Vector2.Transform(new Vector2( renderWidth * renderPivot.X,  renderHeight * renderPivot.Y), renderSprite->Transform);

                var material = Raylib.LoadMaterialDefault();
                Raylib.SetMaterialTexture(ref material, 0, _atlases![renderSprite->AltasId]!.Value.Texture);

                var mesh = new Mesh(4, 6);
                mesh.AllocVertices();
                mesh.AllocIndices();
                mesh.AllocTexCoords();
                var vertices = (Vector3*)mesh.Vertices;
                vertices[0] = new Vector3(-renderWidth * renderPivot.X, -renderHeight * renderPivot.Y, 0);
                vertices[1] = new Vector3( renderWidth * renderPivot.X, -renderHeight * renderPivot.Y, 0);
                vertices[2] = new Vector3( renderWidth * renderPivot.X,  renderHeight * renderPivot.Y, 0);
                vertices[3] = new Vector3(-renderWidth * renderPivot.X,  renderHeight * renderPivot.Y, 0);
                var indices = (ushort*)mesh.Indices;
                indices[0] = 0;
                indices[1] = 1;
                indices[2] = 2;
                indices[3] = 2;
                indices[4] = 1;
                indices[5] = 3;
                var uv = (Vector2*)mesh.TexCoords;
                uv[0] = new Vector2(0, 0);
                uv[1] = new Vector2(1, 0);
                uv[2] = new Vector2(0, 1);
                uv[3] = new Vector2(1, 1);
                Raylib.UploadMesh(ref mesh, false);

                var transform = new Matrix4x4(renderSprite->Transform);

                //Raylib.SetMaterialTexture(material, )
                Raylib.DrawMesh(mesh, material, Matrix4x4.Identity);

                //if (renderSprite->Sprite.AtlasId < 0)
                //    Raylib.DrawTexturePro(
                //        _emptyTexture,
                //        sourceRect,
                //        renderRect,
                //        renderPivot,
                //        renderSprite->Rotation,
                //        renderSprite->Color);
                //else
                //    Raylib.DrawTexturePro(
                //        _atlases![renderSprite->Sprite.AtlasId]!.Value.Texture,
                //        sourceRect,
                //        renderRect,
                //        renderPivot,
                //        renderSprite->Rotation,
                //        renderSprite->Color);
#endif
                // TODO: remove transpose here
                Raylib.DrawMesh(
                    renderObject->Mesh,
                    _materials![renderObject->MaterialIndex]._material,
                    Matrix4x4.Transpose(new Matrix4x4(renderObject->Transform)));
            }


            Raylib.EndBlendMode();

//            _currentShader = default;
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
            //if (_count >= _sprites!.Length)
            //    return;

            //if (color.A == 0)
            //    return;

            //_sprites[_count++] = new SpriteRendererOld {
            //    Color = color,
            //    Sprite = sprite,
            //    Position = new Vector2(position.X, -position.Y + position.Z),
            //    Scale = Vector2.One * scale,
            //    SortOrder = sortOrder
            //};
        }
    }
}
