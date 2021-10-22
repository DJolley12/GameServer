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

        public void Update();
        private void Move()
        {

        }
    }
}
