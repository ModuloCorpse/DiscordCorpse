using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordCorpse.MessagePart.Text
{
    public class DiscordMention(string id) : ADiscordTextPart
    {
        private readonly string m_ID = id;

        public DiscordMention(DiscordUser user) : this(user.ID) { }

        internal override string GetTextPart() => string.Format("<@{0}>", m_ID);
    }
}
