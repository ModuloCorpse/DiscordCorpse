﻿using CorpseLib;
using CorpseLib.DataNotation;
using CorpseLib.Json;
using CorpseLib.Logging;
using CorpseLib.Network;
using CorpseLib.Web;
using CorpseLib.Web.Http;
using System.Text;

namespace DiscordCorpse
{
    public class DiscordClientProtocol(DiscordAPI api, DiscordClient client) : WebSocketProtocol
    {
        private class GatewayEvent
        {
            private readonly DataNode m_Data;
            private readonly string? m_Name = null;
            private readonly int? m_SequenceNumber = null;
            private readonly int m_OpCode;

            public int OpCode => m_OpCode;
            public DataNode Data => m_Data;
            public int SequenceNumber => (int)m_SequenceNumber!;
            public string EventName => m_Name!;

            public GatewayEvent(int opCode)
            {
                m_OpCode = opCode;
                m_Data = new DataObject();
            }

            public GatewayEvent(int opCode, object? data)
            {
                m_OpCode = opCode;
                m_Data = DataHelper.Cast(data);
            }

            public GatewayEvent(int opCode, DataNode data)
            {
                m_OpCode = opCode;
                m_Data = data;
            }

            public GatewayEvent(int opCode, object? data, int sequenceNumber, string name)
            {
                m_OpCode = opCode;
                m_Data = DataHelper.Cast(data);
                m_SequenceNumber = sequenceNumber;
                m_Name = name;
            }

            public GatewayEvent(int opCode, DataNode data, int sequenceNumber, string name)
            {
                m_OpCode = opCode;
                m_Data = data;
                m_SequenceNumber = sequenceNumber;
                m_Name = name;
            }

            public GatewayEvent(DataObject json)
            {
                m_OpCode = json.Get<int>("op")!;
                m_Data = json.Get<DataNode>("d")!;
                if (m_OpCode == 0)
                {
                    m_SequenceNumber = json.Get<int>("s");
                    m_Name = json.Get<string>("t");
                }
            }

            public override string ToString() => JsonParser.NetStr(new DataObject()
            {
                { "op", m_OpCode },
                { "d", m_Data },
                { "s", m_SequenceNumber },
                { "t", m_Name }
            });
        }

        public static readonly Logger DISCORD_GATEWAY = new("[${d}-${M}-${y} ${h}:${m}:${s}.${ms}] ${log}") { new LogInFile("./log/${y}${M}${d}${h}-Discord.log") };
        public static void StartLogging() => DISCORD_GATEWAY.Start();
        public static void StopLogging() => DISCORD_GATEWAY.Stop();

        private enum GatewayIntents : uint
        {
            GUILDS = 1 << 0,
            GUILD_MEMBERS = 1 << 1,
            GUILD_MODERATION = 1 << 2,
            GUILD_EMOJIS_AND_STICKERS = 1 << 3,
            GUILD_INTEGRATIONS = 1 << 4,
            GUILD_WEBHOOKS = 1 << 5,
            GUILD_INVITES = 1 << 6,
            GUILD_VOICE_STATES = 1 << 7,
            GUILD_PRESENCES = 1 << 8,
            GUILD_MESSAGES = 1 << 9,
            GUILD_MESSAGE_REACTIONS = 1 << 10,
            GUILD_MESSAGE_TYPING = 1 << 11,
            DIRECT_MESSAGES = 1 << 12,
            DIRECT_MESSAGE_REACTIONS = 1 << 13,
            DIRECT_MESSAGE_TYPING = 1 << 14,
            MESSAGE_CONTENT = 1 << 15,
            GUILD_SCHEDULED_EVENTS = 1 << 16,
            AUTO_MODERATION_CONFIGURATION = 1 << 20,
            AUTO_MODERATION_EXECUTION = 1 << 21
        }

        internal delegate void ReconnectionDelegate();

        private readonly DiscordClient m_Client = client;
        private RecurringAction? m_HeartbeatAction;
        private readonly DiscordAPI m_API = api;
        private readonly Dictionary<string, DiscordChannel> m_Channels = [];
        private DiscordUser? m_BotUser = null;
        private URI m_URI = URI.Parse(api.GetGatewayURL());
        private string m_SessionID = string.Empty;
        private string m_LastMessageFragment = string.Empty;
        private int? m_LastSequenceNumber = null;

        internal event ReconnectionDelegate? OnReconnectionRequested;

        internal URI URI => m_URI;
        internal string SessionID => m_SessionID;
        internal int? LastSequenceNumber => m_LastSequenceNumber;

        internal void SetReconnectionInfo(string sessionID, int? lastSequenceNumber)
        {
            m_SessionID = sessionID;
            m_LastSequenceNumber = lastSequenceNumber;
        }

        public DiscordChannel GetChannel(string channelID)
        {
            if (m_Channels.TryGetValue(channelID, out var channel))
                return channel;
            DiscordChannel discordChannel = new(m_Client, channelID);
            m_Channels.Add(channelID, discordChannel);
            return discordChannel;
        }

