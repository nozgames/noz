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
    public class TileMap : Resource
    {
        /// <summary>
        /// Defines an object shape type
        /// </summary>
        public enum ShapeType
        {
            Point,
            Box,
            Circle,
            PolyLine,
            Polygon
        }

        /// <summary>
        /// Defines a property
        /// </summary>
        public class PropertyInfo
        {
            public string Name { get; private set; }
            public string Value { get; private set; }

            public PropertyInfo(BinaryReader reader)
            {
                Name = reader.ReadString();
                Value = reader.ReadString();
            }
        }

        /// <summary>
        /// Definition of an object within a layer
        /// </summary>
        public class ObjectInfo
        {
            public string Name { get; private set; }
            public string Type { get; private set; }
            public Vector2 Position { get; set; }
            public ShapeType ShapeType { get; private set; }
            public Vector2[] Points { get; private set; }
            public PropertyInfo[] Properties { get; private set; }

            /// <summary>
            /// Return the property with the given name
            /// </summary>
            public PropertyInfo GetProperty(string name)
            {
                if (null == Properties)
                    return null;

                foreach (var property in Properties)
                    if (property.Name == name)
                        return property;

                return null;
            }

            public ObjectInfo (BinaryReader reader)
            {
                Name = reader.ReadString();
                Type = reader.ReadString();
                Position = reader.ReadVector2();
                ShapeType = (ShapeType)reader.ReadByte();

                switch(ShapeType)
                {
                    case ShapeType.Box:
                    {
                        var w = reader.ReadSingle();
                        var h = reader.ReadSingle();

                        Points = new Vector2[4] {
                            Vector2.Zero,
                            new Vector2(w, 0),
                            new Vector2(w, h),
                            new Vector2(0, h)
                        };
                        break;
                    }

                    case ShapeType.Polygon:
                    {
                        Points = new Vector2[reader.ReadUInt16()];
                        for(int p=0; p<Points.Length; p++)
                            Points[p] = reader.ReadVector2();
                        break;
                    }

                    case ShapeType.PolyLine:
                    {
                        Points = new Vector2[reader.ReadUInt16()];
                        for (int p = 0; p < Points.Length; p++)
                            Points[p] = reader.ReadVector2();
                        break;
                    }

                    case ShapeType.Circle:
                    {
                        Points = new Vector2[] { reader.ReadVector2() };
                        break;
                    }
                }

                var propertyCount = reader.ReadUInt16();
                if(propertyCount > 0)
                {
                    Properties = new PropertyInfo[propertyCount];
                    for (int propertyIndex = 0; propertyIndex < propertyCount; propertyIndex++)
                        Properties[propertyIndex] = new PropertyInfo(reader);
                }
            }
        }

        public class LayerInfo
        {
            /// <summary>
            /// Tilemap the layer belongs to
            /// </summary>
            public TileMap TileMap { get; private set; }

            /// <summary>
            /// Name of the layer
            /// </summary>
            public string Name { get; private set; }

            /// <summary>
            /// Objects defined in the layer
            /// </summary>
            public ObjectInfo[] Objects { get; private set; }

            /// <summary>
            /// Number of non empty tiles in the tile map
            /// </summary>
            public int TileCount { get; private set; }

            /// <summary>
            /// Tiles in the tile map
            /// </summary>
            public Tile[] Tiles { get; private set; }

            /// <summary>
            /// Size of the tile map in tiles.
            /// </summary>
            public Vector2Int Size { get; private set; }

            /// <summary>
            /// Offset from the tilemap top-left corner in tiles
            /// </summary>
            public Vector2Int Offset { get; private set; }

            public PropertyInfo[] Properties { get; private set; }

            public ObjectInfo GetObject(string name)
            {
                if (null == Objects)
                    return null;

                foreach (var info in Objects)
                    if (info.Name == name)
                        return info;

                return null;
            }

            /// <summary>
            /// Construct a layer from a tilemap stream
            /// </summary>
            public LayerInfo (TileMap tilemap, BinaryReader reader)
            {
                TileMap = tilemap;
                Name = reader.ReadString();
                Size = new Vector2Int(reader.ReadUInt16(), reader.ReadUInt16());

                // Read in all properties
                var propertyCount = reader.ReadUInt16();
                if (propertyCount > 0)
                {
                    Properties = new PropertyInfo[propertyCount];
                    for (int propertyIndex = 0; propertyIndex < propertyCount; propertyIndex++)
                        Properties[propertyIndex] = new PropertyInfo(reader);
                }

                TileCount = reader.ReadUInt16();
                if(TileCount > 0)
                {
                    Tiles = new Tile[Size.x * Size.y];

                    for (var tileIndex = 0; tileIndex < TileCount; tileIndex++)
                    {
                        var tileSetIndex = reader.ReadUInt16();
                        if (tileSetIndex == 0xFFFF)
                            continue;

                        var tileId = reader.ReadUInt16();
                        Tiles[tileIndex] = tilemap.TileSets[tileSetIndex].Tiles[tileId];
                    }
                }

                // Read in all objects
                var objectCount = reader.ReadUInt16();
                if (objectCount > 0)
                {
                    Objects = new ObjectInfo[objectCount];
                    for (int objectIndex = 0; objectIndex < Objects.Length; objectIndex++)
                        Objects[objectIndex] = new ObjectInfo(reader);
                }
            }

            /// <summary>
            /// Return the property with the given name
            /// </summary>
            public PropertyInfo GetProperty(string name)
            {
                if (null == Properties)
                    return null;

                foreach (var property in Properties)
                    if (property.Name == name)
                        return property;

                return null;
            }
        }

        /// <summary>
        /// Size of the tilemap in tiles.
        /// </summary>
        public Vector2Int Size { get; private set; }

        /// <summary>
        /// Tile sets used in the tile map
        /// </summary>
        public TileSet[] TileSets { get; private set; }

        /// <summary>
        /// Size of a single tile
        /// </summary>
        public Vector2Int TileSize { get; private set; }

        protected TileMap(string name) : base(name) { }

        public LayerInfo[] Layers { get; private set; }

        public static TileMap Create (string name, BinaryReader reader)
        {
            // Create the resuslting tilemap
            var tilemap = new TileMap(name);

            // Size of a tile in pixels
            tilemap.Size = new Vector2Int(reader.ReadUInt16(), reader.ReadUInt16());
            tilemap.TileSize = new Vector2Int(reader.ReadUInt16(), reader.ReadUInt16());

            // Load all tile sets
            tilemap.TileSets = new TileSet[reader.ReadUInt16()];
            for (var tileSetIndex = 0; tileSetIndex < tilemap.TileSets.Length; tileSetIndex++)
                tilemap.TileSets[tileSetIndex] = Resource.Load<TileSet>(reader.ReadString());

            // Read in all layers
            tilemap.Layers = new LayerInfo[reader.ReadUInt16()];
            for (int layerIndex=0; layerIndex< tilemap.Layers.Length; layerIndex++)
                tilemap.Layers[layerIndex] = new LayerInfo(tilemap, reader);

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

            return Layers[0].Tiles[x + y * Size.x];
        }

        public Tile GetTile(in Vector2Int position) => GetTile(position.x, position.y);


        public delegate Node CreateObjectDelegate (ObjectInfo def);

        private static Dictionary<string, CreateObjectDelegate> _createObjectDelegates = 
            new Dictionary<string, CreateObjectDelegate>();

        static TileMap ( )
        {
            // Collider support
            RegisterObject("Collider", (ObjectInfo objectInfo) =>
            {
                switch (objectInfo.ShapeType)
                {
                    case ShapeType.Box:
                    case ShapeType.Polygon:
                        return new PolygonCollider(objectInfo.Points, Physics.NameToLayer(objectInfo.GetProperty("Layer").Value));

                    case ShapeType.Circle:
                        return new CircleCollider(MathEx.Max(objectInfo.Points[0].x, objectInfo.Points[0].y), Physics.NameToLayer(objectInfo.GetProperty("Layer").Value));
                }

                return null;
            });
        }

        public static void RegisterObject (string typeName, CreateObjectDelegate createObjectDelegate)
        {
            _createObjectDelegates.Add(typeName, createObjectDelegate);
        }

        /// <summary>
        /// Create a node from the given object information.
        /// </summary>
        public static Node CreateObject (ObjectInfo def)
        {
            if (!_createObjectDelegates.TryGetValue(def.Type, out var createObjectDelegate))
                return null;

            return createObjectDelegate(def);
        }
    }
}
