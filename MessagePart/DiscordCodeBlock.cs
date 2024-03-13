using System.Text;

namespace DiscordCorpse.MessagePart
{
    public class DiscordCodeBlock(string language) : IDiscordMessagePart
    {
        private readonly string m_Language = language;
        private readonly List<string> m_Lines = [];

        public DiscordCodeBlock() : this(string.Empty) { }

        public void AddLine(string line) => m_Lines.Add(line);

        public string GetPart()
        {
            StringBuilder builder = new();
            builder.Append("```");
            builder.AppendLine(m_Language);
            foreach (string line in m_Lines)
                builder.AppendLine(line);
            builder.Append("```");
            return builder.ToString();
        }
    }
}