        private void HandleReady(DataObject data)
        {
            DISCORD_GATEWAY.Log($"<=[READY] {JsonParser.NetStr(data)}");
            m_BotUser = data.Get<DiscordUser>("user");
            m_SessionID = data.Get<string>("session_id")!;
            m_URI = URI.Parse(data.Get<string>("resume_gateway_url")!);
            m_Client?.OnReady();
        }

        private void HandleMessageCreate(DataObject data)
        {
            DISCORD_GATEWAY.Log($"<=[MESSAGE] {JsonParser.NetStr(data)}");
            DiscordChannel channel = GetChannel(data.Get<string>("channel_id")!);
            DiscordReceivedMessage message = new(m_API, channel, data);
            if (m_BotUser?.ID != message.Author.ID)
                m_Client?.OnMessageCreate(message);
            else
                m_Client?.OnBotMessageCreate(message);
        }

        private void HandleDispatch(GatewayEvent receivedEvent)
        {
            m_LastSequenceNumber = receivedEvent.SequenceNumber;
            switch (receivedEvent.EventName)
            {
                case "READY": HandleReady((DataObject)receivedEvent.Data); break;
                case "MESSAGE_CREATE": HandleMessageCreate((DataObject)receivedEvent.Data); break;
                case "RESUMED": DISCORD_GATEWAY.Log("Resumed"); break;
            }
        }

        private void Identify()
        {
            Send(new GatewayEvent(2, new DataObject()
            {
                { "token", m_API.Token },
                { "properties", new DataObject()
                    {
                        { "os", Environment.OSVersion.VersionString },
                        { "browser", "DiscordCorpse" },
                        { "device", "DiscordCorpse" }
                    }
                },
                { "intents", GatewayIntents.GUILD_MESSAGES | GatewayIntents.DIRECT_MESSAGES }
            }).ToString());
        }

        private void HandleHello(GatewayEvent receivedEvent)
        {
            if (receivedEvent.Data is DataObject helloData)
            {
                m_HeartbeatAction?.Stop();
                m_HeartbeatAction = new RecurringAction(helloData.Get<int>("heartbeat_interval")!);
                m_HeartbeatAction.OnUpdate += Heartbeat;
                m_HeartbeatAction?.Start();

                if (string.IsNullOrEmpty(m_SessionID))
                {
                    Send(new GatewayEvent(1, m_LastSequenceNumber).ToString());
                    Identify();
                }
            }
        }

        private void Heartbeat(object? sender, EventArgs e)
        {
            DISCORD_GATEWAY.Log("=> HeartBeat");
            Send(new GatewayEvent(1, m_LastSequenceNumber).ToString());
        }

        protected override void OnWSMessage(string message)
        {
            string messageWithFragment = m_LastMessageFragment + message;
            DISCORD_GATEWAY.Log($"<= ${messageWithFragment}");
            try
            {
                DataObject receivedEventJson = JsonParser.Parse(messageWithFragment);
                m_LastMessageFragment = string.Empty;
                GatewayEvent receivedEvent = new(receivedEventJson);
                switch (receivedEvent.OpCode)
                {
                    case 0: HandleDispatch(receivedEvent); break;
                    case 7: OnReconnectionRequested?.Invoke(); break;
                    case 9: /* Handle Invalid Session */ break;
                    case 10: HandleHello(receivedEvent); break;
                    case 11: break; // Heartbeat ACK DISCARDED
                }
            }
            catch
            {
                m_LastMessageFragment = messageWithFragment;
            }
        }

        public void SendMessage(string channelID, DiscordMessage message) => m_API.SendMessageToChannel(channelID, message);

        protected override void OnDiscardException(Exception exception)
        {
            DISCORD_GATEWAY.Log(exception.ToString());
        }

        protected override void OnWSOpen(Response message)
        {
            DISCORD_GATEWAY.Log("Websocket open");
            base.OnWSOpen(message);
            if (!string.IsNullOrEmpty(m_SessionID))
            {
                Send(new GatewayEvent(6, new DataObject()
                {
                    { "token", m_API.Token },
                    { "session_id", m_SessionID },
                    { "seq", m_LastSequenceNumber }
                }).ToString());
                m_HeartbeatAction?.Start();
                DISCORD_GATEWAY.Log("Reconnected");
            }
        }

        protected override void OnClientDisconnected()
        {
            StringBuilder builder = new();
            foreach (string traceLine in Environment.StackTrace.Split('\n'))
            {
                string trace = traceLine.Trim();
                if (!string.IsNullOrEmpty(trace) && !trace.Contains("at System.Environment.get_StackTrace()"))
                    builder.AppendLine(trace);
            }
            DISCORD_GATEWAY.Log("Disconnection trace");
            DISCORD_GATEWAY.Log(builder.ToString());

            //TODO Check why action isn't stopped properly
            m_HeartbeatAction?.Stop();
            DISCORD_GATEWAY.Log("Disconnected");
        }

        protected override void OnWSClose(int status, string message)
        {
            //TODO Check why action isn't stopped properly
            m_HeartbeatAction?.Stop();
            DISCORD_GATEWAY.Log($"[${status}] ${message}");
            if (status == 1001)
                Reconnect();
        }
    }
}
