namespace DiscordCorpse.MessagePart.Text
{
    public class DiscordFormatedText(string text) : ADiscordTextPart
    {
        private readonly string m_Text = text;
        internal override string GetTextPart() => m_Text.Replace("\\", "\\\\").Replace("_", "\\_").Replace("*", "\\*").Replace("~", "\\~");
    }
}
