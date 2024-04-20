using CorpseLib;
using CorpseLib.DataNotation;

namespace DiscordCorpse
{
    public class DiscordUser(string username, string id, string discriminator, bool isVerified, bool isMFAEnabled, bool isBot)
    {
        public class DataSerializer : ADataSerializer<DiscordUser>
        {
            protected override OperationResult<DiscordUser> Deserialize(DataObject reader)
            {
                bool isVerified = reader.Get<bool>("verified")!;
                string username = reader.Get<string>("username")!;
                bool isMFAEnabled = reader.Get<bool>("mfa_enabled")!;
                string id = reader.Get<string>("id")!;
                //string globalName = reader.Get<string>("global_name")!;
                //int flags = reader.Get<int>("flags")!;
                //string email = reader.Get<string>("email")!;
                string discriminator = reader.Get<string>("discriminator")!;
                bool isBot = reader.Get<bool>("bot")!;
                //string avatar = reader.Get<string>("avatar")!;
                return new(new(username, id, discriminator, isVerified, isMFAEnabled, isBot));
            }

            protected override void Serialize(DiscordUser obj, DataObject writer)
            {
                writer["verified"] = obj.IsVerified;
                writer["username"] = obj.UserName;
                writer["mfa_enabled"] = obj.IsMFAEnabled;
                writer["id"] = obj.ID;
                //writer["global_name"] = obj.GlobalName;
                //writer["flags"] = obj.Flags;
                //writer["email"] = obj.Email;
                writer["discriminator"] = obj.Discriminator;
                writer["bot"] = obj.IsBot;
                //writer["avatar"] = obj.Avatar;
            }
        }

        private readonly string m_Username = username;
        private readonly string m_ID = id;
        //private readonly string m_GlobalName;
        //private readonly string m_Email;
        private readonly string m_Discriminator = discriminator;
        //private readonly string m_Avatar;
        //private readonly int m_Flags;
        private readonly bool m_IsVerified = isVerified;
        private readonly bool m_IsMFAEnabled = isMFAEnabled;
        private readonly bool m_IsBot = isBot;

        public string UserName => m_Username;
        public string ID => m_ID;
        //public string GlobalName => m_GlobalName;
        //public string Email => m_Email;
        public string Discriminator => m_Discriminator;
        //public string Avatar => m_Avatar;
        //public int Flags => m_Flags;
        public bool IsVerified => m_IsVerified;
        public bool IsMFAEnabled => m_IsMFAEnabled;
        public bool IsBot => m_IsBot;
    }
}
