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

            ThreadManager.UpdateMain();
        }
    }
}
