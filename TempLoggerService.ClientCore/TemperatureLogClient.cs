using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TempLoggerService.ModelsCore;

namespace TempLoggerService.ClientCore
{
    public class TemperatureLogClient : ITemperatureLogClient
    {
        private HttpClient _client;

        public TemperatureLogClient(Uri serviceEndpoint)
        {
            _client = new HttpClient();

            _client.BaseAddress = serviceEndpoint;
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.TransferEncodingChunked = false;
        }

        public async Task<Guid> GetDeviceGuidByName(string name)
        {
            Guid id = Guid.Empty;
            HttpResponseMessage response = await _client.GetAsync(String.Format("/api/device/{0}", name));
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                id = (await CreateDevice(name)).DeviceId;
            }
            else if (response.StatusCode == HttpStatusCode.OK)
            {
                id = (await response.Content.ReadAsAsync<Device>()).DeviceId;
            }
            else
                throw new Exception("Unable to fetch device GUID");

            if (id == Guid.Empty)
                throw new Exception("For some reason we still failed to get the device GUID.");

            return id;
        }

        public async Task SetTemperature(Guid devID, decimal temperature)
        {
            var t = new Temperature()
            {
                Value = temperature,
                DeviceId = devID
            };
            //var stringContent = new StringContent(JsonConvert.SerializeObject(t), Encoding.UTF8, "application/json");
            //for some reason, using PostAsJsonAsync here does not work. no content-length header is added and HttpClient uses chunked encoding which it seems
            //that ASP.NET can't deal with. Manually doing the JSON conversion fixes the problem...
            HttpResponseMessage postresp = await _client.PostAsJsonAsync("/api/temperature", t);

            //HttpResponseMessage postresp = await _client.PostAsync("api/temperature/LogTemp", stringContent);
            if (!postresp.IsSuccessStatusCode)
                throw new Exception("Failed to log temperature.");
        }

        public async Task SetTemperature(string device, decimal temperature)
        {
            await SetTemperature(await GetDeviceGuidByName(device), temperature);
        }

        private async Task<Device> CreateDevice(string name)
        {
            HttpResponseMessage createresp = await _client.PostAsJsonAsync("/api/device", name);
            if (createresp.StatusCode != HttpStatusCode.Created)
                throw new Exception("Unable to create device: " + createresp.StatusCode);
            else
                return await createresp.Content.ReadAsAsync<Device>();
        }
    }
}
