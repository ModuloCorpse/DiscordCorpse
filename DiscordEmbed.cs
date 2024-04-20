using CorpseLib;
using CorpseLib.DataNotation;
using CorpseLib.Network;
using DiscordCorpse.Embed;

namespace DiscordCorpse
{
    public class DiscordEmbed
    {
        public class DataSerializer : ADataSerializer<DiscordEmbed>
        {
            protected override OperationResult<DiscordEmbed> Deserialize(DataObject reader)
            {
                DiscordEmbed embed = new();
                DiscordEmbedFooter? footer = reader.GetOrDefault<DiscordEmbedFooter?>("footer", null);
                if (footer != null)
                    embed.SetFooter(footer);

                DiscordEmbedImage? image = reader.GetOrDefault<DiscordEmbedImage?>("image", null);
                if (image != null)
                    embed.SetImage(image);

                DiscordEmbedThumbnail? thumbnail = reader.GetOrDefault<DiscordEmbedThumbnail?>("thumbnail", null);
                if (thumbnail != null)
                    embed.SetThumbnail(thumbnail);

                DiscordEmbedVideo? video = reader.GetOrDefault<DiscordEmbedVideo?>("video", null);
                if (video != null)
                    embed.SetVideo(video);

                DiscordEmbedProvider? provider = reader.GetOrDefault<DiscordEmbedProvider?>("provider", null);
                if (provider != null)
                    embed.SetProvider(provider);

                DiscordEmbedAuthor? author = reader.GetOrDefault<DiscordEmbedAuthor?>("author", null);
                if (author != null)
                    embed.SetAuthor(author);

                List<DiscordEmbedField> fields = reader.GetList<DiscordEmbedField>("fields");
                foreach (DiscordEmbedField field in fields)
                    embed.AddField(field);

                URI? url = URI.NullParse(reader.GetOrDefault<string?>("url", null));
                if (url != null)
                    embed.SetURL(url);

                string? timestampStr = reader.GetOrDefault<string?>("timestamp", null);
                DateTime? timestamp = (timestampStr != null) ? DateTime.Parse(timestampStr) : null;
                if (timestamp != null)
                    embed.SetTimestamp((DateTime)timestamp);

                string? title = reader.GetOrDefault<string?>("title", null);
                if (title != null)
                    embed.SetTitle(title);

                string? type = reader.GetOrDefault<string?>("type", null);
                if (type != null)
                    embed.SetType(type);

                string? description = reader.GetOrDefault<string?>("description", null);
                if (description != null)
                    embed.SetDescription(description);

                int? color = reader.GetOrDefault<int?>("color", null);
                if (color != null)
                    embed.SetColor((int)color);

                return new(embed);
            }

            protected override void Serialize(DiscordEmbed obj, DataObject writer)
            {
                if (obj.m_Title != null) writer["title"] = obj.m_Title;
                if (obj.m_Type != null) writer["type"] = obj.m_Type;
                if (obj.m_Description != null) writer["description"] = obj.m_Description;
                if (obj.m_URL != null) writer["url"] = obj.m_URL.ToString();
                if (obj.m_Timestamp != null) writer["timestamp"] = obj.m_Timestamp.ToString();
                if (obj.m_Color != null) writer["color"] = obj.m_Color;
                if (obj.m_Footer != null) writer["footer"] = obj.m_Footer;
                if (obj.m_Image != null) writer["image"] = obj.m_Image;
                if (obj.m_Thumbnail != null) writer["thumbnail"] = obj.m_Thumbnail;
                if (obj.m_Video != null) writer["video"] = obj.m_Video;
                if (obj.m_Provider != null) writer["provider"] = obj.m_Provider;
                if (obj.m_Author != null) writer["author"] = obj.m_Author;
                if (obj.m_Fields.Count > 0) writer["fields"] = obj.m_Fields;
            }
        }

        private DiscordEmbedFooter? m_Footer = null; //footer information
        private DiscordEmbedImage? m_Image = null; //image information
        private DiscordEmbedThumbnail? m_Thumbnail = null; //thumbnail information
        private DiscordEmbedVideo? m_Video = null; //video information
        private DiscordEmbedProvider? m_Provider = null; //provider information
        private DiscordEmbedAuthor? m_Author = null; //author information
        private List<DiscordEmbedField> m_Fields = []; //fields information, max of 25
        private URI? m_URL = null; //url of embed
        private DateTime? m_Timestamp = null; //timestamp of embed content
        private string? m_Title = null; //title of embed
        private string? m_Type = null; //type of embed(always "rich" for webhook embeds)
        private string? m_Description = null; //description of embed
        private int? m_Color = null; //color code of the embed

        public void SetFooter(DiscordEmbedFooter footer) => m_Footer = footer;
        public void SetImage(DiscordEmbedImage image) => m_Image = image;
        public void SetThumbnail(DiscordEmbedThumbnail thumbnail) => m_Thumbnail = thumbnail;
        public void SetVideo(DiscordEmbedVideo video) => m_Video = video;
        public void SetProvider(DiscordEmbedProvider provider) => m_Provider = provider;
        public void SetAuthor(DiscordEmbedAuthor author) => m_Author = author;
        public void SetURL(URI url) => m_URL = url;
        public void SetTimestamp(DateTime timestamp) => m_Timestamp = timestamp;
        public void SetTitle(string title) => m_Title = title;
        public void SetType(string type) => m_Type = type;
        public void SetDescription(string description) => m_Description = description;
        public void SetColor(int color) => m_Color = color;

        public bool AddField(DiscordEmbedField field)
        {
            if (m_Fields.Count == 25)
                return false;
            m_Fields.Add(field);
            return true;
        }
    }
}
