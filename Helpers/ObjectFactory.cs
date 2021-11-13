using System;
using GameServer;

public static class ObjectFactory
{
    public static object Instantiate(string typeString, object[] ctorParams)
    {
        var assembly = typeof(ServerHandle).Assembly;
        var type = assembly.GetType($"GameServer.{typeString}");
        var ctors = type.GetConstructors();
        var newObject = ctors[0].Invoke(ctorParams);

        return newObject; 
    }
}
