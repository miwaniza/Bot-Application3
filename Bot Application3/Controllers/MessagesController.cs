using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RestSharp;

namespace Bot_Application3.Controllers
{
    
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        public class RootObject
        {
            public string token_type { get; set; }
            public int expires_in { get; set; }
            public string refresh_token { get; set; }
            public string access_token { get; set; }
        }
        public RootObject JSONresp = new RootObject();
        public string token1;
        public int i=0;
        public ConnectorClient connectorMain;
        [AcceptVerbs("GET", "HEAD")]
        public string ParseCallback()
        {
            Uri oAuthURI = Request.RequestUri;
            token1 = HttpUtility.ParseQueryString(oAuthURI.Query).Get("code");

            string client_id = "m6nODcjyTKtwZ8eDqEWYr0ODsY6QXG7O";
            string client_secret = "dt6I7rdABZ50CLFo";
            var client = new RestClient("https://developer.api.autodesk.com/authentication/v1/gettoken");
            string redirect_uri = "https://forgebot3legged.azurewebsites.net/api/messages";
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddHeader("cache-control", "no-cache");
            request.AddParameter("application/x-www-form-urlencoded", $"grant_type=authorization_code&client_id={client_id}&client_secret={client_secret}&redirect_uri={redirect_uri}", ParameterType.RequestBody);
            IRestResponse response1 = client.Execute(request);
            JSONresp = JsonConvert.DeserializeObject<RootObject>(response1.Content.ToString());
            return JSONresp.access_token;
        }
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            string redirect_uri = "https://forgebot3legged.azurewebsites.net/api/messages";
            var responseType = "code";
            string client_id = "m6nODcjyTKtwZ8eDqEWYr0ODsY6QXG7O";
            var scope = "data:read data:write data:create data:search bucket:create bucket:read";
            var redirectionUrl = "https://developer.api.autodesk.com/authentication/v1/authorize?" +
             "response_type=" + HttpUtility.UrlEncode(responseType) +
             "&client_id=" + HttpUtility.UrlEncode(client_id) +
             "&redirect_uri=" + HttpUtility.UrlEncode(redirect_uri) +
             "&scope=" + HttpUtility.UrlEncode(scope);

            if (activity.Type == ActivityTypes.Message)
            {

                connectorMain = new ConnectorClient(new Uri(activity.ServiceUrl));
                // calculate something for us to return
                int length = (activity.Text ?? string.Empty).Length;

                // return our reply to the user
                Activity reply = activity.CreateReply($"go to {redirectionUrl} and get a token");
                await connectorMain.Conversations.ReplyToActivityAsync(reply);
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }
            return null;
        }
        }

}