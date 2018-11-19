using System;
using System.Threading.Tasks;
using Urho;
using Urho.Physics;

namespace Fish
{
    /// <summary>
    /// A base class for all aircrafts including the player and enemies
    /// </summary>
    public abstract class Player : Component
    {
        TaskCompletionSource<bool> liveTask;

        protected Player()
        {
            ReceiveSceneUpdates = true;
        }

        /// <summary>
        /// Spawn the aircraft and wait until it's exploded
        /// </summary>
        public Task Play()
        {
            liveTask = new TaskCompletionSource<bool>();
            Health = MaxHealth;
            var node = Node;

            // Define physics for handling collisions
            var body = node.CreateComponent<RigidBody>();
            body.Mass = 1;
            body.Kinematic = true;
            body.CollisionMask = (uint)CollisionLayer;
            CollisionShape shape = node.CreateComponent<CollisionShape>();
            shape.SetBox(CollisionShapeSize, Vector3.Zero, Quaternion.Identity);

            Init();
            return liveTask.Task;
        }

        protected virtual void Init() { }

        protected virtual CollisionLayers CollisionLayer => CollisionLayers.Enemy;

        protected virtual Vector3 CollisionShapeSize => new Vector3(1.2f, 1.2f, 1.2f);
    }

    public enum CollisionLayers : uint
    {
        Player = 2,
        Enemy = 4
    }
}