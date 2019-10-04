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
using System.Collections.Generic;
using System.IO;

namespace NoZ
{
    public class Tile
    {
        public class Polygon
        {
            public Vector2[] Points { get; }
            public Dictionary<string, string> Properties { get;  }

            public Polygon(Vector2[] points, Dictionary<string,string> properties)
            {
                Points = points;
                Properties = properties;
            }
        }

        public ushort Id { get; private set; }
        public Polygon[] Polygons { get; private set; }
        public Dictionary<string, string> Properties { get; private set; }

        /// <summary>
        /// Rectangle of the tile within the image
        /// </summary>
        public RectInt Rect { get; private set; }

        private Tile () { }

        private static Dictionary<string, string> ReadProperties (BinaryReader reader)
        {
            var propertyCount = reader.ReadUInt16();
            var properties = new Dictionary<string, string>();
            if (propertyCount > 0)
            {
                for (int propertyIndex = 0; propertyIndex < propertyCount; propertyIndex++)
                {
                    var key = reader.ReadString();
                    var value = reader.ReadString();
                    properties[key] = value;
                }
            }

            return properties;
        }

        public static Tile Create (BinaryReader reader, in Vector2Int tileSize)
        {
            var id = reader.ReadUInt16();
            if (id == 0xFFFF)
                return null;

            var tile = new Tile();
            tile.Id = id;
            tile.Rect = new RectInt(reader.ReadUInt16(), reader.ReadUInt16(), tileSize.x, tileSize.y);

            tile.Properties = ReadProperties(reader);

            tile.Polygons = new Polygon[reader.ReadUInt16()];
            for(int i=0; i<tile.Polygons.Length; i++)
            {
                var points = new Vector2[reader.ReadUInt16()];
                for(int pointIndex=0; pointIndex<points.Length; pointIndex++)
                    points[pointIndex] = new Vector2(reader.ReadSingle(), reader.ReadSingle());

                var properties = ReadProperties(reader);

                tile.Polygons[i] = new Polygon(points, properties);
            }

            return tile;
        }

    }
}

