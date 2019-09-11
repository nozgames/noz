using System;
using System.Collections.Generic;

namespace NoZ {
    public class BinPacker {
        public enum Method {
            BestShortSideFit,   ///< -BSSF: Positions the rectangle against the short side of a free rectangle into which it fits the best.
			BestLongSideFit,    ///< -BLSF: Positions the rectangle against the long side of a free rectangle into which it fits the best.
			BestAreaFit,        ///< -BAF: Positions the rectangle into the smallest free rect into which it fits.
			BottomLeftRule,     ///< -BL: Does the Tetris placement.
			ContactPointRule    ///< -CP: Choosest the placement where the rectangle touches other rects as much as possible.
		};

        /// Size of the bin packer
        private Vector2Int _size;

        /// Vector of used rectangles
        private List<RectInt> _used;

        /// Vector of free rectangles
        private List<RectInt> _free;

        public BinPacker() {
            _size = Vector2Int.Zero;
            _used = new List<RectInt>();
            _free = new List<RectInt>();
        }

        public BinPacker(int width, int height) : this() {
            Resize(width, height);
        }

        public bool IsEmpty => _used.Count == 0;

        public Vector2Int Size => _size;

        /// <summary>
        /// Returns the rectangle for the given index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public RectInt GetRect(int index) => _used[index];

        public void Resize(int width, int height) {
            _size.x = width;
            _size.y = height;

            _used.Clear();
            _free.Clear();
            _free.Add(new RectInt(1, 1, width - 2, height - 2));
        }

        public int Insert(in Vector2Int size, Method method, out RectInt outRect) {
            RectInt rect = RectInt.Empty;
            int score1 = 0;
            int score2 = 0;

            switch (method) {
                case Method.BestShortSideFit:
                    rect = FindPositionForNewNodeBestShortSideFit(size.x, size.y, ref score1, ref score2);
                    break;
                case Method.BottomLeftRule:
                    rect = FindPositionForNewNodeBottomLeft(size.x, size.y, ref score1, ref score2);
                    break;
                case Method.ContactPointRule:
                    rect = FindPositionForNewNodeContactPoint(size.x, size.y, ref score1);
                    break;
                case Method.BestLongSideFit:
                    rect = FindPositionForNewNodeBestLongSideFit(size.x, size.y, ref score2, ref score1);
                    break;
                case Method.BestAreaFit:
                    rect = FindPositionForNewNodeBestAreaFit(size.x, size.y, ref score1, ref score2);
                    break;
            }

            outRect = rect;

            if (rect.height == 0)
                return -1;

            return PlaceRect(rect);
        }

        private int PlaceRect(in RectInt rect) {
            int freeCount = _free.Count;
            for (int i = 0; i < freeCount; ++i) {
                if (SplitFreeNode(_free[i], rect)) {
                    _free.RemoveAt(i);
                    --i;
                    --freeCount;
                }
            }

            PruneFreeList();

            _used.Add(rect);

            return _used.Count - 1;
        }

        private RectInt ScoreRect(in Vector2Int size, Method method, ref int score1, ref int score2) {
            RectInt rect = RectInt.Empty;
            score1 = int.MaxValue;
            score2 = int.MaxValue;

            switch (method) {
                case Method.BestShortSideFit:
                    rect = FindPositionForNewNodeBestShortSideFit(size.x, size.y, ref score1, ref score2);
                    break;
                case Method.BottomLeftRule:
                    rect = FindPositionForNewNodeBottomLeft(size.x, size.y, ref score1, ref score2);
                    break;
                case Method.ContactPointRule:
                    rect = FindPositionForNewNodeContactPoint(size.x, size.y, ref score1);
                    score1 = -score1; // Reverse since we are minimizing, but for contact point score bigger is better.
                    break;
                case Method.BestLongSideFit:
                    rect = FindPositionForNewNodeBestLongSideFit(size.x, size.y, ref score2, ref score1);
                    break;
                case Method.BestAreaFit:
                    rect = FindPositionForNewNodeBestAreaFit(size.x, size.y, ref score1, ref score2);
                    break;
            }

            // Cannot fit the current rectangle.
            if (rect.height == 0) {
                score1 = int.MaxValue;
                score2 = int.MaxValue;
            }

            return rect;
        }

