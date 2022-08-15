using System.Collections.Generic;
using System.Numerics;

namespace GameServer
{
    public interface IEnvironmentObject
    {
        public int MaxAllowable { get; set; }
        public int Id { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 Velocity { get; set; }
        public float Torque { get; set; }
        public int OwnerId { get; set; }
        public List<int> SubscribedClientIds { get; set; }

        public void Update();
        public void SetValues(Vector3 position, Quaternion rotation, Vector3 velocity, float torque, string typeString);
        private void Move()
        {

        }
    }
}
