using CorpseLib.Network;
using CorpseLib;
using CorpseLib.DataNotation;

namespace DiscordCorpse.Embed
{
    public class DiscordEmbedFooter(string text, URI? iconURL, URI? proxyIconURL)
    {
        public class DataSerializer : ADataSerializer<DiscordEmbedFooter>
        {
            protected override OperationResult<DiscordEmbedFooter> Deserialize(DataObject reader)
            {
                if (reader.TryGet("text", out string? text) && text != null)
                    return new(new(text, URI.NullParse(reader.GetOrDefault<string?>("icon_url", null)), URI.NullParse(reader.GetOrDefault<string?>("proxy_icon_url", null))));
                return new("Deserialization error", "No text");
            }

            protected override void Serialize(DiscordEmbedFooter obj, DataObject writer)
            {
                writer["text"] = obj.m_Text;
                if (obj.m_IconURL != null) writer["icon_url"] = obj.m_IconURL;
                if (obj.m_ProxyIconURL != null) writer["proxy_icon_url"] = obj.m_ProxyIconURL;
            }
        }

        private readonly URI? m_IconURL = iconURL; //url of author icon (only supports http(s) and attachments)
        private readonly URI? m_ProxyIconURL = proxyIconURL; //a proxied url of author icon
        private readonly string m_Text = text; //name of author

        public URI? IconURL => m_IconURL;
        public URI? ProxyIconURL => m_ProxyIconURL;
        public string Text => m_Text;
    }
}
