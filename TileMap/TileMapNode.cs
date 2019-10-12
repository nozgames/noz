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
    public class TileMapNode : Node
    {
        public int SortOrder { get; set; }

        public TileMap TileMap { get; private set; }

        public TileMapNode (TileMap tilemap)
        {
            TileMap = tilemap;

            var offset = -((TileMap.Size * TileMap.TileSize).ToVector2() * 0.5f);

            foreach (var layer in tilemap.Layers)
            {
                var layerNode = new Node();
                layerNode.Name = layer.Name;

                if (layer.Tiles != null)
                {
                    var grid = new TileGridNode(layer);
                    grid.SortOrder = int.Parse(layer.GetProperty("SortOrder")?.Value ?? "0");
                    layerNode.AddChild(grid);
                }

                if(layer.Objects != null)
                    foreach(var objdef in layer.Objects)
                    {
                        var node = TileMap.CreateObject(objdef);
                        if(node != null)
                        {
                            node.Position = objdef.Position + offset;
                            layerNode.AddChild(node);
                        }
                    }
                        

                AddChild(layerNode);
            }
        }

        /// <summary>
        /// Return the tilemap size as its measure
        /// </summary>
        protected override Vector2 MeasureOverride(in Vector2 available) => (TileMap.Size * TileMap.TileSize).ToVector2();

        /// <summary>
        /// Gets the tile at the given XY coordinate
        /// </summary>
        public Tile GetTile(int x, int y) => TileMap?.GetTile(x, y) ?? null;

        public Tile GetTile(in Vector2Int position) => TileMap?.GetTile(position) ?? null;

        public Vector2 CellToLocal (in Vector2Int position)
        {
            if (null == TileMap)
                return Vector2.Zero;

            if (position.x < 0 || position.x >= TileMap.Size.x)
                throw new ArgumentOutOfRangeException("x");

            if (position.y < 0 || position.y >= TileMap.Size.y)
                throw new ArgumentOutOfRangeException("y");

            return (position.ToVector2() * TileMap.TileSize.ToVector2()) - ((TileMap.Size * TileMap.TileSize).ToVector2() * 0.5f);
        }

        public Vector2 CellToScene (in Vector2Int position) => LocalToScene(CellToLocal(position));

        public Vector2Int SceneToCell(in Vector2 position) => LocalToCell(SceneToLocal(position));

        public Vector2Int LocalToCell (in Vector2 position) =>
            ((position + ((TileMap.Size * TileMap.TileSize).ToVector2() * 0.5f)) / TileMap.TileSize.ToVector2()).ToVector2Int();
    }
}

