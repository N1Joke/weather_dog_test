using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace Assets._Project.Scripts.Network.Dogs
{
    public class DogService : IDogService
    {
        private const string Url = "https://dogapi.dog/api/v2/breeds";

        public async UniTask<List<DogBreed>> GetBreedsAsync(CancellationToken ct)
        {
            //await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: ct);

            using var request = UnityWebRequest.Get(Url);
            var operation = await request.SendWebRequest().WithCancellation(ct);

            if (request.result != UnityWebRequest.Result.Success)
                throw new System.Exception(request.error);

            var json = request.downloadHandler.text;
            var data = JsonConvert.DeserializeObject<DogBreedsResponse>(json);

            return data.data.Select(d =>
                new DogBreed(d.id, d.attributes.name)
            ).ToList();
        }

        public async UniTask<DogBreedDetails> GetBreedInfoAsync(string id, CancellationToken ct)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: ct);

            string url = $"https://dogapi.dog/api/v2/breeds/{id}";

            using var request = UnityWebRequest.Get(url);
            await request.SendWebRequest().WithCancellation(ct);

            var json = request.downloadHandler.text;
            var result = JsonConvert.DeserializeObject<DogBreedDetailsResponse>(json);

            var attr = result.data.attributes;

            return new DogBreedDetails(
                attr.name,
                attr.description
            );
        }

        [Serializable]
        private class DogBreedDetailsResponse
        {
            public BreedData data;
        }

        [Serializable]
        private class DogBreedsResponse
        {
            public List<BreedData> data;
        }

        [Serializable]
        private class BreedData
        {
            public string id;
            public BreedAttributes attributes;
        }

        [Serializable]
        private class BreedAttributes
        {
            public string name;
            public string description;
        }
    }
}
