using System.Text;

namespace DiscordCorpse.MessagePart
{
    public class DiscordQuote : IDiscordMessagePart
    {
        private readonly List<string> m_Lines = [];

        public DiscordQuote(string quote) => m_Lines.AddRange(quote.Replace("\r\n", "\n").Split('\n'));

        public void AddLine(string line) => m_Lines.Add(line);

        public string GetPart()
        {
            StringBuilder builder = new();
            for (int i = 0; i < m_Lines.Count; i++)
            {
                builder.Append('>');
                builder.Append(m_Lines[i]);
                if (i < m_Lines.Count - 1)
                    builder.AppendLine();
            }
            return builder.ToString();
        }
    }
}
