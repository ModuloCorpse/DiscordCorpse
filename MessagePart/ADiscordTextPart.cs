using System.Text;

namespace DiscordCorpse.MessagePart
{
    public abstract class ADiscordTextPart
    {
        private bool m_IsItalic = false;
        private bool m_IsBold = false;
        private bool m_IsStrikethrough = false;
        private bool m_IsUnderline = false;
        private bool m_IsSpoiler = false;
        private bool m_IsCodeBlock = false;

        internal bool IsItalic => m_IsItalic;
        internal bool IsBold => m_IsBold;
        internal bool IsStrikethrough => m_IsStrikethrough;
        internal bool IsUnderline => m_IsUnderline;
        internal bool IsSpoiler => m_IsSpoiler;
        internal bool IsCodeBlock => m_IsCodeBlock;

        public void Italic() => m_IsItalic = true;
        public void Bold() => m_IsBold = true;
        public void Strikethrough() => m_IsStrikethrough = true;
        public void Underline() => m_IsUnderline = true;
        public void Spoiler() => m_IsSpoiler = true;
        public void CodeBlock() => m_IsCodeBlock = true;

        public string GetPart()
        {
            StringBuilder builder = new();
            if (m_IsSpoiler)
                builder.Append("|| ");
            if (!m_IsStrikethrough)
            {
                if (m_IsItalic) builder.Append('*');
                if (m_IsBold) builder.Append("**");
                if (m_IsUnderline) builder.Append("__");
            }
            else
                builder.Append("~~");
            builder.Append(GetTextPart());
            if (!m_IsStrikethrough)
            {
                if (m_IsUnderline) builder.Append("__");
                if (m_IsBold) builder.Append("**");
                if (m_IsItalic) builder.Append('*');
            }
            else
                builder.Append("~~");
            if (m_IsSpoiler)
                builder.Append(" ||");
            return builder.ToString();
        }

        internal abstract string GetTextPart();
    }
}
