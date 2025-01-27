/*

    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

using System.Numerics;

namespace NoZ
{
    /// <summary>
    /// Represents a node with a 2d transform
    /// </summary>
    public class Node2D : Node
    {
        private Vector2 _localPosition;
        private Vector2 _localScale = Vector2.One;
        private float _localRotation;
        private Matrix3x2 _localToWorld = Matrix3x2.Identity;
        private Matrix3x2 _worldToLocal = Matrix3x2.Identity;
        private bool _transformsDirty = true;

        private Vector2 _position;

        public Vector2 Forward
        {
            get
            {
                if (_transformsDirty)
                    UpdateTransforms();
                
                return new Vector2(-_localToWorld.M21, -_localToWorld.M22);
            }
        }

        public Vector2 LocalPosition
        {
            get => _localPosition;
            set
            {
                if (_localPosition == value)
                    return;

                _localPosition = value;
                InvalidateTransforms();
            }
        }

        public float LocalRotation
        {
            get => _localRotation;
            set
            {
                if (_localRotation == value)
                    return;

                _localRotation = value;
                InvalidateTransforms();
            }
        }

        public Vector2 LocalScale
        {
            get => _localScale;
            set
            {
                if (_localScale == value)
                    return;

                _localScale = value;
                InvalidateTransforms();
            }
        }

        public Vector2 Position
        {
            get
            {
                if (_transformsDirty)
                    UpdateTransforms();

                return _position;
            }

            set
            {
                if (_transformsDirty)
                    UpdateTransforms();

                if (Parent != null && Parent is Node2D parent)
                    LocalPosition = Vector2.Transform(value, parent._worldToLocal);
                else
                    _localPosition = value;

                _transformsDirty = true;
            }
        }

        public Matrix3x2 LocalToWorld
        {
            get
            {
                if (_transformsDirty)
                    UpdateTransforms();

                return _localToWorld;
            }
        }

        public Vector2 LookAt(Vector2 target)
        {
            var direction = target - Position;
            var angle = MathEx.RAD2DEG * MathF.Atan2(direction.Y, direction.X) + 90;
            LocalRotation = angle;
            return direction;
        }

        public Vector2 TransformPoint(Vector2 point)
        {
            if (_transformsDirty)
                UpdateTransforms();

            return Vector2.Transform(point, _localToWorld);
        }

        private void InvalidateTransforms()
        {
            if (_transformsDirty)
                return;

            _transformsDirty = true;
            
            for (int childIndex=0, childCount=ChildCount; childIndex<childCount; childIndex++)
            {
                var child = GetChild(childIndex);
                if (child is Node2D node2D)
                    node2D.InvalidateTransforms();
            }
        }

        private void UpdateTransforms()
        {
            if (Parent != null && Parent is Node2D node2D && node2D._transformsDirty)
                node2D.UpdateTransforms();

            _localToWorld = Matrix3x2.CreateScale(_localScale) *
                            Matrix3x2.CreateRotation(MathEx.DEG2RAD * _localRotation) *
                            Matrix3x2.CreateTranslation(_localPosition);

            var parent = Parent as Node2D;
            if (parent != null)
                _localToWorld *= parent._localToWorld;

            Matrix3x2.Invert(_localToWorld, out _worldToLocal);

            _position = Vector2.Transform(Vector2.Zero, _localToWorld);
            _transformsDirty = false;
        }
    }
}