        /// Computes the ratio of used surface area.
        private float GetOccupancy() {
            ulong area = 0;
            for(int i=0; i<_used.Count; i++)
                area += (ulong)_used[i].width * (ulong)_used[i].height;

            return (float)area / (_size.x * _size.y);
        }

        RectInt FindPositionForNewNodeBottomLeft(int width, int height, ref int bestY, ref int bestX) {
            RectInt rect = RectInt.Empty;

            bestY = int.MaxValue;

            for (int i = 0; i < _free.Count; ++i) {
                // Try to place the rectangle in upright (non-flipped) orientation.
                if (_free[i].width >= width && _free[i].height >= height) {
                    int topSideY = _free[i].y + height;
                    if (topSideY < bestY || (topSideY == bestY && _free[i].x < bestX)) {
                        rect.x = _free[i].x;
                        rect.y = _free[i].y;
                        rect.width = width;
                        rect.height = height;
                        bestY = topSideY;
                        bestX = _free[i].x;
                    }
                }
                if (_free[i].width >= height && _free[i].height >= width) {
                    int topSideY = _free[i].y + width;
                    if (topSideY < bestY || (topSideY == bestY && _free[i].x < bestX)) {
                        rect.x = _free[i].x;
                        rect.y = _free[i].y;
                        rect.width = width;
                        rect.height = height;
                        bestY = topSideY;
                        bestX = _free[i].x;
                    }
                }
            }
            return rect;
        }

        RectInt FindPositionForNewNodeBestShortSideFit(
            int width,
            int height,
            ref int bestShortSideFit,
            ref int bestLongSideFit) {

            RectInt rect = RectInt.Empty;

            bestShortSideFit = int.MaxValue;

            for (int i = 0; i < _free.Count; ++i) {
                // Try to place the rectangle in upright (non-flipped) orientation.
                if (_free[i].width >= width && _free[i].height >= height) {
                    int leftoverHoriz = Math.Abs(_free[i].width - width);
                    int leftoverVert = Math.Abs(_free[i].height - height);
                    int shortSideFit = Math.Min(leftoverHoriz, leftoverVert);
                    int longSideFit = Math.Max(leftoverHoriz, leftoverVert);

                    if (shortSideFit < bestShortSideFit || (shortSideFit == bestShortSideFit && longSideFit < bestLongSideFit)) {
                        rect.x = _free[i].x;
                        rect.y = _free[i].y;
                        rect.width = width;
                        rect.height = height;
                        bestShortSideFit = shortSideFit;
                        bestLongSideFit = longSideFit;
                    }
                }

                if (_free[i].width >= height && _free[i].height >= width) {
                    int flippedLeftoverHoriz = Math.Abs(_free[i].width - height);
                    int flippedLeftoverVert = Math.Abs(_free[i].height - width);
                    int flippedShortSideFit = Math.Min(flippedLeftoverHoriz, flippedLeftoverVert);
                    int flippedLongSideFit = Math.Max(flippedLeftoverHoriz, flippedLeftoverVert);

                    if (flippedShortSideFit < bestShortSideFit || (flippedShortSideFit == bestShortSideFit && flippedLongSideFit < bestLongSideFit)) {
                        rect.x = _free[i].x;
                        rect.y = _free[i].y;
                        rect.width = height;
                        rect.height = width;
                        bestShortSideFit = flippedShortSideFit;
                        bestLongSideFit = flippedLongSideFit;
                    }
                }
            }
            return rect;
        }

