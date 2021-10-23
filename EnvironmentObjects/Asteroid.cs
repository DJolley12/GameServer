using System;
using System.Numerics;

namespace GameServer
{
    public class Asteroid : IEnvironmentObject
    {
        public int MaxAllowable { get; set; }
        public int Id { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 Velocity { get; set; }
        public Vector3 OwnerPosition { get; set; }

        public Asteroid(int id, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 ownerPosition)
        {
            Id = id;
            Position = position;
            Rotation = rotation;
            Velocity = velocity;
            OwnerPosition = ownerPosition;
        }

        public Asteroid(int id, float range, Vector3 ownerPosition, float maxTorque)
        {
            Id = id;
            // TODO: find position based on player, and range ahead of direction, so 
            // asteroid doesn't spawn on player or too close
            var torque = GetRandom(maxTorque);
            Rotation = new Quaternion();
            // TODO: assign rotation based on torque
            Velocity = new Vector3(GetRandom(range), GetRandom(range), 0);
            OwnerPosition = ownerPosition;
        }

        public void Update()
        {
            Position += Velocity;
            // TODO: rotate with torque
            ServerSend.EnvironmentObjectPosition(this);
            ServerSend.EnvironmentObjectRotation(this);
        }

        private void Move()
        {
        }

        private float GetRandom(float range)
        {
            var random = new Random();
            return random.Next((int)-range, (int)range);
        }
    }
}
