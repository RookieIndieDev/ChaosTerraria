using ChaosTerraria.ChaosUtils;
using ChaosTerraria.Managers;
using ChaosTerraria.Structs;
using ChaosTerraria.UI;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using log4net;

namespace ChaosTerraria.Network
{
    //TODO: GetPackage() Called separately instead of being called by StartSession()?
    public class ChaosNetworkHelper
    {
        private static HttpClient httpClient;
        //private AuthInfo authInfo;
        private AuthResponse authResponse;
        private readonly String baseURI = "https://chaosnet.schematical.com/v0/";
        private string refreshMessage;
        private ILog logger = ModContent.GetInstance<ChaosTerraria>().Logger;

        public ChaosNetworkHelper()
        {
            httpClient = new HttpClient();
        }

        public async void Auth(string username, string password, string trainingRoomOwnerUsername)
        {
            AuthInfo authInfo;
            {
                authInfo.username = username;
                authInfo.password = password;
            }

            string content = JsonConvert.SerializeObject(authInfo);
            HttpResponseMessage response = await SendPostRequest(content, "auth/login");
            switch (response.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    Main.NewText("Auth Success", Color.Green);
                    authResponse = JsonConvert.DeserializeObject<AuthResponse>(await response.Content.ReadAsStringAsync());
                    ChaosNetConfig.data.accessToken = authResponse.accessToken;
                    ChaosNetConfig.data.refreshToken = authResponse.refreshToken;
                    ChaosNetConfig.data.expiration = authResponse.expiration;
                    ChaosNetConfig.data.username = username.ToLower();
                    ChaosNetConfig.Save();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(ChaosNetConfig.data.accessToken);
                    UIHandler.ShowSessionScreen();
                    break;
                case System.Net.HttpStatusCode.BadRequest:
                    logger.Error("/auth: Uh-oh, wrong data being sent.");
                    Main.NewText("/auth: Uh-oh, wrong data being sent", Color.Red);
                    break;
                case System.Net.HttpStatusCode.Forbidden:
                    logger.Error("/auth: Forbidden, Check your Username and password!");
                    Main.NewText("/auth: Forbidden, Check your Username and password!", Color.Red);
                    break;
                case System.Net.HttpStatusCode.Unauthorized:
                    logger.Error("/auth: Unauthorized!");
                    Main.NewText("/auth: Unauthorized", Color.Red);
                    break;
                default:
                    logger.Error("/auth: Something went wrong " + response.StatusCode);
                    Main.NewText("/auth: Something went wrong " + response.StatusCode + " Check client.log", Color.Red);
                    break;
            }
        }

        public async void StartSession(bool reset = false)
        {
            string endpoint = $"{ChaosNetConfig.data.trainingRoomUsernameNamespace}/trainingrooms/{ChaosNetConfig.data.trainingRoomNamespace}/sessions/start";

            AddAuthorizationHeader();

            SessionStartInfo sessionStartInfo;
            {
                sessionStartInfo.TrainingRoomStartRequest.reset = reset;
            }

            string content = JsonConvert.SerializeObject(sessionStartInfo);

            HttpResponseMessage response = await SendPostRequest(content, endpoint);
            switch (response.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    Main.NewText("All Good! Session Started!", Color.Green);
                    string responseString = await response.Content.ReadAsStringAsync();
                    SessionStartResponse sessionStartResponse = JsonConvert.DeserializeObject<SessionStartResponse>(responseString);
                    SessionManager.CurrentSession = sessionStartResponse.session;
                    ChaosNetConfig.data.sessionNamespace = SessionManager.CurrentSession.nameSpace;
                    ChaosNetConfig.Save();
                    GetPackage();
                    DoSessionNext();
                    break;
                case System.Net.HttpStatusCode.BadRequest:
                    logger.Error("Session: Uh-oh, wrong data being sent.");
                    Main.NewText("Session: Uh-oh, wrong data being sent.", Color.Red);
                    break;
                case System.Net.HttpStatusCode.Forbidden:
                    refreshMessage = "Session: Forbidden! Try logging in again!";
                    DoTokenRefresh(refreshMessage);
                    break;
                case System.Net.HttpStatusCode.Unauthorized:
                    refreshMessage = "Session: Unauthorized, attempting token refresh! Try starting the session now!";
                    DoTokenRefresh(refreshMessage);
                    break;
                default:
                    if (response.StatusCode.ToString() == "418")
                    {
                        Main.NewText("Please Sleep the previous session and try starting the session again!", Color.Red);
                        logger.Error("Please Sleep the previous session and try starting the session again!");
                    }
                    logger.Error("Session: Something went wrong! " + response.Content.ReadAsStringAsync() + " " + response.StatusCode);
                    Main.NewText("Session: Something went wrong! " + response.StatusCode + " Check client.log", Color.Red);
                    break;
            }
        }

