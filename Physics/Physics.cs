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

namespace NoZ.Physics
{
    public static class Physics
    {
        public const uint CollisionMaskNone = 0;
        public const uint CollisionMaskAll = 0xFFFFFFFF;

        /// <summary>
        /// Current physics driver
        /// </summary>
        public static IPhysicsDriver Driver { get; set; }

        /// <summary>
        /// Create a new physics world
        /// </summary>
        /// <returns>Created world</returns>
        public static IWorld CreateWorld() => Driver.CreateWorld();

        /// <summary>
        /// Layer names
        /// </summary>
        private static string[] LayerNames = new string[32] {
            "Layer0",
            "Layer1",
            "Layer2",
            "Layer3",
            "Layer4",
            "Layer5",
            "Layer6",
            "Layer7",
            "Layer8",
            "Layer9",
            "Layer10",
            "Layer11",
            "Layer12",
            "Layer13",
            "Layer14",
            "Layer15",
            "Layer16",
            "Layer17",
            "Layer18",
            "Layer19",
            "Layer20",
            "Layer21",
            "Layer22",
            "Layer23",
            "Layer24",
            "Layer25",
            "Layer26",
            "Layer27",
            "Layer28",
            "Layer29",
            "Layer30",
            "Layer31",
            };

        /// <summary>
        /// Set the name of a layer
        /// </summary>
        public static void SetLayerName(int layer, string name) => LayerNames[layer] = name;

        /// <summary>
        /// Return the layer name for a given layer index
        /// </summary>
        public static string LayerToName(int layer) => LayerNames[layer];

        /// <summary>
        /// Convert a layer to a layer mask
        /// </summary>
        public static uint LayerToMask(int layer) => (uint)(1 << layer);

        /// <summary>
        /// Convert a layer name to a layer index
        /// </summary>
        public static int NameToLayer(string name)
        {
            for (int i = 0; i < LayerNames.Length; i++)
                if (LayerNames[i] == name)
                    return i;

            return -1;
        }

        /// <summary>
        /// Convert a layer name to a layer index
        /// </summary>
        public static uint NameToLayerMask(string name) => LayerToMask(NameToLayer(name));
    }
}
