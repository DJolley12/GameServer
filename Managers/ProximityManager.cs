using System.Collections.Generic;
using GameServer;

public static class ProximityManager
{
    private const int playerProximityThreshold = 2000;
    public static List<Player> GroupedPlayers { get; set; }

    public static void AddPlayerIfInProximity(Client client, Client prevClient)
    {
        if (GroupedPlayers == null)
        {
            GroupedPlayers = new List<Player>();
        }

        if (HelperMethods.ObjIsOutOfScope(client.CPlayer.Position, prevClient.CPlayer.Position, playerProximityThreshold))
        {
            if (GroupedPlayers.Count > 2)
            {
                for (int i = 0; i < GroupedPlayers.Count; i++)
                {
                    if (!HelperMethods.ObjIsOutOfScope(client.CPlayer.Position, GroupedPlayers[i].Position, playerProximityThreshold)
                        && client.CPlayer.Id != GroupedPlayers[i].Id
                        && !GroupedPlayers.Contains(GroupedPlayers[i]))
                    {
                        GroupedPlayers.Add(GroupedPlayers[i]);
                        return;
                    }
                }
            }

            GroupedPlayers.Remove(client.CPlayer);
            EnvironmentManager.PlayerProximityChanged(client.CPlayer.Id);
        }
        else
        {
            GroupedPlayers.Add(client.CPlayer);
        }
    }
}

