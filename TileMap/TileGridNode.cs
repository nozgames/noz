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

namespace NoZ
{
    /// <summary>
    /// Renderable grid of tiles used by TileMap
    /// </summary>
    public class TileGridNode : Node
    {
        private Quad[] _quads;
        private IBody _body;

        public int SortOrder { get; set; }

        public int SortLayer { get; set; }

        public TileMap.LayerInfo Layer { get; private set; }

        public TileMap TileMap => Layer.TileMap;

        public TileGridNode (TileMap.LayerInfo layer)
        {
            Layer = layer;
            UpdateMesh();
            IsDrawable = true;
        }

        private void UpdateMesh()
        {
            var offset = -((Layer.Size * TileMap.TileSize).ToVector2() * 0.5f);

            if (_quads == null || _quads.Length != Layer.TileCount)
                _quads = new Quad[Layer.TileCount];

            // Texels per x any y unit 
            var tpx = 1.0f / TileMap.TileSets[0].Image.Width;
            var tpy = 1.0f / TileMap.TileSets[0].Image.Height;

            for (int y = 0, q = 0; y < TileMap.Size.y; y++)
                for (int x = 0; x < TileMap.Size.x; x++, q++)
                {
                    var tile = Layer.Tiles[x + y * TileMap.Size.x];
                    if (tile == null)
                        continue;

                    var rect = new Rect(
                        (tile.Rect.x + 0.25f) * tpx,
                        (tile.Rect.y + 0.25f) * tpy,
                        (tile.Rect.width - 0.5f) * tpx,
                        (tile.Rect.height - 0.5f) * tpy);

                    _quads[q] = new Quad
                    {
                        TL = new Vertex(offset + new Vector2((x + 0) * 32, (y + 0) * 32), rect.TopLeft),
                        TR = new Vertex(offset + new Vector2((x + 1) * 32, (y + 0) * 32), rect.TopRight),
                        BR = new Vertex(offset + new Vector2((x + 1) * 32, (y + 1) * 32), rect.BottomRight),
                        BL = new Vertex(offset + new Vector2((x + 0) * 32, (y + 1) * 32), rect.BottomLeft)
                    };
                }
        }

        private void UpdateColliders()
        {
            if (Scene == null)
                return;

            _body = Scene.World.CreateStaticBody();

            var offset = -((TileMap.Size * TileMap.TileSize).ToVector2() * 0.5f);

            for (int y = 0; y < TileMap.Size.y; y++)
                for (int x = 0; x < TileMap.Size.x; x++)
                {
                    var tile = Layer.Tiles[x + y * TileMap.Size.x];
                    if (null == tile)
                        continue;

                    if (tile.Polygons != null && tile.Polygons.Length > 0)
                    {
                        foreach (var polygon in tile.Polygons)
                        {
                            var collider = _body.AddPolygonCollider(
                                Physics.PixelsToMeters(offset + new Vector2(x * TileMap.TileSize.x, y * TileMap.TileSize.y)), polygon.Points);
                            if (polygon.Properties.TryGetValue("Layer", out var polygonLayer))
                            {
                                collider.CollisionMask = (uint)(1 << Physics.NameToLayer(polygonLayer));
                                collider.CollidesWithMask = Physics.GetLayerCollisionMask(Physics.NameToLayer(polygonLayer));
                            }
                        }
                    }

                    if (tile.Properties.TryGetValue("Layer", out var layer))
                    {
                        var collider = _body.AddBoxCollider(
                            Physics.PixelsToMeters(offset + new Vector2((x + 0.5f) * TileMap.TileSize.x, (y + 0.5f) * TileMap.TileSize.y)),
                            Physics.PixelsToMeters(TileMap.TileSize.ToVector2()));
                        collider.CollisionMask = (uint)(1 << Physics.NameToLayer(layer));
                        collider.CollidesWithMask = Physics.GetLayerCollisionMask(Physics.NameToLayer(layer));
                    }
                }

            _body.Position = Position;
        }

        public override void Draw (GraphicsContext gc)
        {
            if (null == TileMap)
                return;

            gc.Color = Color.White;
            gc.Image = TileMap.TileSets[0].Image;
            gc.SortOrder = (short)SortOrder;
            gc.SortLayer = (byte)SortLayer;
            gc.Draw(_quads, 0, _quads.Length); 
        }

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

        protected override void OnLeaveScene (Scene leaving)
        {
            _body?.Dispose();
            _body = null;

            base.OnLeaveScene(leaving);
        }
    }
}
