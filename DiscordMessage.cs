using CorpseLib.DataNotation;
using DiscordCorpse.MessagePart;
using System.Collections;
using System.Text;

namespace DiscordCorpse
{
    public class DiscordMessage : IEnumerable<IDiscordMessagePart>
    {
        private readonly List<IDiscordMessagePart> m_Parts = [];
        private readonly List<DiscordEmbed> m_Embeds = [];
        private string m_Nonce = string.Empty;

        internal void SetNonce(string nonce) => m_Nonce = nonce;

        public void Add(IDiscordMessagePart messagePart) => m_Parts.Add(messagePart);
        public void Add(DiscordEmbed embed) => m_Embeds.Add(embed);

        public IEnumerator<IDiscordMessagePart> GetEnumerator() => ((IEnumerable<IDiscordMessagePart>)m_Parts).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)m_Parts).GetEnumerator();

        public DataObject Serialize()
        {
            DataObject serialized = [];
            if (m_Parts.Count > 0)
            {
                StringBuilder contentBuilder = new();
                foreach (IDiscordMessagePart part in m_Parts)
                    contentBuilder.Append(part.GetPart());
                serialized["content"] = contentBuilder.ToString();
            }

            if (m_Embeds.Count > 0)
                serialized["embeds"] = m_Embeds;

            if (!string.IsNullOrEmpty(m_Nonce))
                serialized["nonce"] = m_Nonce;
            return serialized;
        }

        public override string ToString()
        {
            StringBuilder builder = new();
            foreach (IDiscordMessagePart part in m_Parts)
                builder.Append(part.GetPart());
            return builder.ToString();
        }
    }
}
