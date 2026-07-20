using CorpseLib;
using CorpseLib.DataNotation;
using CorpseLib.Network.WebSocket;
using DiscordCorpse.Embed;
using DiscordCorpse.MessagePart;
using DiscordCorpse.MessagePart.Text;

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

        public static async Task<DiscordClient?> NewConnection(string clientSecret, IDiscordHandler handler)
        {
            DiscordClient discordClient = new(clientSecret, handler);
            WebSocketClient? webSocket = await WebSocketClient.Connect(URI.Parse(discordClient.GatewayURL), discordClient.Protocol);
            if (webSocket == null)
                return null;
            return discordClient;
        }

        private readonly DiscordAPI m_API;
        private readonly IDiscordHandler m_DiscordHandler;
        private readonly Dictionary<string, Operation<DiscordReceivedMessage>> m_AwaitingMessage = [];
        private DiscordClientProtocol m_Protocol;

        internal string GatewayURL => m_API.GetGatewayURL();
        internal DiscordClientProtocol Protocol => m_Protocol;

        private DiscordClient(string clientSecret, IDiscordHandler handler)
        {
            m_DiscordHandler = handler;
            m_API = new(clientSecret);
            m_Protocol = new(m_API, this);
            m_Protocol.OnReconnectionRequested += OnReconnectionRequested;
        }

        private async Task OnReconnectionRequested()
        {
            await m_Protocol.Disconnect();
            DiscordClientProtocol protocol = new(m_API, this);
            protocol.SetReconnectionInfo(m_Protocol.SessionID, m_Protocol.LastSequenceNumber);
            URI uri = m_Protocol.URI;
            m_Protocol = protocol;
            WebSocketClient? webSocket = await WebSocketClient.Connect(uri, m_Protocol);
            if (webSocket == null)
                return;
            m_Protocol.OnReconnectionRequested += OnReconnectionRequested;
        }

        internal async Task OnReady() => await m_DiscordHandler.OnReady();

        internal async Task OnMessageCreate(DiscordReceivedMessage message) => await m_DiscordHandler.OnMessageCreate(message);

        internal void OnBotMessageCreate(DiscordReceivedMessage message)
        {
            if (m_AwaitingMessage.TryGetValue(message.Nonce, out Operation<DiscordReceivedMessage>? operation))
                operation.SetResult(message);
        }

        public async Task Disconnect() => await m_Protocol.Disconnect();

        public DiscordChannel GetChannel(string channelID) => m_Protocol.GetChannel(channelID);
        public void SendMessage(string channelID, DiscordMessage message) => m_Protocol.SendMessage(channelID, message);
        public void SendMessage(string channelID, string message) => SendMessage(channelID, [new DiscordText() { new DiscordFormatedText(message) }]);

        public string PostMessage(string channelID, string message) => PostMessage(channelID, [new DiscordText() { new DiscordFormatedText(message) }]);
        public string PostMessage(string channelID, DiscordMessage message)
        {
            string nonce = Guid.NewGuid().ToString().Replace("-", "")[..25];
            message.SetNonce(nonce);
            Operation<DiscordReceivedMessage> awaitOperation = new();
            m_AwaitingMessage[nonce] = awaitOperation;
            SendMessage(channelID, message);
            if (awaitOperation.Wait(TimeSpan.FromSeconds(5)))
            {
                OperationResult<DiscordReceivedMessage> receivedMessage = awaitOperation.Result;
                m_AwaitingMessage.Remove(nonce);
                if (receivedMessage)
                    return receivedMessage.Result!.ID;
            }
            m_AwaitingMessage.Remove(nonce);
            return string.Empty;
        }

        public bool CrossPostMessage(string channelID, string message) => CrossPostMessage(channelID, [new DiscordText() { new DiscordFormatedText(message) }]);
        public bool CrossPostMessage(string channelID, DiscordMessage message) => m_API.CrossPostMessage(channelID, message);

        public bool IsConnected() => m_Protocol.IsConnected();

        public async Task Reconnect() => await OnReconnectionRequested();
    }
}
