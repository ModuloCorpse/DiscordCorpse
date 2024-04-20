using CorpseLib;
using CorpseLib.DataNotation;

namespace DiscordCorpse
{
    public class DiscordAuthor
    {
        public class DataSerializer : ADataSerializer<DiscordAuthor>
        {
            protected override OperationResult<DiscordAuthor> Deserialize(DataObject reader)
            {
                string username = reader.Get<string>("username")!;
                int publicFlags = reader.GetOrDefault("public_flags", -1);
                string id = reader.Get<string>("id")!;
                string globalName = reader.Get<string>("global_name") ?? string.Empty;
                string discriminator = reader.Get<string>("discriminator")!;
                //bool isBot = reader.Get<bool>("avatar_decoration_data")!;
                string avatar = reader.Get<string>("avatar") ?? string.Empty;
                return new(new(username, id, globalName, discriminator, avatar, publicFlags));
            }

            protected override void Serialize(DiscordAuthor obj, DataObject writer)
            {
                writer["username"] = obj.UserName;
                writer["public_flags"] = obj.PublicFlags;
                writer["id"] = obj.ID;
                writer["global_name"] = obj.GlobalName;
                writer["discriminator"] = obj.Discriminator;
                writer["avatar_decoration_data"] = obj.AvatarDecorationData;
                writer["avatar"] = obj.Avatar;
            }
        }

        private readonly string m_Username;
        private readonly string m_ID;
        private readonly string m_GlobalName;
        private readonly string m_Discriminator;
        private readonly string m_AvatarDecorationData;
        private readonly string m_Avatar;
        private readonly int m_PublicFlags;

        public string UserName => m_Username;
        public string ID => m_ID;
        public string GlobalName => m_GlobalName;
        public string Discriminator => m_Discriminator;
        public string AvatarDecorationData => m_AvatarDecorationData;
        public string Avatar => m_Avatar;
        public int PublicFlags => m_PublicFlags;

        /*public DiscordAuthor(string username, string id, string globalName, string discriminator, string avatarDecorationData, string avatar, int publicFlags)
        {
            m_Username = username;
            m_ID = id;
            m_GlobalName = globalName;
            m_Discriminator = discriminator;
            m_AvatarDecorationData = avatarDecorationData;
            m_Avatar = avatar;
            m_PublicFlags = publicFlags;
        }*/

        public DiscordAuthor(string username, string id, string globalName, string discriminator, string avatar, int publicFlags)
        {
            m_Username = username;
            m_ID = id;
            m_GlobalName = globalName;
            m_Discriminator = discriminator;
            m_Avatar = avatar;
            m_PublicFlags = publicFlags;
        }
    }
}
