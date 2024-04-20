using CorpseLib.DataNotation;

namespace DiscordCorpse
{
    public class DiscordReceivedMessage
    {
        private readonly DiscordAPI m_API;
        private readonly DiscordChannel m_Channel;
        private readonly DiscordAuthor m_Author;
        private readonly string m_ID;
        private readonly string m_Content;
        private readonly string m_Nonce;

        public DiscordChannel Channel => m_Channel;
        public DiscordAuthor Author => m_Author;
        public string ID => m_ID;
        public string Content => m_Content;
        public string Nonce => m_Nonce;

        internal DiscordReceivedMessage(DiscordAPI api, DiscordChannel channel, DataObject data)
        {
            m_API = api;
            m_Channel = channel;
            m_ID = data.Get<string>("id")!;
            m_Content = data.Get<string>("content")!;
            m_Author = data.Get<DiscordAuthor>("author")!;
            if (data.TryGet("nonce", out string? nonce) && nonce != null)
                m_Nonce = nonce;
            else
                m_Nonce = string.Empty;
        }

        public void Delete() => m_API.DeleteMessage(m_Channel.ID, m_ID);
    }
}
