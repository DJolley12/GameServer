namespace GameServer
{
    public class GameLogic
    {
        public static void Update()
        {
            foreach (Client client in Server.Clients.Values)
            {
                if (client.CPlayer != null)
                {
                    client.CPlayer.Update();
                }
            }

            foreach (Client client in Server.Clients.Values)
            {
                foreach (var environmentObject in client.EnvironmentObjects)
                {
                    environmentObject.Update();
                }
            }

            ThreadManager.UpdateMain();
        }
    }
}
