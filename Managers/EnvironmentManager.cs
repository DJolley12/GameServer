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
                outOfScopeClientIds = new List<int>();
            }
                
            outOfScopeClientIds = outOfScopeClientIds.Append(playerId);
            // if prox changed, object that player is subscribed to but does not own
            // needs to be removed
            //TODO: check all objects for playerId to make sure it's destroyed if
            //add player to out of scope ids - later need to check this list and destroy objects
            //on players client that player does not own
        }

        //public static void Update()
        //{
        //    for (int i = 0; i < EnvironmentObjects.Count; i++)
        //    {
        //        var environmentObject = EnvironmentObjects.ElementAt(i);
        //        var ownerId = environmentObject.Value.OwnerId;
        //        var ownerPosition = Server.Clients[ownerId].CPlayer.Position;
        //        var ownerDiff = ownerPosition - environmentObject.Value.Position;

        //        // Remove object if owner is out of scope, and no other players in scope
        //        if (HelperMethods.ObjIsOutOfScope(ownerPosition, environmentObject.Value.Position, 1000))
        //        {
        //            environmentObject.Value.OwnerId = -1;

        //            foreach (var client in Server.Clients.Values)
        //            {
        //                var diff = client.CPlayer.Position - environmentObject.Value.Position;
        //                if (!HelperMethods.ObjIsOutOfScope(client.CPlayer.Position, environmentObject.Value.Position, 1000))
        //                {
        //                    environmentObject.Value.OwnerId = client.Id;
        //                }
        //                else 
        //                {
        //                    environmentObject.Value.SubscribedClientIds.Remove(client)
        //                }
        //            }

        //            if (environmentObject.Value.OwnerId < 0)
        //            {
        //                // Remove, client side will handle out of scope on their own

        //                EnvironmentObjects.Remove(environmentObject.Key);
        //            }
        //        }


        //        //for (int j = 0; j < ProximityManager.GroupedPlayers.Count(); j++)
        //        //{
        //        //    var playerId = ProximityManager.GroupedPlayers.ElementAt(i).Id;
        //        //    if (environmentObject.Value.SubscribedClientIds.Contains(playerId))
        //        //    {
        //        //        environmentObject.Value.SubscribedClientIds.Remove(playerId);
        //        //    }
        //        //}
        //        environmentObject.Value.Update();
        //    }
        //}

        public static void Update()
        {
            for (int i = 0; i < EnvironmentObjects.Count; i++)
            {
                var environmentObject = EnvironmentObjects.ElementAt(i);
                var clientCount = Server.Clients.Count();
                for (int s = 0; s < clientCount; s++)
                {
                    var player = Server.Clients.ElementAt(s).Value.CPlayer;

                    // Need to remove out of scope items so server does not keep the in memory
                    // indefinitely.
                    if (HelperMethods.ObjIsOutOfScope(player.Position, environmentObject.Value.Position, 1000))
                    {
                        // Object owner is out of scope, assign -1 to signal reassignment
                        environmentObject.Value.SubscribedClientIds.Remove(player.Id);
                        if (player.Id == environmentObject.Value.OwnerId)
                        {
                            environmentObject.Value.OwnerId = -1;
                        }
                    }
                    // Object is in scope, need to ensure player is sub'd
                    else if (!environmentObject.Value.SubscribedClientIds.Contains(player.Id))
                    {
                        environmentObject.Value.SubscribedClientIds.Add(player.Id);
                    }
                }

                // Object no longer has subscribers, remove from dictionary
                if (environmentObject.Value.SubscribedClientIds.Count() < 1)
                {
                    EnvironmentObjects.Remove(environmentObject.Key);
                }
                // Assign first subscriber as owner
                else if (environmentObject.Value.OwnerId == -1)
                {
                    environmentObject.Value.OwnerId = environmentObject.Value.SubscribedClientIds[0];
                }

                environmentObject.Value.Update();
            }
        }

        internal static void AddNewEnvObject(int objId, int ownerId, string typeString, Vector3 position, Quaternion rotation, Vector3 velocity, float torque)
        {
            objId = EnvironmentManager.GetNewId();
            var newObject = ObjectFactory.Instantiate(typeString, new object[] { objId, ownerId, position, rotation, velocity, torque });
            EnvironmentObjects.Add(objId, (IEnvironmentObject)newObject);
        }

        public static int GetNewId() => currentId > 2000 ? 1 : currentId += 1;
    }
}