        public async void DoSessionNext()
        {
            string endpoint = $"{ChaosNetConfig.data.trainingRoomUsernameNamespace}/trainingrooms/{ChaosNetConfig.data.trainingRoomNamespace}/sessions/{ChaosNetConfig.data.sessionNamespace}/next";
            TrainingRoomSessionNextRequest trainingRoomSessionNextRequest;
            {
                trainingRoomSessionNextRequest.nNetRaw = false;
                trainingRoomSessionNextRequest.report = SessionManager.Reports;
                trainingRoomSessionNextRequest.observedAttributes = SessionManager.ObservedAttributes;
            }

            SessionNextInfo sessionNextInfo;
            {
                sessionNextInfo.trainingRoomSessionNextRequest = trainingRoomSessionNextRequest;
            }

            AddAuthorizationHeader();

            string content = JsonConvert.SerializeObject(trainingRoomSessionNextRequest);
            HttpResponseMessage responseMessage = await SendPostRequest(content, endpoint);
            string response = await responseMessage.Content.ReadAsStringAsync();
            switch (responseMessage.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    SessionNextResponse sessionNextResponse = JsonConvert.DeserializeObject<SessionNextResponse>(response);
                    SessionManager.Organisms = sessionNextResponse.organisms;
                    SessionManager.CurrentStats = sessionNextResponse.stats;
                    SessionManager.Species = sessionNextResponse.species;
                    if (SessionManager.Reports != null)
                        SessionManager.Reports.Clear();
                    if (SessionManager.ObservedAttributes != null)
                        SessionManager.ObservedAttributes.Clear();
                    break;
                case System.Net.HttpStatusCode.BadRequest:
                    logger.Error("/next: " + response);
                    Main.NewText("/next: " + responseMessage.StatusCode + " Check client.log", Color.Red);
                    break;
                case System.Net.HttpStatusCode.Forbidden:
                    refreshMessage = "/next: Forbidden! Refreshing tokens!";
                    DoTokenRefresh(refreshMessage);
                    break;
                case System.Net.HttpStatusCode.Unauthorized:
                    refreshMessage = "/next: Unauthorized! Refreshing tokens!";
                    DoTokenRefresh(refreshMessage);
                    break;
                default:
                    if (responseMessage.StatusCode.ToString() == "418")
                    {
                        logger.Error("Please Sleep the previous session and try starting the session again!");
                        Main.NewText("Please Sleep the previous session and try starting the session again!", Color.Red);
                        break;
                    }
                    logger.Error("/next: Something went wrong! " + response + " " + responseMessage.StatusCode);
                    Main.NewText("/next: Something went wrong! " + responseMessage.StatusCode + " Check client.log", Color.Red);
                    break;
            }
        }

        private void AddAuthorizationHeader()
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(ChaosNetConfig.data.accessToken);
        }

