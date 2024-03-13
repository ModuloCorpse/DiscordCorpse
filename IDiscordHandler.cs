namespace DiscordCorpse
{
    public interface IDiscordHandler
    {
        public void OnReady();
        public void OnMessageCreate(DiscordReceivedMessage message);
    }
}
