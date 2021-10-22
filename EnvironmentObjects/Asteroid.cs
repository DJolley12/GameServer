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

        public void Update()
        {
        }

        private void Move()
        {
        }
    }
}
