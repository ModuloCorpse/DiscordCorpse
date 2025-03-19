using CorpseLib.DataNotation;
using CorpseLib.Json;
using CorpseLib.Logging;
using CorpseLib.Network;
using CorpseLib.Web;
using CorpseLib.Web.Http;

namespace DiscordCorpse
{
    public class DiscordAPI(string accessToken)
    {
        public static readonly Logger DISCORD_API = new("[${d}-${M}-${y} ${h}:${m}:${s}.${ms}] ${log}") { new LogInFile("./log/${y}${M}${d}${h}-DiscordAPI.log") };
        public static void StartLogging() => DISCORD_API.Start();
        public static void StopLogging() => DISCORD_API.Stop();

        private readonly string m_AccessToken = accessToken;

        internal string Token => m_AccessToken;

        private void FillHeader(URLRequest request)
        {
            request.AddHeaderField("Authorization", string.Format("Bot {0}", m_AccessToken));
            request.AddHeaderField("User-Agent", string.Format("DiscordBot (https://github.com/ModuloCorpse/DiscordCorpse, {0})", "1.0.0-alpha"));
        }

        private void SendComposedRequestWithoutResponse(URLRequest request)
        {
            FillHeader(request);
            DISCORD_API.Log(string.Format("Sending: {0}", request.Request.ToString()));
            request.SendWithoutResponse();
        }

        private Response SendComposedRequest(URLRequest request)
        {
            FillHeader(request);
            DISCORD_API.Log(string.Format("Sending: {0}", request.Request.ToString()));
            Response response = request.Send();
            DISCORD_API.Log(string.Format("Received: {0}", response.ToString()));
            return response;
        }

        private void SendRequestWithoutResponse(Request.MethodType method, string url, DataObject content)
        {
            URLRequest request = new(URI.Parse(url), method, JsonParser.NetStr(content));
            request.AddContentType(MIME.APPLICATION.JSON);
            SendComposedRequestWithoutResponse(request);
        }

        private Response SendRequest(Request.MethodType method, string url, DataObject content)
        {
            URLRequest request = new(URI.Parse(url), method, JsonParser.NetStr(content));
            request.AddContentType(MIME.APPLICATION.JSON);
            return SendComposedRequest(request);
        }

        private void SendRequestWithoutResponse(Request.MethodType method, string url) => SendComposedRequestWithoutResponse(new(URI.Parse(url), method));

        private Response SendRequest(Request.MethodType method, string url) => SendComposedRequest(new(URI.Parse(url), method));

        internal string GetGatewayURL()
        {
            Response response = SendRequest(Request.MethodType.GET, "https://discordapp.com/api/gateway");
            if (response.StatusCode == 200)
            {
                DataObject jsonResponse = JsonParser.Parse(response.Body);
                return jsonResponse.Get<string>("url")!;
            }
            return string.Empty;
        }

        internal void CrossPostMessage(string channelID, string messageID)
        {
            SendRequest(Request.MethodType.POST, string.Format("https://discordapp.com/api/channels/{0}/messages/{1}/crosspost", channelID, messageID));
        }

        internal void SendMessageToChannel(string channelID, DiscordMessage message)
        {
            SendRequest(Request.MethodType.POST, string.Format("https://discordapp.com/api/channels/{0}/messages", channelID), message.Serialize());
        }

        internal void DeleteMessage(string channelID, string messageID)
        {
            SendRequest(Request.MethodType.DELETE, string.Format("https://discordapp.com/api/channels/{0}/messages/{1}", channelID, messageID));
        }
    }
}
