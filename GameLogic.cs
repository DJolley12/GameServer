namespace GameServer
{
    public class GameLogic
    {
        public static void Update()
        {
            Client prevClient = null;
            foreach (Client client in Server.Clients.Values)
            {
                if (client.CPlayer != null)
                {
                    client.CPlayer.Update();
                    if (prevClient != null)
                    {
                        //compare prevClient/client pos
                        ProximityManager.AddPlayerIfInProximity(client, prevClient);
                    }

                    prevClient = client;
                }
            }

            EnvironmentManager.Update();

            ThreadManager.UpdateMain();
        }
    }
}
