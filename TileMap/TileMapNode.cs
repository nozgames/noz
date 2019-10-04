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


using System.Collections.Generic;

namespace NoZ
{
    public class TileMapNode : Node, IDrawable
    {
        private Vertex[] _vertexBuffer;
        private short[] _indexBuffer;

        // TODO: Render the tiles
        // TODO: create colliders for tiles
        // TODO: size to tilemap
        // TODO: Tiles (Tile reference, quad)

        public int SortOrder { get; set; }

        private TileMap _tilemap;

        public TileMap TileMap {
            get => _tilemap;
            set {
                if (value == _tilemap)
                    return;

                // TODO: cleanup colliders

                _tilemap = value;

                UpdateColliders();
                UpdateMesh();
            }
        }

        private NoZ.Physics.IBody _body;

        protected override void OnEnterScene(Scene entering)
        {
            base.OnEnterScene(entering);

            if (_body != null)
            {
                _body.Dispose();
                _body = null;
            }

            UpdateColliders();
        }

        protected override Rect CalculateRect()
        {
            if(_tilemap==null)
                return base.CalculateRect();

            return new Rect(-((TileMap.Size * TileMap.TileSize).ToVector2() * 0.5f), (TileMap.Size * TileMap.TileSize).ToVector2());
        }

        private void UpdateColliders()
        {
            if (Scene == null)
                return;

            _body = Scene.World.CreateStaticBody();

            var offset = -((TileMap.Size * TileMap.TileSize).ToVector2() * 0.5f);

            List<NoZ.Physics.ICollider> colliders = new List<Physics.ICollider>();
            for (int y = 0; y < TileMap.Size.y; y++)
                for (int x = 0; x < TileMap.Size.x; x++)
                {
                    var tile = TileMap.Tiles[x + y * TileMap.Size.x];
                    if (tile.Polygons != null && tile.Polygons.Length > 0)
                    {
                        foreach (var polygon in tile.Polygons)
                        {
                            var collider = _body.AddPolygonCollider(
                                Physics.Physics.PixelsToMeters(offset + new Vector2(x * TileMap.TileSize.x, y * TileMap.TileSize.y)), polygon.Points);
                            if (polygon.Properties.TryGetValue("Layer", out var polygonLayer))
                                collider.CollisionMask = Physics.Physics.LayerToMask(Physics.Physics.NameToLayer(polygonLayer));
                        }                            
                    }

                    if(tile.Properties.TryGetValue("Layer", out var layer))
                    {                        
                        var collider = _body.AddBoxCollider(
                            Physics.Physics.PixelsToMeters(offset + new Vector2((x+0.5f) * TileMap.TileSize.x, (y+0.5f) * TileMap.TileSize.y)), 
                            Physics.Physics.PixelsToMeters(TileMap.TileSize.ToVector2()));
                        collider.CollisionMask = Physics.Physics.LayerToMask(Physics.Physics.NameToLayer(layer));
                    }
                }

            _body.Position = Position;
        }

        private void UpdateMesh ()
        {
            var offset = -((TileMap.Size * TileMap.TileSize).ToVector2() * 0.5f);

            var vertexCount = _tilemap.TileCount * 4;
            if (_vertexBuffer == null || _vertexBuffer.Length != vertexCount)
                _vertexBuffer = new Vertex[vertexCount];

            var indexCount = _tilemap.TileCount * 6;
            if (_indexBuffer == null || _indexBuffer.Length != indexCount)
                _indexBuffer = new short[indexCount];

            var v = 0;
            var i = 0;
            for (int y = 0; y < TileMap.Size.y; y++)
                for (int x = 0; x < TileMap.Size.x; x++)
                {
                    var tile = TileMap.Tiles[x + y * TileMap.Size.x];
                    if (tile == null)
                        continue;

                    var rect = new Rect(
                        tile.Rect.x / (float)TileMap.TileSets[0].Image.Width,
                        tile.Rect.y / (float)TileMap.TileSets[0].Image.Height,
                        tile.Rect.width / (float)TileMap.TileSets[0].Image.Width,
                        tile.Rect.height / (float)TileMap.TileSets[0].Image.Height);

                    _vertexBuffer[v + 0] = new Vertex(offset + new Vector2((x + 0) * 32, (y + 0) * 32), rect.TopLeft);
                    _vertexBuffer[v + 1] = new Vertex(offset + new Vector2((x + 1) * 32, (y + 0) * 32), rect.TopRight);
                    _vertexBuffer[v + 2] = new Vertex(offset + new Vector2((x + 1) * 32, (y + 1) * 32), rect.BottomRight);
                    _vertexBuffer[v + 3] = new Vertex(offset + new Vector2((x + 0) * 32, (y + 1) * 32), rect.BottomLeft);

                    _indexBuffer[i + 0] = (short)v;
                    _indexBuffer[i + 1] = (short)(v + 1);
                    _indexBuffer[i + 2] = (short)(v + 3);
                    _indexBuffer[i + 3] = (short)(v + 3);
                    _indexBuffer[i + 4] = (short)(v + 2);
                    _indexBuffer[i + 5] = (short)(v + 1);

                    v += 4;
                    i += 6;
                }
        }

        public bool Draw(GraphicsContext context)
        {
            if (null == TileMap)
                return false;

            context.Color = Color.White;
            context.Image = TileMap.TileSets[0].Image;
            context.Draw(PrimitiveType.TriangleList, _vertexBuffer, _vertexBuffer.Length, _indexBuffer, _indexBuffer.Length);

            return true;
        }
    }
}

