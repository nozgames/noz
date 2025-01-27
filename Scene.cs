/*

    Copyright (c) 2025 NoZ Games, LLC. All rights reserved.

*/

using Box2D.NetStandard.Collision;
using Box2D.NetStandard.Dynamics.Contacts;
using Box2D.NetStandard.Dynamics.World;
using Box2D.NetStandard.Dynamics.World.Callbacks;
using NoZ.Nodes;
using NoZ.Nodes.Physics;
using System.Numerics;

namespace NoZ
{
    public class Scene : IDisposable
    {
        private struct Collision
        {
            public bool IsEnter;
            public Collider2D colliderA;
            public Collider2D colliderB;
        }

        private class SceneContactListener : ContactListener
        {
            public Scene _scene;

            public override void BeginContact(in Contact contact)
            {
                var colliderA = (Collider2D)contact.FixtureA.UserData;
                var colliderB = (Collider2D)contact.FixtureB.UserData;
                _scene._collisions.Enqueue(new Collision { IsEnter = true, colliderA = colliderA, colliderB = colliderB });
            }

            public override void EndContact(in Contact contact)
            {
                var colliderA = (Collider2D)contact.FixtureA.UserData;
                var colliderB = (Collider2D)contact.FixtureB.UserData;
                _scene._collisions.Enqueue(new Collision { IsEnter = false, colliderA = colliderA, colliderB = colliderB });
            }

            public override void PostSolve(in Contact contact, in ContactImpulse impulse)
            {
            }

            public override void PreSolve(in Contact contact, in Manifold oldManifold)
            {
            }
        }

        private Queue<Collision> _collisions;
        private World? _world;
        private float _fixedTimeElapsed;
        private float _fixedTimeStep;
        private Node _node;
        private Node? _rootNode;
        private List<IRenderable> _renderables;
        private List<IUpdatable> _updatables;
        private List<IUpdatable> _tempUpdatables;
        private List<IFixedUpdate> _fixedUpdates;
        private List<IFixedUpdate> _tempFixedUpdates;

        internal World Physics => _world!;

        public Node? RootNode
        {
            get => _rootNode;
            set
            {
                if (_rootNode == value)
                    return;

                _rootNode = value;
                if (_rootNode != null)
                    _rootNode.Dispose();

                _rootNode = value;
                if (_rootNode != null)
                    _rootNode.SetParent(_node);
            }
        }

        public Scene(int physicsFramesPerSecond=60)
        {
            _collisions = new Queue<Collision>();

            _world = new World();
            _world.SetGravity(Vector2.Zero);
            _world.SetContactListener(new SceneContactListener { _scene = this });

            _node = new Node();
            _node._scene = this;
            _renderables = new List<IRenderable>(128);
            _updatables = new List<IUpdatable>(128);
            _tempUpdatables = new List<IUpdatable>(128);
            _fixedUpdates = new List<IFixedUpdate>(128);
            _tempFixedUpdates = new List<IFixedUpdate>(128);
            _fixedTimeStep = 1.0f / physicsFramesPerSecond;
        }

        public void Dispose()
        {            
            _world = null;
            _node = null;
        }

        public void Render()
        {
            foreach(var renderable in _renderables)
                renderable.Render();
        }

        public void Update()
        {
            _tempUpdatables.Clear();
            _tempUpdatables.AddRange(_updatables);

            foreach (var updatable in _tempUpdatables)
                updatable.Update();
        }

        public void FixedUpdate()
        {
            _fixedTimeElapsed += Time.DeltaTime;
            while (_fixedTimeElapsed >= _fixedTimeStep)
            {
                _fixedTimeElapsed -= _fixedTimeStep;
                Time.BeginFixed(_fixedTimeStep);

                _world!.Step(_fixedTimeStep, 8, 3);

                while (_collisions.Count > 0)
                {
                    var collision = _collisions.Dequeue();

                    if (collision.IsEnter)
                    {
                        collision.colliderA.OnCollisionEnter(collision.colliderB);
                        collision.colliderB.OnCollisionEnter(collision.colliderA);
                    }
                    else
                    {
                        collision.colliderA.OnCollisionExit(collision.colliderB);
                        collision.colliderB.OnCollisionExit(collision.colliderA);
                    }
                }

                _tempFixedUpdates.Clear();
                _tempFixedUpdates.AddRange(_fixedUpdates);

                foreach (var fixedUpdate in _tempFixedUpdates)
                    fixedUpdate.FixedUpdate();

                Time.EndFixed();
            }
        }

        internal void RegisterNode(Node node)
        {
            if (node is IRenderable renderable)
                _renderables.Add(renderable);

            if (node is IUpdatable updatable)
                _updatables.Add(updatable);

            if (node is IFixedUpdate fixedUpdate)
                _fixedUpdates.Add(fixedUpdate);
        }

        internal void UnregisterNode(Node node)
        {
            if (node is IRenderable renderable)
                _renderables.Remove(renderable);

            // TODO: this is slow, linked list?
            if (node is IUpdatable updatable)
                _updatables.Remove(updatable);

            if (node is IFixedUpdate fixedUpdate)
                _fixedUpdates.Remove(fixedUpdate);
        }
    }
}
