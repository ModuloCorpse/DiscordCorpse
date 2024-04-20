using CorpseLib;
using CorpseLib.Json;
using CorpseLib.Network;
using DiscordCorpse.Embed;
using DiscordCorpse.MessagePart.Text;
using DiscordCorpse.MessagePart;
using CorpseLib.DataNotation;

namespace DiscordCorpse
{
    public class DiscordClient
    {
        static DiscordClient()
        {
            DataHelper.RegisterSerializer(new DiscordAuthor.DataSerializer());
            DataHelper.RegisterSerializer(new DiscordUser.DataSerializer());
            //Embed
            DataHelper.RegisterSerializer(new DiscordEmbedAuthor.DataSerializer());
            DataHelper.RegisterSerializer(new DiscordEmbedField.DataSerializer());
            DataHelper.RegisterSerializer(new DiscordEmbedFooter.DataSerializer());
            DataHelper.RegisterSerializer(new DiscordEmbedImage.DataSerializer());
            DataHelper.RegisterSerializer(new DiscordEmbedProvider.DataSerializer());
            DataHelper.RegisterSerializer(new DiscordEmbedThumbnail.DataSerializer());
            DataHelper.RegisterSerializer(new DiscordEmbedVideo.DataSerializer());
            DataHelper.RegisterSerializer(new DiscordEmbed.DataSerializer());
        }

        public static DiscordClient NewConnection(string clientSecret) => new(clientSecret, null, null);
        public static DiscordClient NewConnection(string clientSecret, IDiscordHandler discordHandler) => new(clientSecret, discordHandler, null);
        public static DiscordClient NewConnection(string clientSecret, IMonitor monitor) => new(clientSecret, null, monitor);
        public static DiscordClient NewConnection(string clientSecret, IDiscordHandler discordHandler, IMonitor monitor) => new(clientSecret, discordHandler, monitor);

        private readonly DiscordAPI m_API;
        private readonly IDiscordHandler? m_DiscordHandler;
        private readonly Dictionary<string, Operation<DiscordReceivedMessage>> m_AwaitingMessage = [];
        private DiscordClientProtocol m_Protocol;
        private IMonitor? m_Monitor;

        private DiscordClient(string clientSecret, IDiscordHandler? handler, IMonitor? monitor)
        {
            m_DiscordHandler = handler;
            m_API = new(clientSecret);
            m_Protocol = new(m_API, this);
            m_Monitor = monitor;
            TCPAsyncClient discordGatewayClient = new(m_Protocol, URI.Parse(m_API.GetGatewayURL()));
            if (m_Monitor != null)
                m_Protocol.SetMonitor(m_Monitor);
            discordGatewayClient.Start();
            m_Protocol.OnReconnectionRequested += OnReconnectionRequested;
        }

        private void OnReconnectionRequested()
        {
            m_Protocol.Disconnect();
            DiscordClientProtocol protocol = new(m_API, this);
            protocol.SetReconnectionInfo(m_Protocol.SessionID, m_Protocol.LastSequenceNumber);
            URI uri = m_Protocol.URI;
            m_Protocol = protocol;
            TCPAsyncClient discordGatewayClient = new(m_Protocol, uri);
            if (m_Monitor != null)
                m_Protocol.SetMonitor(m_Monitor);
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
            m_API.CrossPostMessage(channelID, messageID);
            return true;
        }

        public bool IsConnected() => m_Protocol.IsConnected();
    }
}
