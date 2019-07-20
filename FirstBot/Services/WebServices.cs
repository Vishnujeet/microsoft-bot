using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http;
using FirstBot.Helpers;
using FirstBot.Models;
using Newtonsoft.Json;
using RestSharp;

namespace FirstBot.Services
{
    public static class WebServices
    {

        private readonly static string baseURL = "https://domainName.net";
        public static FieldState GetFieldState(Guid fieldId)
        {
            var client = new RestClient($"{baseURL}/field/{fieldId}/state");
            var request = new RestRequest(Method.GET);
            FieldState fieldState = null;
            var accesstoken = GetToken();
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", $"Bearer {accesstoken.token}");
            var response = client.Execute(request);
            if (response.IsSuccessful)
            {
                fieldState = JsonConvert.DeserializeObject<FieldState>(response.Content);
            }
            return fieldState;
        }

        public static IRestResponse SetFieldState(Guid fieldId, FieldStateEnum state)
        {
            var client = new RestClient($"https:{baseURL}/field/{fieldId}/state");
            var request = new RestRequest(Method.PUT);
            var accesstoken = GetToken();
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/json");
            request.AddHeader("Authorization", $"Bearer {accesstoken.token}");
            request.AddParameter("application/json", "{\n    \"state\": \"{state}\"\n}", ParameterType.RequestBody);
            return client.Execute(request);
        }

        public static void GetSiteState(Guid fieldId)
        {
            var client = new RestClient($"https://{baseURL}/field/{fieldId}/state");
            var request = new RestRequest(Method.GET);
            var accesstoken = GetToken();
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", $"Bearer {accesstoken.token}");
            IRestResponse response = client.Execute(request);
        }

        public static AccessToken GetToken()
        {
            var client = new RestClient($"{baseURL}/oauth/accesstoken");
            var request = new RestRequest(Method.POST);
            AccessToken accessToken = null;
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("content-length", "79");
            request.AddHeader("accept-encoding", "gzip, deflate");
            request.AddHeader("Host", $"{baseURL}");
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Accept", "*/*");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Authorization", "Basic <your email id and pwassword encoded in base64>");
            request.AddParameter("undefined",
                "app_key=<app key>",
                ParameterType.RequestBody);
            var response = client.Execute(request);
            if (response.IsSuccessful)
            {
                accessToken = JsonConvert.DeserializeObject<AccessToken>(response.Content);
            }

            return accessToken;
        }

        public static List<Site> GetSites()
        {
            var client =
                new RestClient($"{baseURL}/control/SITES/");
            var request = new RestRequest(Method.GET);
            var accesstoken = GetToken();
            List<Site> site = null;
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", $"Bearer {accesstoken.token}");
            var response = client.Execute(request);
            if (response.IsSuccessful)
            {
                site = JsonConvert.DeserializeObject<List<Site>>(response.Content);
            }

            return site;
        }
        public enum FieldStateEnum { Match, off };

    }
    public class FieldState
    {
        public string State { get; set; }
    }
}