        private async void DoTokenRefresh(string message)
        {
            AuthTokenRequest tokenRequest;
            tokenRequest.username = ChaosNetConfig.data.username;
            tokenRequest.refreshToken = ChaosNetConfig.data.refreshToken;
            string json = JsonConvert.SerializeObject(tokenRequest);
            HttpResponseMessage responseMessage = await SendPostRequest(json, "auth/token");
            string response = await responseMessage.Content.ReadAsStringAsync();
            switch (responseMessage.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    if (response.Contains("error"))
                    {
                        logger.Error(response);
                        Main.NewText(response, Color.Red);
                    }
                    Main.NewText(message, Color.Green);
                    AuthResponse refreshResponse = JsonConvert.DeserializeObject<AuthResponse>(await responseMessage.Content.ReadAsStringAsync());
                    ChaosNetConfig.data.accessToken = refreshResponse.accessToken;
                    ChaosNetConfig.data.refreshToken = refreshResponse.refreshToken;
                    ChaosNetConfig.data.expiration = refreshResponse.expiration;
                    ChaosNetConfig.data.idToken = refreshResponse.idToken;
                    ChaosNetConfig.Save();
                    break;
                case System.Net.HttpStatusCode.BadRequest:
                    logger.Error ("Token Refresh: " + response);
                    Main.NewText("Token Refresh" + response, Color.Red);
                    break;
                case System.Net.HttpStatusCode.Forbidden:
                    logger.Error("Token Refresh: Forbidden, refresh token Expired, Try logging in again");
                    Main.NewText("Token Refresh: Forbidden, refresh token Expired, Try logging in again", Color.Red);
                    break;
                case System.Net.HttpStatusCode.Unauthorized:
                    logger.Error("Token Refresh: Unauthorized, refresh token Expired, Try logging in again");
                    Main.NewText("Token Refresh: Unauthorized, refresh token Expired, Try logging in again", Color.Red);
                    break;
                default:
                    if (responseMessage.StatusCode.ToString() == "418")
                    {
                        logger.Error("Please Sleep the previous session and try starting the session again!");
                        Main.NewText("Please Sleep the previous session and try starting the session again!", Color.Red);
                        break;
                    }
                    logger.Error("Token Refresh: Something went wrong! " + responseMessage.Content.ReadAsStringAsync() + " " + responseMessage.StatusCode);
                    Main.NewText("Token Refresh: Something went wrong! " + responseMessage.StatusCode, Color.Red);
                    break;
            }
        }

        private async void GetPackage()
        {
            string endpoint = $"{ChaosNetConfig.data.trainingRoomUsernameNamespace}/trainingrooms/{ChaosNetConfig.data.trainingRoomNamespace}/package";
            HttpResponseMessage response = await DoGet(endpoint);
            Package pack = JsonConvert.DeserializeObject<Package>(await response.Content.ReadAsStringAsync());
            SessionManager.Package = pack;
            foreach (Role role in SessionManager.Package.roles)
            {
                foreach (Setting setting in role.settings)
                {
                    if (setting.nameSpace == "ORG_BATCH_SIZE")
                    {
                        SpawnManager.spawnCount += int.Parse(setting.value);
                        break;
                    }
                }
            }
        }

        private async Task<HttpResponseMessage> SendPostRequest(string json, string endpoint, string headers = "", string headerName = "", bool headersRequired = false)
        {
            string uri = baseURI + endpoint;
            StringContent content = new StringContent(json);
            if (headersRequired)
            {
                content.Headers.Add(headerName, headers);
            }
            return await httpClient.PostAsync(uri, content);
        }

        private async Task<HttpResponseMessage> DoGet(string endpoint)
        {
            string uri = baseURI + endpoint;
            return await httpClient.GetAsync(uri);
        }

        private async Task<HttpResponseMessage> SendGetRequest(string json, string endpoint)
        {
            string uri = baseURI + endpoint;
            var request = new HttpRequestMessage(HttpMethod.Get, uri)
            {
                Content = new StringContent(json)
            };
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return await httpClient.SendAsync(request);
        }

    }
}
