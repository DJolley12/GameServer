using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
    public class GameLogic
    {
        public static void Update()
        {
            ThreadManager.UpdateMain();
        }
    }
}
