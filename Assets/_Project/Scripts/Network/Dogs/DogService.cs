using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace Assets._Project.Scripts.Network.Dogs
{
    public class DogService : IDogService
    {
        private const string Url = "https://dog.ceo/api/breeds/list/all";

        public async UniTask<List<DogBreed>> GetBreedsAsync(CancellationToken ct)
        {
            using var request = UnityWebRequest.Get(Url);
            var operation = await request.SendWebRequest().WithCancellation(ct);

            if (request.result != UnityWebRequest.Result.Success)
                throw new System.Exception(request.error);

            var json = request.downloadHandler.text;
            var data = JsonConvert.DeserializeObject<DogApiResponse>(json);

            return data.message
                .SelectMany(kvp => kvp.Value.Count == 0
                    ? new[] { new DogBreed(kvp.Key) }
                    : kvp.Value.Select(sub => new DogBreed($"{kvp.Key} {sub}")))
                .ToList();
        }

        private class DogApiResponse
        {
            public Dictionary<string, List<string>> message;
            public string status;
        }
    }
}
