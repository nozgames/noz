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
    public class EdgeCollider : Collider
    {
        public Vector2[] Points { get; set; }

        public bool IsLoop { get; set; }

        public EdgeCollider()
        {
        }

        public EdgeCollider(Vector2 start, Vector2 end)
        {
            Points = new Vector2[] { start, end };
        }

        public EdgeCollider(Vector2[] points, bool loop=false)
        {
            Points = points;
            IsLoop = loop;
        }

        protected override ICollider CreateCollider(IBody body)
        {
            if (Points.Length < 2)
                return null;

            if(Points.Length == 2)
                return body.AddEdgeCollider (
                    Physics.PixelsToMeters(Position + Points[0]), 
                    Physics.PixelsToMeters(Position + Points[1]));

            return body.AddChainCollider(Position, Points, IsLoop);
        }
    }
}
