using System.Text;
using DiscordCorpse.MessagePart.Text;

namespace DiscordCorpse.MessagePart
{
    public class DiscordHeader(DiscordText header, DiscordHeader.Size size) : IDiscordMessagePart
    {
        public enum Size
        {
            BIG,
            MEDIUM,
            SMALL
        }

        private readonly DiscordText m_Header = header;
        private readonly Size m_Size = size;

        public DiscordHeader(string header, Size size) : this([new DiscordFormatedText(header)], size) { }

        public string GetPart()
        {
            StringBuilder builder = new();
            switch (m_Size)
            {
                case Size.BIG: builder.Append("# "); break;
                case Size.MEDIUM: builder.Append("## "); break;
                case Size.SMALL: builder.Append("### "); break;
            }
            builder.Append(m_Header.GetPart());
            return builder.ToString();
        }
    }
}
