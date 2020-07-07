using ChaosTerraria.ChaosUtils;
using ChaosTerraria.Structs;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Terraria;
using static Terraria.ModLoader.ModContent;

namespace ChaosTerraria.Network
{
    public class ChaosNetworkHelper
    {
        private readonly HttpClient httpClient;
        //private AuthInfo authInfo;
        private AuthResponse authResponse;
        private readonly String baseURI = "https://chaosnet.schematical.com/dev/";

        public ChaosNetworkHelper()
        {
            httpClient = new HttpClient();
        }

        public async void Auth(string username, string password)
        {
            AuthInfo authInfo;
            {
                authInfo.username = username;
                authInfo.password = password;
            }

            string content = JsonConvert.SerializeObject(authInfo);
            try
            {
                HttpResponseMessage response = await SendPostRequest(content, "auth/login");
                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.OK:
                        Main.NewText("Auth Success", Color.Green);
                        authResponse = JsonConvert.DeserializeObject<AuthResponse>(await response.Content.ReadAsStringAsync());
                        ChaosNetConfig.data.accessToken = authResponse.accessToken;
                        ChaosNetConfig.data.refreshToken = authResponse.refreshToken;
                        ChaosNetConfig.data.expiration = authResponse.expiration;
                        ChaosNetConfig.data.username = username;
                        ChaosNetConfig.Save();
                        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Authorization", ChaosNetConfig.data.accessToken);
                        StartSession();
                        break;
                    case System.Net.HttpStatusCode.BadRequest:
                        throw new Exception("Uh-oh, wrong data being sent.");
                    case System.Net.HttpStatusCode.Forbidden:
                        throw new Exception("Forbidden, Check your Username and password!");
                    default:
                        throw new Exception("Something went wrong " + response.StatusCode);
                }
            }
            catch
            {

            }
        }

        public async void StartSession(bool reset = true)
        {
            string endpoint = "/" + ChaosNetConfig.data.username + "/trainingrooms/" + ChaosNetConfig.data.trainingRoomNamespace + "/sessions/start";
            SessionStartInfo sessionStartInfo;
            {
                sessionStartInfo.username = ChaosNetConfig.data.username;
                sessionStartInfo.trainingroom = ChaosNetConfig.data.trainingRoomNamespace;
                sessionStartInfo.TrainingRoomStartRequest.reset = reset;
            }

            string content = JsonConvert.SerializeObject(sessionStartInfo);

            try
            {
                HttpResponseMessage response = await SendPostRequest(content, endpoint);
                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.OK:
                        Main.NewText("All Good! Session Started!", Color.Green);
                        break;
                    case System.Net.HttpStatusCode.BadRequest:
                        throw new Exception("Uh-oh, wrong data being sent.");
                    case System.Net.HttpStatusCode.Forbidden:
                        throw new Exception("Forbidden, Check your Username and password!");
                    default:
                        throw new Exception("Something went wrong " + response.StatusCode);
                }
            }
            catch
            {

            }
        }

        private async Task<HttpResponseMessage> SendPostRequest(string json, string endpoint, string headers = "", string headerName="",  bool headersRequired=false)
        {
            string uri = baseURI + endpoint;
            StringContent content = new StringContent(json);
            if (headersRequired)
            {
                content.Headers.Add(headerName, headers);
            }
            return await httpClient.PostAsync(uri, content);
        }

        private async Task<HttpResponseMessage> SendGetRequest(string endpoint)
        {
            string uri = baseURI + endpoint;
            return await httpClient.GetAsync(uri);
        }
    }
}
