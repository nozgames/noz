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
    public class TileSet : Resource
    {
        public Tile[] Tiles { get; private set; }
        public Vector2Int TileSize { get; private set; }

        protected TileSet(string name) : base(name) { }

        public static TileSet Create(string name, BinaryReader reader)
        {
            var tilemap = new TileSet(name);
            tilemap.TileSize = new Vector2Int(reader.ReadInt16(), reader.ReadInt16());

            // Read in all tiles
            tilemap.Tiles = new Tile[reader.ReadUInt16()];
            var tileCount = reader.ReadUInt16();
            for(int tileIndex = 0; tileIndex < tileCount; tileIndex++)
            {
                var tile = Tile.Create(reader);
                if (tile == null)
                    continue;

                tilemap.Tiles[tile.Id] = tile;
            }

            return tilemap;
        }
    }
}
