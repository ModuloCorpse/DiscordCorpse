using DiscordCorpse.MessagePart.Text;
using System.Collections;
using System.Text;

namespace DiscordCorpse.MessagePart
{
    public class DiscordText : IEnumerable<ADiscordTextPart>, IDiscordMessagePart
    {
        private readonly List<ADiscordTextPart> m_FormattedText = [];

        public void Add(ADiscordTextPart textPart) => m_FormattedText.Add(textPart);
        public void Add(string textPart) => m_FormattedText.Add(new DiscordFormatedText(textPart));

        private static bool CheckFormat(bool newFormat, ref bool oldFormat, string formatStr, StringBuilder builder)
        {
            if (newFormat != oldFormat)
            {
                builder.Append(formatStr);
                oldFormat = newFormat;
                return true;
            }
            return false;
        }

        private static void CloseFormat(ref bool format, string formatStr, StringBuilder builder)
        {
            if (format)
            {
                builder.Append(formatStr);
                format = false;
            }
        }

        public string GetPart()
        {
            StringBuilder builder = new();
            bool italic = false;
            bool bold = false;
            bool strikethrough = false;
            bool underline = false;
            bool spoiler = false;
            bool codeBlock = false;
            foreach (ADiscordTextPart part in m_FormattedText)
            {
                if (!codeBlock)
                {
                    CheckFormat(part.IsSpoiler, ref spoiler, "||", builder);
                    CheckFormat(part.IsStrikethrough, ref strikethrough, "~~", builder);
                    CheckFormat(part.IsUnderline, ref underline, "__", builder);
                    CheckFormat(part.IsItalic, ref italic, "*", builder);
                    CheckFormat(part.IsBold, ref bold, "**", builder);
                }
                if (CheckFormat(part.IsCodeBlock, ref codeBlock, "`", builder) && !codeBlock)
                {
                    CheckFormat(part.IsSpoiler, ref spoiler, "||", builder);
                    CheckFormat(part.IsStrikethrough, ref strikethrough, "~~", builder);
                    CheckFormat(part.IsUnderline, ref underline, "__", builder);
                    CheckFormat(part.IsItalic, ref italic, "*", builder);
                    CheckFormat(part.IsBold, ref bold, "**", builder);
                }
                builder.Append(part.GetTextPart());
            }
            CloseFormat(ref codeBlock, "`", builder);
            CloseFormat(ref underline, "__", builder);
            CloseFormat(ref bold, "**", builder);
            CloseFormat(ref italic, "*", builder);
            CloseFormat(ref strikethrough, "~~", builder);
            CloseFormat(ref spoiler, "||", builder);
            return builder.ToString();
        }

        public IEnumerator<ADiscordTextPart> GetEnumerator() => ((IEnumerable<ADiscordTextPart>)m_FormattedText).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)m_FormattedText).GetEnumerator();
    }
}
