using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Worms.Behaviour {
    internal sealed class RemoteBehavior : IBehaviour {
        private readonly HttpClient client;
        private readonly string url;
        
        public RemoteBehavior(
            IHttpClientFactory factory,
            string url_
            ) => (client, url) = (factory.CreateClient(), url_);

        public Action NextAction(ISimulationState simulation, Worm worm) {
            var json = JsonSerializer.Serialize(DTO.World.CreateFrom(simulation));
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = client.PostAsync($"{url}/{worm.Name}/getAction", content).Result;
            response.EnsureSuccessStatusCode();

            using var stream = response.Content.ReadAsStream();
            using var responseReader = new StreamReader(stream);
            var actionResponse = JsonSerializer.Deserialize<DTO.Response>(responseReader.ReadToEnd())!;

            return actionResponse.Action.ToWormAction();
        }
    }
}