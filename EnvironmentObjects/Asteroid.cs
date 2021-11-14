using System;
using System.Collections.Generic;
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
        public float Torque { get; set; }
        public int OwnerId { get; set; }
        public List<int> SubscribedClientIds { get; set; }

        public Asteroid(int id, int ownerId, Vector3 position, Quaternion rotation, Vector3 velocity, float torque)
        {
            Id = id;
            OwnerId = ownerId;
            Position = position;
            Rotation = rotation;
            Velocity = velocity;
            Torque = torque;
        }

        public void Update()
        {
            for (int i = 0; i < SubscribedClientIds.Count; i++)
            {
                ServerSend.EnvironmentObject(SubscribedClientIds[i], this);
            }
        }

        private void Move()
        {
        }

        private float GetRandom(float range)
        {
            var random = new Random();
            return random.Next((int)-range, (int)range);
        }

        public void SetValues(Vector3 position, Quaternion rotation, Vector3 velocity, float torque)
        {
            Position = position;
            Rotation = rotation;
            Velocity = velocity;
            Torque = torque;
        }
    }
}
