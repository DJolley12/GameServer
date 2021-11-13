using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace GameServer
{
    public static class EnvironmentManager
    {
        private static float asteroidMaxThrust { get; set; }
        private static float asteroidMaxTorque { get; set; }
        private static int currentId { get; set; }
        public static Dictionary<int, IEnvironmentObject> EnvironmentObjects { get; set; }
        private static IEnumerable<int> outOfScopeClientIds { get; set; }
        private static int threshold = 1000;

        public static void UpdateEnvironment()
        {
            AssignAsteroidNumbers();
        }

        private static void AssignAsteroidNumbers()
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

        internal static void PlayerProximityChanged(int playerId)
        {
            if (outOfScopeClientIds == null)
            {
                outOfScopeClientIds = new int[] {};
            }
                
            outOfScopeClientIds = outOfScopeClientIds.Append(playerId);
            // if prox changed, object that player is subscribed to but does not own
            // needs to be removed
            //TODO: check all objects for playerId to make sure it's destroyed if
            //add player to out of scope ids - later need to check this list and destroy objects
            //on players client that player does not own
        }

        public static void Update()
        {
            for (int i = 0; i < EnvironmentObjects.Count; i++)
            {
                var environmentObject = EnvironmentObjects.ElementAt(i);
                var ownerId = environmentObject.Value.OwnerId;
                var ownerPosition = Server.Clients[ownerId].CPlayer.Position;
                var ownerDiff = ownerPosition - environmentObject.Value.Position;

                if (HelperMethods.ObjIsOutOfScope(ownerPosition, environmentObject.Value.Position, 1000))
                {
                    environmentObject.Value.OwnerId = -1;

                    foreach (var client in Server.Clients.Values)
                    {
                        var diff = client.CPlayer.Position - environmentObject.Value.Position;
                        if (!HelperMethods.ObjIsOutOfScope(client.CPlayer.Position, environmentObject.Value.Position, 1000))
                        {
                            environmentObject.Value.OwnerId = client.Id;
                        }
                    }

                    if (environmentObject.Value.OwnerId < 0)
                    {
                        // Remove, client side will handle out of scope on their own

                        EnvironmentObjects.Remove(environmentObject.Key);
                        // TODO: send destroy message to clients
                    }
                }

                for (int j = 0; j < ProximityManager.GroupedPlayers.Count(); j++)
                {
                    var playerId = ProximityManager.GroupedPlayers.ElementAt(i).Id;
                    if (environmentObject.Value.SubscribedClientIds.Contains(playerId))
                    {
                        environmentObject.Value.SubscribedClientIds.Remove(playerId);
                    }
                }
                environmentObject.Value.Update();
            }
        }

        internal static void AddNewEnvObject(int objId, int ownerId, string typeString, Vector3 position, Quaternion rotation, Vector3 velocity)
        {
            objId = EnvironmentManager.GetNewId();
            var newObject = ObjectFactory.Instantiate(typeString, new object[] { objId, ownerId, position, rotation, velocity });
            EnvironmentObjects.Add(objId, (IEnvironmentObject)newObject);
        }

        public static int GetNewId() => currentId > 2000 ? 1 : currentId += 1;
    }
}
