using System;
using System.Numerics;

namespace GameServer
{
    public class EnvironmentManager
    {
        private float asteroidMaxThrust { get; set; }
        private float asteroidMaxTorque { get; set; }

        public void UpdateEnvironment()
        {
            AssignAsteroidNumbers();
        }

        private void AssignAsteroidNumbers()
        {
            foreach (Client client in Server.Clients.Values)
            {
                if (client.CPlayer != null)
                {
                    var position = client.CPlayer.Position;
                    // assign different max number depending on 
                    // level of cluster the asteroids are
                    // if position is greater than certain distance from
                    // player, check if other player is near. => reassign or remove

                }
            }
        }
    }
}
