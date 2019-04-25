using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkRift.Client;
using System.Net;
using System;
using DarkRift;
using Scripts.Models;
using UnityEngine.SceneManagement;

namespace Script.Networking{

	public class NetworkingManager
    {
        private static NetworkingManager instance;
        private DarkRiftClient client;
        public bool GotMatch = false;

        public static NetworkingManager Instance
        {
            get {
                if(instance == null)
                {
                    instance = new NetworkingManager();
                }
                return instance;
            }
        }

        private NetworkingManager()
        {
            client = new DarkRiftClient();
        }

        public ConnectionState ConnectionState
        {
            get
            {
                return client.ConnectionState; 
            }
        }

        public bool IsConnected
        {
            get
            {
                return client.ConnectionState == ConnectionState.Connected;
            }
        }

        public bool Connect()
        {
            if (client.ConnectionState == DarkRift.ConnectionState.Connecting)
            {
                return false;
            }
            if (client.ConnectionState == DarkRift.ConnectionState.Connected)
            {
                return true;
            }
            try
            {
                client.Connect(IPAddress.Parse("192.168.43.41"), 4296, DarkRift.IPVersion.IPv4);
                client.MessageReceived += OnMessageReceived;
                return true;
            }
            catch(Exception)
            {

            }
            return false;
        }

        public void MessageNameToServer(string name)
        {
            if (IsConnected)
            {
                using (DarkRiftWriter writer = DarkRiftWriter.Create())
                {
                    writer.Write(name);

                    using (Message message = Message.Create((ushort)Tags.Tag.SET_NAME, writer))
                    {
                        client.SendMessage(message, SendMode.Reliable);
                    }
                }
            }
        }

        public void MessageSlateTaken (ushort slateIndex, ushort matchID)
        {
            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                writer.Write(matchID);
                writer.Write(slateIndex);
                using (Message message = Message.Create((ushort)Tags.Tag.SLATE_TAKEN, writer))
                {
                    client.SendMessage(message, SendMode.Reliable);
                }
            }
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            switch ((Tags.Tag)e.Tag)
            {
                case Tags.Tag.GOT_MATCH:
                    using (Message msg = e.GetMessage())
                    {
                        using (DarkRiftReader reader = msg.GetReader())
                        {
                            ushort matchID = reader.ReadUInt16();
                            MatchModel.currentMatch = new MatchModel(matchID);

                            //SceneManager.LoadScene("Play");
                            //Debug.Log(MatchModel.currentMatch.Id);
                        }
                    }
                    GotMatch = true;
                    break;

                case Tags.Tag.SERVER_CONFIRM_SLATE_TAKEN:
                    using (Message msg = e.GetMessage())
                    {
                        using (DarkRiftReader reader = msg.GetReader())
                        {
                            ushort slateIndex = reader.ReadUInt16();
                            ushort clientID = reader.ReadUInt16();
                            MatchModel.currentMatch.ServerReportSlateTaken(slateIndex, clientID == client.ID);
                        }
                    }
                    break;
            }
        }
    }
}
