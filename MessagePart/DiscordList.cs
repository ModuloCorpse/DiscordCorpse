using System.Text;

namespace DiscordCorpse.MessagePart
{
    public class DiscordList(bool isOrdered) : IDiscordMessagePart, DiscordList.IListElement
    {
        private interface IListElement
        {
            public void Add(StringBuilder builder, int pos, int indent, bool isOrdered);
        }

        private class DiscordListElement(DiscordText text) : IListElement
        {
            private readonly DiscordText m_Text = text;

            public void Add(StringBuilder builder, int pos, int indent, bool isOrdered)
            {
                for (int i = 0; i != indent; i++)
                    builder.Append(' ');
                if (isOrdered)
                    builder.Append(string.Format("{0}. ", pos));
                else
                    builder.Append("- ");
                builder.Append(m_Text.GetPart());
            }
        }

        private readonly List<IListElement> m_Elements = [];
        private readonly bool m_IsOrdered = isOrdered;

        public void Add(StringBuilder builder, int pos, int indent, bool isOrdered)
        {
            int idx = 1;
            foreach (var element in m_Elements)
                element.Add(builder, idx, indent + 1, isOrdered);
        }

        public string GetPart()
        {
            StringBuilder builder = new();
            Add(builder, 0, 0, m_IsOrdered);
            return builder.ToString();
        }
    }
}
