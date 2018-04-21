using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using Bot.Messenger;
using System.Web.Http.Controllers;
using Bot.Messenger.Models;
using System.IO;

namespace MessengerBot.Controllers
{
    public class WebhookController : ApiController
    {
        string _pageToken = "";
        string _appSecret = "";
        string _verifyToken = "";

        string _quickReplyPayload_IsUserMsg = "WAS_USER_MESSAGE";
        string _quickReplyPayload_IsNotUserMsg = "WAS_NOT_USER_MESSAGE";


        private MessengerPlatform _Bot { get; set; }

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);

            /***Credentials are fetched from web.config ApplicationSettings when the CreateInstance
            ----method is called without a credentials parameter or if the parameterless constructor
            ----is used to initialize the MessengerPlatform class. This holds true for all types that inherit from
            ----Bot.Messenger.ApiBase

                _Bot = MessengerPlatform.CreateInstance();
                _Bot = new MessengerPlatform();
            ***/

            _Bot = MessengerPlatform.CreateInstance(
                MessengerPlatform.CreateCredentials(_appSecret, _pageToken, _verifyToken));
        }

        public HttpResponseMessage Get()
        {
            var querystrings = Request.GetQueryNameValuePairs().ToDictionary(x => x.Key, x => x.Value);

            if (_Bot.Authenticator.VerifyToken(querystrings["hub.verify_token"]))
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(querystrings["hub.challenge"], Encoding.UTF8, "text/plain")
                };
            }

            return new HttpResponseMessage(HttpStatusCode.Unauthorized);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Post()
        {
            var body = await Request.Content.ReadAsStringAsync();

            LogInfo("WebHook_Received", new Dictionary<string, string>
            {
                { "Request Body", body }
            });

            if (!_Bot.Authenticator.VerifySignature(Request.Headers.GetValues("X-Hub-Signature").FirstOrDefault(), body))
                return new HttpResponseMessage(HttpStatusCode.BadRequest);

            WebhookModel webhookModel = _Bot.ProcessWebhookRequest(body);

            if (webhookModel._Object != "page")
                return new HttpResponseMessage(HttpStatusCode.OK);

            foreach (var entry in webhookModel.Entries)
            {
                foreach (var evt in entry.Events)
                {
                    if (evt.EventType == WebhookEventType.PostbackRecievedCallback
                        || evt.EventType == WebhookEventType.MessageReceivedCallback)
                    {
                        await _Bot.SendApi.SendActionAsync(evt.Sender.ID, SenderAction.typing_on);

                        var userProfileRsp = await _Bot.UserProfileApi.GetUserProfileAsync(evt.Sender.ID);

                        if (evt.EventType == WebhookEventType.PostbackRecievedCallback)
                        {
                            await ProcessPostBack(evt.Sender.ID, userProfileRsp?.FirstName, evt.Postback);
                        }
                        if (evt.EventType == WebhookEventType.MessageReceivedCallback)
                        {
                            if (evt.Message.IsQuickReplyPostBack)
                                await ProcessPostBack(evt.Sender.ID, userProfileRsp?.FirstName, evt.Message.QuickReplyPostback);
                            else
                            {
                                bool hasReservoirName = false;
                                if (string.IsNullOrEmpty(evt.Message.Text) == false)
                                {
                                    if (evt.Message.Text.ToLower().Contains("caonillas"))
                                    {
                                        hasReservoirName = true;
                                        ReservoirData currentData = GetReservoirLevelAsync("50026140", 1);
                                        string CurrentLevel = currentData.GetCurrentLevel().ToString();
                                        string date = currentData.getDate();
                                        string time = currentData.getTime();
                                        await _Bot.SendApi.SendTextAsync(evt.Sender.ID, "El nivel en Caonillas es " + CurrentLevel + " metros. Datos recopilados el " + date + " a las " + time);
                                    }
                                    if (evt.Message.Text.ToLower().Contains("carite"))
                                    {
                                        hasReservoirName = true;
                                        ReservoirData currentData = GetReservoirLevelAsync("50039995", 3);
                                        string CurrentLevel = currentData.GetCurrentLevel().ToString();
                                        string date = currentData.getDate();
                                        string time = currentData.getTime();
                                        await _Bot.SendApi.SendTextAsync(evt.Sender.ID, "El nivel en Carite es " + CurrentLevel + " metros. Datos recopilados el " + date + " a las " + time);
                                    }
                                    if (evt.Message.Text.ToLower().Contains("carraizo") || (evt.Message.Text.ToLower().Contains("carraízo")))
                                    {
                                        hasReservoirName = true;
                                        ReservoirData currentData = GetReservoirLevelAsync("50059000", 2);
                                        string CurrentLevel = currentData.GetCurrentLevel().ToString();
                                        string date = currentData.getDate();
                                        string time = currentData.getTime();
                                        await _Bot.SendApi.SendTextAsync(evt.Sender.ID, "El nivel en Carraizo es " + CurrentLevel + " metros. Datos recopilados el " + date + " a las " + time);
                                    }
                                    if (evt.Message.Text.ToLower().Contains("cerrillos"))
                                    {
                                        hasReservoirName = true;
                                        ReservoirData currentData = GetReservoirLevelAsync("50113950", 2);
                                        string CurrentLevel = currentData.GetCurrentLevel().ToString();
                                        string date = currentData.getDate();
                                        string time = currentData.getTime();
                                        await _Bot.SendApi.SendTextAsync(evt.Sender.ID, "El nivel en Cerrillos es " + CurrentLevel + " metros. Datos recopilados el " + date + " a las " + time);
                                    }
                                    if (evt.Message.Text.ToLower().Contains("cidra"))
                                    {
                                        hasReservoirName = true;
                                        ReservoirData currentData = GetReservoirLevelAsync("50047550", 2);
                                        string CurrentLevel = currentData.GetCurrentLevel().ToString();
                                        string date = currentData.getDate();
                                        string time = currentData.getTime();
                                        await _Bot.SendApi.SendTextAsync(evt.Sender.ID, "El nivel en Cidra es " + CurrentLevel + " metros. Datos recopilados el " + date + " a las " + time);
                                    }
                                    if (evt.Message.Text.ToLower().Contains("fajardo"))
                                    {
                                        hasReservoirName = true;
                                        ReservoirData currentData = GetReservoirLevelAsync("50071225", 1);
                                        string CurrentLevel = currentData.GetCurrentLevel().ToString();
                                        string date = currentData.getDate();
                                        string time = currentData.getTime();
                                        await _Bot.SendApi.SendTextAsync(evt.Sender.ID, "El nivel en Fajardo es " + CurrentLevel + " metros. Datos recopilados el " + date + " a las " + time);
                                    }
                                    if (evt.Message.Text.ToLower().Contains("guajataca"))
                                    {
                                        hasReservoirName = true;
                                        ReservoirData currentData = GetReservoirLevelAsync("50010800", 1);
                                        string CurrentLevel = currentData.GetCurrentLevel().ToString();
                                        string date = currentData.getDate();
                                        string time = currentData.getTime();
                                        await _Bot.SendApi.SendTextAsync(evt.Sender.ID, "El nivel en Guajataca es " + CurrentLevel + " metros. Datos recopilados el " + date + " a las " + time);
                                    }
                                    if (evt.Message.Text.ToLower().Contains("la plata"))
                                    {
                                        hasReservoirName = true;
                                        ReservoirData currentData = GetReservoirLevelAsync("50045000", 1);
                                        string CurrentLevel = currentData.GetCurrentLevel().ToString();
                                        string date = currentData.getDate();
                                        string time = currentData.getTime();
                                        await _Bot.SendApi.SendTextAsync(evt.Sender.ID, "El nivel en La Plata es " + CurrentLevel + " metros. Datos recopilados el " + date + " a las " + time);
                                    }
                                    if (evt.Message.Text.ToLower().Contains("patillas"))
                                    {
                                        hasReservoirName = true;
                                        ReservoirData currentData = GetReservoirLevelAsync("50093045", 3);
                                        string CurrentLevel = currentData.GetCurrentLevel().ToString();
                                        string date = currentData.getDate();
                                        string time = currentData.getTime();
                                        await _Bot.SendApi.SendTextAsync(evt.Sender.ID, "El nivel en Patillas es " + CurrentLevel + " metros. Datos recopilados el " + date + " a las " + time);
                                    }
                                    if (evt.Message.Text.ToLower().Contains("rio blanco"))
                                    {
                                        hasReservoirName = true;
                                        ReservoirData currentData = GetReservoirLevelAsync("50076800", 1);
                                        string CurrentLevel = currentData.GetCurrentLevel().ToString();
                                        string date = currentData.getDate();
                                        string time = currentData.getTime();
                                        await _Bot.SendApi.SendTextAsync(evt.Sender.ID, "El nivel en Rio Blanco es " + CurrentLevel + " metros. Datos recopilados el " + date + " a las " + time);
                                    }
                                    if (evt.Message.Text.ToLower().Contains("toa vaca"))
                                    {
                                        hasReservoirName = true;
                                        ReservoirData currentData = GetReservoirLevelAsync("50111210", 3);
                                        string CurrentLevel = currentData.GetCurrentLevel().ToString();
                                        string date = currentData.getDate();
                                        string time = currentData.getTime();
                                        await _Bot.SendApi.SendTextAsync(evt.Sender.ID, "El nivel en Toa Vaca es " + CurrentLevel + " metros. Datos recopilados el " + date + " a las " + time);
                                    }
                                    if (hasReservoirName == false)
                                    {
                                        await _Bot.SendApi.SendTextAsync(evt.Sender.ID, $"{userProfileRsp?.FirstName}, No reconocí ese embalse.");
                                    }
                                    else
                                    {
                                        await _Bot.SendApi.SendTextAsync(evt.Sender.ID, "¿Quieres saber más sobre las condiciones de los embalses? ¡Descarga el app para Android ya! https://play.google.com/store/apps/details?id=msc.app.embalsespuertorico");
                                    }
                                }
                                else
                                {
                                    await _Bot.SendApi.SendTextAsync(evt.Sender.ID, $"{userProfileRsp?.FirstName}, Para usar este bot, debe escribir el nombre de un embalse.");
                                }
                               
                            }
                        }
                    }

                    await _Bot.SendApi.SendActionAsync(evt.Sender.ID, SenderAction.typing_off);

                }
            }

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        private async Task ConfirmIfCorrect(WebhookEvent evt)
        {
            SendApiResponse sendQuickReplyResponse = await _Bot.SendApi.SendTextAsync(evt.Sender.ID, "Is that you message?", new List<QuickReply>
            {
                new QuickReply
                {
                    ContentType = QuickReplyContentType.text,
                    Title = "Yes",
                    Payload = _quickReplyPayload_IsUserMsg
                },
                new QuickReply
                {
                    ContentType = QuickReplyContentType.text,
                    Title = "No",
                    Payload = _quickReplyPayload_IsNotUserMsg
                }
            });

            LogSendApiResponse(sendQuickReplyResponse);
        }

        private async Task ResendMessageToUser(WebhookEvent evt)
        {
            SendApiResponse response = new SendApiResponse();

            if (evt.Message.Attachments == null)
            {
                string text = evt.Message?.Text;

                if (string.IsNullOrWhiteSpace(text))
                    text = "Hello :)";

                response = await _Bot.SendApi.SendTextAsync(evt.Sender.ID, $"Your Message => {text}");
            }
            else
            {
                foreach (var attachment in evt.Message.Attachments)
                {
                    if (attachment.Type != AttachmentType.fallback && attachment.Type != AttachmentType.location)
                    {
                        response = await _Bot.SendApi.SendAttachmentAsync(evt.Sender.ID, attachment);
                    }
                }
            }

            LogSendApiResponse(response);
        }

        private async Task ProcessPostBack(string userId, string username, Postback postback)
        {
            if (postback.Payload == _quickReplyPayload_IsNotUserMsg)
                await _Bot.SendApi.SendTextAsync(userId, $"Sorry about that {username}, try sending something else.");
            else if (postback.Payload == _quickReplyPayload_IsUserMsg)
                await _Bot.SendApi.SendTextAsync(userId, $"Yay! We got it.");
        }

        private static void LogSendApiResponse(SendApiResponse response)
        {
            LogInfo("SendApi Web Request", new Dictionary<string, string>
            {
                { "Response", response?.ToString() }
            });
        }

        private static void LogInfo(string eventName, Dictionary<string, string> telemetryProperties)
        {
            //Log telemetry in DB or Application Insights
        }
        private ReservoirData GetReservoirLevelAsync(string zoneID, int ArrayData)
        {
            string CurrentLevel = "";
            WebRequest request = WebRequest.Create("https://waterservices.usgs.gov/nwis/iv/?sites=" + zoneID.ToString() + "&period=P1D&format=json");
            System.Net.WebResponse response =  request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string ResponseFromServer = reader.ReadToEnd();
            JObject JsonDataResult = JObject.Parse(ResponseFromServer);
            IList<JToken> results = JsonDataResult["value"]["timeSeries"][ArrayData]["values"].Children().ToList();

            JToken LastItemInJson = results[results.Count() - 1];
            JToken Item1 = LastItemInJson["value"];
            JToken Item2 = Item1[Item1.Count() - 1];
            CurrentLevel = Item2.SelectToken("value").ToString();
            DateTime dateAndTime = DateTime.Parse(Item2.SelectToken("dateTime").ToString());
            string MonthName = string.Empty;
            if (dateAndTime.ToString("MM") == "01")
                MonthName = "enero";
            else if (dateAndTime.ToString("MM") == "02")
                MonthName = "febrero";
            else if (dateAndTime.ToString("MM") == "03")
                MonthName = "marzo";
            else if (dateAndTime.ToString("MM") == "04")
                MonthName = "abril";
            else if (dateAndTime.ToString("MM") == "05")
                MonthName = "mayo";
            else if (dateAndTime.ToString("MM") == "06")
                MonthName = "junio";
            else if (dateAndTime.ToString("MM") == "07")
                MonthName = "julio";
            else if (dateAndTime.ToString("MM") == "08")
                MonthName = "agosto";
            else if (dateAndTime.ToString("MM") == "09")
                MonthName = "septiembre";
            else if (dateAndTime.ToString("MM") == "10")
                MonthName = "octubre";
            else if (dateAndTime.ToString("MM") == "11")
                MonthName = "noviembre";
            else if (dateAndTime.ToString("MM") == "12")
                MonthName = "diciembre";
            string FullDate = dateAndTime.ToString("dd") + " de " + MonthName + " de " + dateAndTime.ToString("yyyy");
            string Time = dateAndTime.ToString("hh:mm tt");
            return new ReservoirData(CurrentLevel, FullDate, Time);
        }
        public class ReservoirData
        {
            private string CurrentLevel = string.Empty;
            private string date = string.Empty;
            private string time = string.Empty;
            public ReservoirData()
            {
                CurrentLevel = string.Empty;
                date = string.Empty;
                time = string.Empty;
            }
            public ReservoirData(string GetCurrentLevel, string getDate, string getTime)
            {
                CurrentLevel = GetCurrentLevel;
                date = getDate;
                time = getTime;
            }
            public string GetCurrentLevel()
            {
                return CurrentLevel;
            }
            public string getDate()
            {
                return date;
            }
            public string getTime()
            {
                return time;
            }
        }
    }
}