        RectInt FindPositionForNewNodeBestLongSideFit(
            int width,
            int height,
            ref int bestShortSideFit,
            ref int bestLongSideFit) {

            RectInt rect = RectInt.Empty;

            bestLongSideFit = int.MaxValue;

            for (int i = 0; i < _free.Count; ++i) {
                // Try to place the rectangle in upright (non-flipped) orientation.
                if (_free[i].width >= width && _free[i].height >= height) {
                    int leftoverHoriz = Math.Abs(_free[i].width - width);
                    int leftoverVert = Math.Abs(_free[i].height - height);
                    int shortSideFit = Math.Min(leftoverHoriz, leftoverVert);
                    int longSideFit = Math.Max(leftoverHoriz, leftoverVert);

                    if (longSideFit < bestLongSideFit || (longSideFit == bestLongSideFit && shortSideFit < bestShortSideFit)) {
                        rect.x = _free[i].x;
                        rect.y = _free[i].y;
                        rect.width = width;
                        rect.height = height;
                        bestShortSideFit = shortSideFit;
                        bestLongSideFit = longSideFit;
                    }
                }
                /*
                    if (_free[i].width >= height && _free[i].height >= width)
                    {
                        int leftoverHoriz = Math.Abs(_free[i].width - height);
                        int leftoverVert = Math.Abs(_free[i].height - width);
                        int shortSideFit = Math.Min(leftoverHoriz, leftoverVert);
                        int longSideFit = Math.Max(leftoverHoriz, leftoverVert);

                        if (longSideFit < bestLongSideFit || (longSideFit == bestLongSideFit && shortSideFit < bestShortSideFit))
                        {
                            rect.x = _free[i].x;
                            rect.y = _free[i].y;
                            rect.width = height;
                            rect.height = width;
                            bestShortSideFit = shortSideFit;
                            bestLongSideFit = longSideFit;
                        }
                    }
            */

            }
            return rect;
        }

        RectInt FindPositionForNewNodeBestAreaFit(
            int width,
            int height,
            ref int bestAreaFit,
            ref int bestShortSideFit) {

            RectInt rect = RectInt.Empty;

            bestAreaFit = int.MaxValue;

            for (int i = 0; i < _free.Count; ++i) {
                int areaFit = _free[i].width * _free[i].height - width * height;

                // Try to place the rectangle in upright (non-flipped) orientation.
                if (_free[i].width >= width && _free[i].height >= height) {
                    int leftoverHoriz = Math.Abs(_free[i].width - width);
                    int leftoverVert = Math.Abs(_free[i].height - height);
                    int shortSideFit = Math.Min(leftoverHoriz, leftoverVert);

                    if (areaFit < bestAreaFit || (areaFit == bestAreaFit && shortSideFit < bestShortSideFit)) {
                        rect.x = _free[i].x;
                        rect.y = _free[i].y;
                        rect.width = width;
                        rect.height = height;
                        bestShortSideFit = shortSideFit;
                        bestAreaFit = areaFit;
                    }
                }

                if (_free[i].width >= height && _free[i].height >= width) {
                    int leftoverHoriz = Math.Abs(_free[i].width - height);
                    int leftoverVert = Math.Abs(_free[i].height - width);
                    int shortSideFit = Math.Min(leftoverHoriz, leftoverVert);

                    if (areaFit < bestAreaFit || (areaFit == bestAreaFit && shortSideFit < bestShortSideFit)) {
                        rect.x = _free[i].x;
                        rect.y = _free[i].y;
                        rect.width = height;
                        rect.height = width;
                        bestShortSideFit = shortSideFit;
                        bestAreaFit = areaFit;
                    }
                }
            }
            return rect;
        }

        /// Returns 0 if the two intervals i1 and i2 are disjoint, or the length of their overlap otherwise.
        int CommonIntervalLength(int i1start, int i1end, int i2start, int i2end) {
            if (i1end < i2start || i2end < i1start)
                return 0;
            return Math.Min(i1end, i2end) - Math.Max(i1start, i2start);
        }

