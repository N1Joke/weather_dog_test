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
        private const string UrlDogList = "https://dogapi.dog/api/v2/breeds";
        private const string UrlDogInfoFormat = "https://dogapi.dog/api/v2/breeds/{0}";

        public async UniTask<List<DogBreed>> GetBreedsAsync(CancellationToken ct)
        {
            using var request = UnityWebRequest.Get(UrlDogList);
            var operation = await request.SendWebRequest().WithCancellation(ct);

            if (request.result != UnityWebRequest.Result.Success)
                throw new System.Exception(request.error);

            var json = request.downloadHandler.text;
            var data = JsonConvert.DeserializeObject<DogBreedsResponse>(json);

            return data.data.Select(d =>
                new DogBreed(d.id, d.attributes.name)
            ).ToList();
        }

        public async UniTask<DogBreedDetails> GetBreedInfoAsync(string id, CancellationToken ct, float delay)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: ct);

            using var request = UnityWebRequest.Get(string.Format(UrlDogInfoFormat, id));
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
