using CorpseLib;
using CorpseLib.DataNotation;
using CorpseLib.Network;

namespace DiscordCorpse.Embed
{
    public class DiscordEmbedProvider(string? name, URI? url)
    {
        public class DataSerializer : ADataSerializer<DiscordEmbedProvider>
        {
            protected override OperationResult<DiscordEmbedProvider> Deserialize(DataObject reader)
            {
                return new(new(reader.GetOrDefault<string?>("name", null),
                    URI.NullParse(reader.GetOrDefault<string?>("url", null))));
            }

            protected override void Serialize(DiscordEmbedProvider obj, DataObject writer)
            {
                if (obj.m_Name != null) writer["name"] = obj.m_Name;
                if (obj.m_URL != null) writer["url"] = obj.m_URL;
            }
        }

        private readonly URI? m_URL = url; //url of provider
        private readonly string? m_Name = name; //name of provider

        public URI? URL => m_URL;
        public string? Name => m_Name;
    }
}