        private int ContactPointScoreNode(int x, int y, int width, int height) {
            int score = 0;

            if (x == 0 || x + width == _size.x)
                score += height;
            if (y == 0 || y + height == _size.y)
                score += width;

            for (int i = 0; i < _used.Count; ++i) {
                if (_used[i].x == x + width || _used[i].x + _used[i].width == x)
                    score += CommonIntervalLength(_used[i].y, _used[i].y + _used[i].height, y, y + height);
                if (_used[i].y == y + height || _used[i].y + _used[i].height == y)
                    score += CommonIntervalLength(_used[i].x, _used[i].x + _used[i].width, x, x + width);
            }

            return score;
        }

        RectInt FindPositionForNewNodeContactPoint(int width, int height, ref int bestContactScore) {
            RectInt rect = RectInt.Empty;

            bestContactScore = -1;

            for (int i = 0; i < _free.Count; ++i) {
                // Try to place the rectangle in upright (non-flipped) orientation.
                if (_free[i].width >= width && _free[i].height >= height) {
                    int score = ContactPointScoreNode(_free[i].x, _free[i].y, width, height);
                    if (score > bestContactScore) {
                        rect.x = _free[i].x;
                        rect.y = _free[i].y;
                        rect.width = width;
                        rect.height = height;
                        bestContactScore = score;
                    }
                }
                if (_free[i].width >= height && _free[i].height >= width) {
                    int score = ContactPointScoreNode(_free[i].x, _free[i].y, width, height);
                    if (score > bestContactScore) {
                        rect.x = _free[i].x;
                        rect.y = _free[i].y;
                        rect.width = height;
                        rect.height = width;
                        bestContactScore = score;
                    }
                }
            }
            return rect;
        }

        bool SplitFreeNode(RectInt freeNode, in RectInt usedNode) {
            // Test with SAT if the rectangles even intersect.
            if (usedNode.x >= freeNode.x + freeNode.width || usedNode.x + usedNode.width <= freeNode.x ||
               usedNode.y >= freeNode.y + freeNode.height || usedNode.y + usedNode.height <= freeNode.y)
                return false;

            if (usedNode.x < freeNode.x + freeNode.width && usedNode.x + usedNode.width > freeNode.x) {
                // New node at the top side of the used node.
                if (usedNode.y > freeNode.y && usedNode.y < freeNode.y + freeNode.height) {
                    RectInt newNode = freeNode;
                    newNode.height = usedNode.y - newNode.y;
                    _free.Add(newNode);
                }

                // New node at the bottom side of the used node.
                if (usedNode.y + usedNode.height < freeNode.y + freeNode.height) {
                    RectInt newNode = freeNode;
                    newNode.y = usedNode.y + usedNode.height;
                    newNode.height = freeNode.y + freeNode.height - (usedNode.y + usedNode.height);
                    _free.Add(newNode);
                }
            }

            if (usedNode.y < freeNode.y + freeNode.height && usedNode.y + usedNode.height > freeNode.y) {
                // New node at the left side of the used node.
                if (usedNode.x > freeNode.x && usedNode.x < freeNode.x + freeNode.width) {
                    RectInt newNode = freeNode;
                    newNode.width = usedNode.x - newNode.x;
                    _free.Add(newNode);
                }

                // New node at the right side of the used node.
                if (usedNode.x + usedNode.width < freeNode.x + freeNode.width) {
                    RectInt newNode = freeNode;
                    newNode.x = usedNode.x + usedNode.width;
                    newNode.width = freeNode.x + freeNode.width - (usedNode.x + usedNode.width);
                    _free.Add(newNode);
                }
            }

            return true;
        }

        private bool IsContainedIn(in RectInt a, in RectInt b) {
			return a.x>=b.x && a.y>=b.y && a.x+a.width<=b.x+b.width && a.y+a.height<=b.y+b.height;
		}

        private void PruneFreeList() {
            // Remoe redundance rectangles
            for (int i = 0; i < _free.Count; ++i) {
                for (int j = i + 1; j < _free.Count; ++j) {
                    if (IsContainedIn(_free[i], _free[j])) {
                        _free.RemoveAt(i);
                        --i;
                        break;
                    }
                    if (IsContainedIn(_free[j], _free[i])) {
                        _free.RemoveAt(j);
                        --j;
                    }
                }
            }
        }
    }
}
