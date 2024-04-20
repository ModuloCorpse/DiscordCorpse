using CorpseLib.Network;
using CorpseLib;
using CorpseLib.DataNotation;

namespace DiscordCorpse.Embed
{
    public class DiscordEmbedVideo(URI? url, URI? proxyURL, int? width, int? height)
    {
        public class DataSerializer : ADataSerializer<DiscordEmbedVideo>
        {
            protected override OperationResult<DiscordEmbedVideo> Deserialize(DataObject reader)
            {
                return new(new(URI.NullParse(reader.GetOrDefault<string?>("url", null)), URI.NullParse(reader.GetOrDefault<string?>("proxy_url", null)), reader.GetOrDefault<int?>("width", null), reader.GetOrDefault<int?>("height", null)));
            }

            protected override void Serialize(DiscordEmbedVideo obj, DataObject writer)
            {
                if (obj.m_URL != null) writer["url"] = obj.m_URL;
                if (obj.m_ProxyURL != null) writer["proxy_url"] = obj.m_ProxyURL;
                if (obj.m_Width != null) writer["width"] = obj.m_Width;
                if (obj.m_Height != null) writer["height"] = obj.m_Height;
            }
        }

        private readonly URI? m_URL = url; //source url of video
        private readonly URI? m_ProxyURL = proxyURL; //a proxied url of the video
        private readonly int? m_Width = width; //height of video
        private readonly int? m_Height = height; //width of video

        public URI? URL => m_URL;
        public URI? ProxyURL => m_ProxyURL;
        public int? Width => m_Width;
        public int? Height => m_Height;
    }
}
