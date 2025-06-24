namespace DiscordCorpse.MessagePart.Text
{
    public class DiscordMention(string id) : ADiscordTextPart
    {
        private readonly string m_ID = id;

        public DiscordMention(DiscordUser user) : this(user.ID) { }

        internal override string GetTextPart() => $"<@{m_ID}>";
    }
}
