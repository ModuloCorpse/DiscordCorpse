using CorpseLib.Network;
using CorpseLib;
using CorpseLib.DataNotation;

namespace DiscordCorpse.Embed
{
    public class DiscordEmbedThumbnail(URI url, URI? proxyURL, int? width, int? height)
    {
        public class DataSerializer : ADataSerializer<DiscordEmbedThumbnail>
        {
            protected override OperationResult<DiscordEmbedThumbnail> Deserialize(DataObject reader)
            {
                if (reader.TryGet("url", out string? url) && url != null)
                    return new(new(URI.Parse(url), URI.NullParse(reader.GetOrDefault<string?>("proxy_url", null)), reader.GetOrDefault<int?>("width", null), reader.GetOrDefault<int?>("height", null)));
                return new("Deserialization error", "No url");
            }

            protected override void Serialize(DiscordEmbedThumbnail obj, DataObject writer)
            {
                writer["url"] = obj.m_URL;
                if (obj.m_ProxyURL != null) writer["proxy_url"] = obj.m_ProxyURL;
                if (obj.m_Width != null) writer["width"] = obj.m_Width;
                if (obj.m_Height != null) writer["height"] = obj.m_Height;
            }
        }

        private readonly URI m_URL = url; //source url of thumbnail (only supports http(s) and attachments)
        private readonly URI? m_ProxyURL = proxyURL; //a proxied url of the thumbnail
        private readonly int? m_Width = width; //height of thumbnail
        private readonly int? m_Height = height; //width of thumbnail

        public URI URL => m_URL;
        public URI? ProxyURL => m_ProxyURL;
        public int? Width => m_Width;
        public int? Height => m_Height;
    }
}
