using CorpseLib;
using CorpseLib.DataNotation;

namespace DiscordCorpse.Embed
{
    public class DiscordEmbedField(string name, string value, bool? inline = null)
    {
        public class DataSerializer : ADataSerializer<DiscordEmbedField>
        {
            protected override OperationResult<DiscordEmbedField> Deserialize(DataObject reader)
            {
                if (reader.TryGet("name", out string? name) && name != null &&
                    reader.TryGet("value", out string? value) && value != null)
                    return new(new(name, value, reader.GetOrDefault<bool?>("inline", null)));
                return new("Deserialization error", "No name or value");
            }

            protected override void Serialize(DiscordEmbedField obj, DataObject writer)
            {
                writer["name"] = obj.m_Name;
                writer["value"] = obj.m_Value;
                if (obj.m_Inline != null) writer["inline"] = obj.m_Inline;
            }
        }

        private readonly string m_Name = name; //name of the field
        private readonly string m_Value = value; //value of the field
        private readonly bool? m_Inline = inline; //whether or not this field should display inline

        public string Name => m_Name;
        public string Value => m_Value;
        public bool Inline => m_Inline == true;
    }
}
