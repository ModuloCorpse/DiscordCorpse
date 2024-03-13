using CorpseLib.Network;
using System.Text;

namespace DiscordCorpse.MessagePart.Text
{
    public class DiscordLink(URI uri) : ADiscordTextPart
    {
        private readonly URI m_URI = uri;
        private string m_Text = string.Empty;
        private bool m_Embed = true;

        public void Hide(string text) => m_Text = text;
        public void NoEmbed() => m_Embed = false;

        internal override string GetTextPart()
        {
            StringBuilder builder = new();
            if (!string.IsNullOrEmpty(m_Text))
            {
                builder.Append('[');
                builder.Append(m_Text);
                builder.Append("](");
            }
            if (!m_Embed)
                builder.Append('<');
            builder.Append(m_URI.ToString());
            if (!m_Embed)
                builder.Append('>');
            if (!string.IsNullOrEmpty(m_Text))
                builder.Append(')');
            return builder.ToString();
        }
    }
}
