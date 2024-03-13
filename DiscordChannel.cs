namespace DiscordCorpse
{
    public class DiscordChannel
    {
        private readonly DiscordClient m_Client;
        private readonly string m_ID;

        public string ID => m_ID;

        internal DiscordChannel(DiscordClient client, string id)
        {
            m_Client = client;
            m_ID = id;
        }

        public void SendMessage(string message) => m_Client.SendMessage(m_ID, message);
        public void SendMessage(DiscordMessage message) => m_Client.SendMessage(m_ID, message);
    }
}
