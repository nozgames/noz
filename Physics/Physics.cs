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

namespace NoZ
{
    public static class Physics
    {
        public const uint CollisionMaskNone = 0;
        public const uint CollisionMaskAll = 0xFFFFFFFF;

        /// <summary>
        /// True if a world has been created
        /// </summary>
        private static bool _worldCreated = false;

        public static int _pixelsPerMeter = 100;
        private static float _pixelsToMeters = 1.0f / _pixelsPerMeter;
        private static float _metersToPixels = _pixelsPerMeter;

        /// <summary>
        /// Determiens the pixels per meter of the world.  Must be initialized before 
        /// any world have been created.
        /// </summary>
        public static int PixelsPerMeter {
            get => _pixelsPerMeter;
            set {
                if (_worldCreated)
                    throw new System.InvalidOperationException("Pixels per meter must be set before any worlds are created");

                _pixelsPerMeter = value;
                _pixelsToMeters = 1.0f / _pixelsToMeters;
                _metersToPixels = _pixelsPerMeter;
            }
        }

        /// <summary>
        /// Convert pixels to meters
        /// </summary>
        public static float PixelsToMeters(float pixels) => pixels * _pixelsToMeters;

        /// <summary>
        /// Convert pixels to meters
        /// </summary>
        public static Vector2 PixelsToMeters(in Vector2 pixels) => pixels * _pixelsToMeters;

        /// <summary>
        /// Convert meters to pixels
        /// </summary>
        public static float MetersToPixels(float meters) => meters * _metersToPixels;

        /// <summary>
        /// Convert meters to pixels
        /// </summary>
        public static Vector2 MetersToPixels(in Vector2 meters) => meters * _metersToPixels;

        /// <summary>
        /// Current physics driver
        /// </summary>
        public static IPhysicsDriver Driver { get; set; }

        /// <summary>
        /// Create a new physics world
        /// </summary>
        /// <returns>Created world</returns>
        public static IWorld CreateWorld()
        {
            _worldCreated = true;
            return Driver.CreateWorld();
        }

        private struct Layer
        {
            public string name;
            public uint mask;

            public Layer(string _name) { name = _name; mask = Physics.CollisionMaskNone; }
        }

        /// <summary>
        /// Layers
        /// </summary>
        private static Layer[] Layers = new Layer[32] {
            new Layer("Default"),
            new Layer("Layer1"),
            new Layer("Layer2"),
            new Layer("Layer3"),
            new Layer("Layer4"),
            new Layer("Layer5"),
            new Layer("Layer6"),
            new Layer("Layer7"),
            new Layer("Layer8"),
            new Layer("Layer9"),
            new Layer("Layer10"),
            new Layer("Layer11"),
            new Layer("Layer12"),
            new Layer("Layer13"),
            new Layer("Layer14"),
            new Layer("Layer15"),
            new Layer("Layer16"),
            new Layer("Layer17"),
            new Layer("Layer18"),
            new Layer("Layer19"),
            new Layer("Layer20"),
            new Layer("Layer21"),
            new Layer("Layer22"),
            new Layer("Layer23"),
            new Layer("Layer24"),
            new Layer("Layer25"),
            new Layer("Layer26"),
            new Layer("Layer27"),
            new Layer("Layer28"),
            new Layer("Layer29"),
            new Layer("Layer30"),
            new Layer("Layer31"),
            };

        /// <summary>
        /// Set the name of a layer
        /// </summary>
        public static void SetLayerName(int layer, string name)
        {
            if (layer == 0)
                throw new System.ArgumentOutOfRangeException("default layer name cannot be changed");

            Layers[layer].name = name;
        }

        /// <summary>
        /// Return the layer name for a given layer index
        /// </summary>
        public static string LayerToName(int layer) => Layers[layer].name;

        /// <summary>
        /// Convert a layer name to a layer index
        /// </summary>
        public static int NameToLayer(string name)
        {
            for (int i = 0; i < Layers.Length; i++)
                if (Layers[i].name == name)
                    return i;

            return -1;
        }

        /// <summary>
        /// Enable or disable colision between two layers
        /// </summary>
        public static void EnableCollision(int layer1, int layer2, bool enable=true)
        {
            if (enable)
            {
                Layers[layer1].mask |= (uint)(1 << layer2);
                Layers[layer2].mask |= (uint)(1 << layer1);
            }
            else
            {
                Layers[layer1].mask &= (uint)~(1 << layer2);
                Layers[layer2].mask &= (uint)~(1 << layer1);
            }
        }

        /// <summary>
        /// Get the collision layer mask that indicates which layer(s) the given layer will collide with.
        /// </summary>
        public static uint GetLayerCollisionMask(int layer) => Layers[layer].mask;

        /// <summary>
        /// Set the collision layer mask that indicates which layer(s) the given layer will collide with
        /// </summary>
        public static uint SetLayerCollisionMask(int layer, uint mask) => Layers[layer].mask = mask;
    }
}
