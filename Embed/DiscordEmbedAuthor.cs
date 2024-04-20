using CorpseLib;
using CorpseLib.DataNotation;
using CorpseLib.Network;

namespace DiscordCorpse.Embed
{
    public class DiscordEmbedAuthor(string name, URI? url, URI? iconURL, URI? proxyIconURL)
    {
        public class DataSerializer : ADataSerializer<DiscordEmbedAuthor>
        {
            protected override OperationResult<DiscordEmbedAuthor> Deserialize(DataObject reader)
            {
                if (reader.TryGet("name", out string? name) && name != null)
                    return new(new(name, URI.NullParse(reader.GetOrDefault<string?>("url", null)), URI.NullParse(reader.GetOrDefault<string?>("icon_url", null)), URI.NullParse(reader.GetOrDefault<string?>("proxy_icon_url", null))));
                return new("Deserialization error", "No name");
            }

            protected override void Serialize(DiscordEmbedAuthor obj, DataObject writer)
            {
                writer["name"] = obj.m_Name;
                if (obj.m_URL != null) writer["url"] = obj.m_URL;
                if (obj.m_IconURL != null) writer["icon_url"] = obj.m_IconURL;
                if (obj.m_ProxyIconURL != null) writer["proxy_icon_url"] = obj.m_ProxyIconURL;
            }
        }

        private readonly URI? m_URL = url; //url of author (only supports http(s))
        private readonly URI? m_IconURL = iconURL; //url of author icon (only supports http(s) and attachments)
        private readonly URI? m_ProxyIconURL = proxyIconURL; //a proxied url of author icon
        private readonly string m_Name = name; //name of author

        public URI? URL => m_URL;
        public URI? IconURL => m_IconURL;
        public URI? ProxyIconURL => m_ProxyIconURL;
        public string Name => m_Name;
    }
}
