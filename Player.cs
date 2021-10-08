using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace GameServer
{
    public class Player
    {
        public int Id { get; set; }
        public string Username { get; set; }

        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }

        private float moveSpeed = 5f / Constants.TICKS_PER_SEC;
        private bool[] inputs;

        public Player(int id, string username, Vector3 spawnPosition)
        {
            Id = id;
            Username = username;
            Position = spawnPosition;
            Rotation = Quaternion.Identity;

            inputs = new bool[4];
        }

        public void Update()
        {
            // TODO: Add SpaceShipControls script logic to add velocity
            // based on input
            
        }

        public void SetInput(bool[] _inputs, Quaternion rotation, Vector3 position)
        {
            inputs = _inputs;
            Rotation = rotation;
            Position = position;
        }
    }
}
