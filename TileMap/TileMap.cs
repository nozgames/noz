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

namespace NoZ
{
    public class TileMap : Resource
    {
        /// <summary>
        /// Tiles in the tile map
        /// </summary>
        public Tile[] Tiles { get; private set; }

        /// <summary>
        /// Tile sets used in the tile map
        /// </summary>
        public TileSet[] TileSets { get; private set; }

        /// <summary>
        /// Size of the tile map in tiles.
        /// </summary>
        public Vector2Int Size { get; private set; }

        /// <summary>
        /// Size of a single tile
        /// </summary>
        public Vector2Int TileSize { get; private set; }

        /// <summary>
        /// Number of tiles in the tile map (excludes empty tiles)
        /// </summary>
        public int TileCount { get; private set; }

        protected TileMap(string name) : base(name) { }


        public static TileMap Create (string name, BinaryReader reader)
        {
            var tilemap = new TileMap(name);
            tilemap.Size = new Vector2Int(reader.ReadUInt16(), reader.ReadUInt16());
            tilemap.TileSize = new Vector2Int(reader.ReadUInt16(), reader.ReadUInt16());

            // Load all tile sets
            tilemap.TileSets = new TileSet[reader.ReadUInt16()];
            for(var tileSetIndex = 0; tileSetIndex<tilemap.TileSets.Length; tileSetIndex++)
                tilemap.TileSets[tileSetIndex] = Resource.Load<TileSet>(reader.ReadString());

            // Load all tiles
            tilemap.Tiles = new Tile[tilemap.Size.x * tilemap.Size.y];
            tilemap.TileCount = reader.ReadUInt16();
            for(var tileIndex = 0; tileIndex < tilemap.TileCount; tileIndex++)
            {
                var tileSetIndex = reader.ReadUInt16();
                if (tileSetIndex == 0xFFFF)
                    continue;

                var tileId = reader.ReadUInt16();
                tilemap.Tiles[tileIndex] = tilemap.TileSets[tileSetIndex].Tiles[tileId];
            }

            return tilemap;
        }

        /// <summary>
        /// Gets the tile at the given XY coordinate
        /// </summary>
        public Tile GetTile (int x, int y) 
        {
            if (x < 0 || x >= Size.x)
                throw new ArgumentOutOfRangeException("x");

            if(y < 0 || y >= Size.y)
                throw new ArgumentOutOfRangeException("y");

            return Tiles[x + y * Size.x];
        }

        public Tile GetTile(in Vector2Int position) => GetTile(position.x, position.y);
    }
}
