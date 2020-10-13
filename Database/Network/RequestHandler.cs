﻿using Common;
using Common.Formatting;
using Common.Network;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Database.Network
{
    class RequestHandler
    {
        private readonly Database database;
        private readonly TCPServer server;
        private Thread handler;

        public RequestHandler(Database database, TCPServer server)
        {
            this.database = database;
            this.server = server;
        }

        public void Start()
        {
            handler = new Thread(new ThreadStart(HandlerThread));
            handler.Start();
        }

        public void Stop()
        {
            handler.Abort();
        }

        private void HandlerThread()
        {
            if (!server.Start())
            {
                Console.WriteLine("Server start failed");
                return;
            }

            while (true)
            {
                while (server.PacketCount() > 0)
                {
                    Packet packet = server.GetPacket();

                    if (packet.PacketId == (uint)PacketType.EXECUTE_COMMAND)
                    {
                        ExecuteCommand(packet);
                    }
                    else if (packet.PacketId == (uint)PacketType.ADD_ENTRY_TABLENAME)
                    {
                        Packet[] packets = new Packet[2];
                        packets[0] = packet;
                        packets[1] = server.GetPacket();
                        AddEntry(packets);
                    }
                    else if (packet.PacketId == (uint)PacketType.EDIT_ENTRY_TABLENAME)
                    {
                        Packet[] packets = new Packet[3];
                        packets[0] = packet;
                        packets[1] = server.GetPacket();
                        packets[2] = server.GetPacket();
                        EditEntry(packets);
                    }


                    
                }
                Thread.Sleep(10);
            }
        }

        private void ExecuteCommand(Packet packet)
        {
            string command = Encoding.ASCII.GetString(packet.Data);
            DataList result = database.Execute(command);
            if (result != null)
            {
                string jsonStr = Json.FromList(DataList.ToList(result));
                server.Send(new Packet
                {
                    Data = Encoding.ASCII.GetBytes(jsonStr),
                    PacketId = (uint)PacketType.DATA,
                    SenderId = packet.SenderId
                });
            }
            else
            {
                server.Send(new Packet
                {
                    Data = Encoding.ASCII.GetBytes("null"),
                    PacketId = (uint)PacketType.TEXT,
                    SenderId = packet.SenderId
                });
            }
        }

        private void AddEntry(Packet[] data)
        {
            string tableName = Encoding.ASCII.GetString(data[0].Data);
            string jsonStr = Encoding.ASCII.GetString(data[1].Data);
            DataList entry = DataList.FromList(Json.ToList(jsonStr));
            database.AddEntry(entry, tableName);
        }

        private void EditEntry(Packet[] data)
        {
            string tableName = Encoding.ASCII.GetString(data[0].Data);
            string rowName = Encoding.ASCII.GetString(data[1].Data);
            string jsonStr = Encoding.ASCII.GetString(data[2].Data);
            DataList entry = DataList.FromList(Json.ToList(jsonStr));
            database.EditEntry(entry, tableName, rowName);
        }
    }
}