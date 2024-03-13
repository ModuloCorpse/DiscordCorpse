using CorpseLib;
using CorpseLib.Json;
using CorpseLib.Network;
using DiscordCorpse.Embed;
using DiscordCorpse.MessagePart.Text;
using DiscordCorpse.MessagePart;

namespace DiscordCorpse
{
    public class DiscordClient
    {
        static DiscordClient()
        {
            JsonHelper.RegisterSerializer(new DiscordAuthor.JsonSerializer());
            JsonHelper.RegisterSerializer(new DiscordUser.JsonSerializer());
            //Embed
            JsonHelper.RegisterSerializer(new DiscordEmbedAuthor.JsonSerializer());
            JsonHelper.RegisterSerializer(new DiscordEmbedField.JsonSerializer());
            JsonHelper.RegisterSerializer(new DiscordEmbedFooter.JsonSerializer());
            JsonHelper.RegisterSerializer(new DiscordEmbedImage.JsonSerializer());
            JsonHelper.RegisterSerializer(new DiscordEmbedProvider.JsonSerializer());
            JsonHelper.RegisterSerializer(new DiscordEmbedThumbnail.JsonSerializer());
            JsonHelper.RegisterSerializer(new DiscordEmbedVideo.JsonSerializer());
            JsonHelper.RegisterSerializer(new DiscordEmbed.JsonSerializer());
        }

        public static DiscordClient NewConnection(string clientSecret) => new(clientSecret, null);
        public static DiscordClient NewConnection(string clientSecret, IDiscordHandler discordHandler) => new(clientSecret, discordHandler);

        private readonly DiscordAPI m_API;
        private readonly IDiscordHandler? m_DiscordHandler;
        private readonly Dictionary<string, Operation<DiscordReceivedMessage>> m_AwaitingMessage = [];
        private DiscordClientProtocol m_Protocol;
        private IMonitor? m_Monitor;

        private DiscordClient(string clientSecret, IDiscordHandler? handler)
        {
            m_DiscordHandler = handler;
            m_API = new(clientSecret);
            m_Protocol = new(m_API, this);
            if (m_Monitor != null)
                m_Protocol.SetMonitor(m_Monitor);
            TCPAsyncClient discordGatewayClient = new(m_Protocol, URI.Parse(m_API.GetGatewayURL()));
            discordGatewayClient.SetSelfReconnectionDelayAndMaxNbTry(TimeSpan.FromMilliseconds(200), 5);
            discordGatewayClient.EnableSelfReconnection();
            discordGatewayClient.Start();
            m_Protocol.OnReconnectionRequested += OnReconnectionRequested;
        }

        private void OnReconnectionRequested()
        {
            DiscordClientProtocol protocol = new(m_API, this);
            protocol.SetSessionID(m_Protocol.SessionID);
            URI uri = m_Protocol.URI;
            m_Protocol.Disconnect();
            m_Protocol = protocol;
            TCPAsyncClient discordGatewayClient = new(m_Protocol, uri);
            if (m_Monitor != null)
                m_Protocol.SetMonitor(m_Monitor);
            discordGatewayClient.SetSelfReconnectionDelayAndMaxNbTry(TimeSpan.FromMilliseconds(200), 5);
            discordGatewayClient.EnableSelfReconnection();
            discordGatewayClient.Start();
            m_Protocol.OnReconnectionRequested += OnReconnectionRequested;
        }

        internal void OnReady() => m_DiscordHandler?.OnReady();

        internal void OnMessageCreate(DiscordReceivedMessage message) => m_DiscordHandler?.OnMessageCreate(message);

        internal void OnBotMessageCreate(DiscordReceivedMessage message)
        {
            if (m_AwaitingMessage.TryGetValue(message.Nonce, out Operation<DiscordReceivedMessage>? operation))
                operation.SetResult(message);
        }

        public void Disconnect() => m_Protocol.Disconnect();
        
        public void SetMonitor(IMonitor monitor)
        {
            m_Monitor = monitor;
            m_Protocol?.SetMonitor(monitor);
        }

        public DiscordChannel GetChannel(string channelID) => m_Protocol.GetChannel(channelID);
        public void SendMessage(string channelID, DiscordMessage message) => m_Protocol.SendMessage(channelID, message);
        public void SendMessage(string channelID, string message) => SendMessage(channelID, new DiscordMessage() { new DiscordText() { new DiscordFormatedText(message) } });

        public string PostMessage(string channelID, string message) => PostMessage(channelID, new DiscordMessage() { new DiscordText() { new DiscordFormatedText(message) } });
        public string PostMessage(string channelID, DiscordMessage message)
        {
            string nonce = Guid.NewGuid().ToString().Replace("-", "")[..25];
            message.SetNonce(nonce);
            Operation<DiscordReceivedMessage> awaitOperation = new();
            m_AwaitingMessage[nonce] = awaitOperation;
            SendMessage(channelID, message);
            if (awaitOperation.Wait())
            {
                OperationResult<DiscordReceivedMessage> receivedMessage = awaitOperation.Result;
                m_AwaitingMessage.Remove(nonce);
                if (receivedMessage)
                    return receivedMessage.Result!.ID;
            }
            m_AwaitingMessage.Remove(nonce);
            return string.Empty;
        }

        public bool CrossPostMessage(string channelID, string message) => CrossPostMessage(channelID, new DiscordMessage() { new DiscordText() { new DiscordFormatedText(message) } });
        public bool CrossPostMessage(string channelID, DiscordMessage message)
        {
            string messageID = PostMessage(channelID, message);
            if (string.IsNullOrEmpty(messageID))
                return false;
            return m_API.CrossPostMessage(channelID, messageID);
        }
    }
}
