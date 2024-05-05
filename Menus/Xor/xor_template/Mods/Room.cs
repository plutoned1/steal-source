using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ark.ModHandler;


namespace ark.Mods
{
    internal class Room
    {
        public static void Disconnect()
        {
            PhotonNetwork.Disconnect();
            Disable("Disconnect");
        }
        public static void JoinRandom()
        {
            PhotonNetwork.JoinRandomRoom();
            if (PhotonNetwork.InRoom) Disable("Join Random");
        }
    }
}
