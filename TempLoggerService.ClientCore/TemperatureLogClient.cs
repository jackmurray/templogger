using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using TempLoggerService.Models;

namespace TempLoggerService.ClientCore
{
    public class TemperatureLogClient : ITemperatureLogClient
    {
        private HttpClient _client;

        public TemperatureLogClient(Uri serviceEndpoint)
        {
            _client = new HttpClient();

            // New code:
            _client.BaseAddress = serviceEndpoint;
            //client.BaseAddress = new Uri("http://localhost:11317/");
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.TransferEncodingChunked = false;
        }

        public Guid GetDeviceGuidByName(string name)
        {
            Guid id = Guid.Empty;
            HttpResponseMessage response = _client.GetAsync(String.Format("api/device/{0}", name)).Result;
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                id = CreateDevice(name);
            }
            else if (response.StatusCode == HttpStatusCode.OK)
            {
                id = response.Content.ReadAsAsync<Guid>().Result;
            }
            else
                throw new Exception("Unable to fetch device GUID");

            if (id == Guid.Empty)
                throw new Exception("For some reason we still failed to get the device GUID.");

            return id;
        }

        public void SetTemperature(Guid devID, decimal temperature)
        {
            TempEntry n = new TempEntry()
            {
                temp = temperature,
                device = devID,
                timestamp = DateTime.UtcNow
            };
            var stringContent = new StringContent(JsonConvert.SerializeObject(n), Encoding.UTF8, "application/json");
            //for some reason, using PostAsJsonAsync here does not work. no content-length header is added and HttpClient uses chunked encoding which it seems
            //that ASP.NET can't deal with. Manually doing the JSON conversion fixes the problem...
            //HttpResponseMessage postresp = client.PostAsJsonAsync("api/temperature/LogTemp", n).Result;

            HttpResponseMessage postresp = _client.PostAsync("api/temperature/LogTemp", stringContent).Result;
            if (!postresp.IsSuccessStatusCode)
                throw new Exception("Failed to log temperature.");
        }

        public void SetTemperature(string device, decimal temperature)
        {
            SetTemperature(GetDeviceGuidByName(device), temperature);
        }

        private Guid CreateDevice(string name)
        {
            HttpResponseMessage createresp = _client.PostAsJsonAsync("api/device/", name).Result;
            if (createresp.StatusCode != HttpStatusCode.OK)
                throw new Exception("Unable to create device: " + createresp.StatusCode);
            else
                return createresp.Content.ReadAsAsync<Guid>().Result;
        }
    }
}
