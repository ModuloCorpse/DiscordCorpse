namespace DiscordCorpse
{
    public interface IDiscordHandler
    {
        public Task OnReady();
        public Task OnMessageCreate(DiscordReceivedMessage message);
    }
}
