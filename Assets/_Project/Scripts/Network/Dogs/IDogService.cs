﻿using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace Assets._Project.Scripts.Network.Dogs
{
    public interface IDogService
    {
        UniTask<List<DogBreed>> GetBreedsAsync(CancellationToken ct);
        UniTask<DogBreedDetails> GetBreedInfoAsync(string id, CancellationToken ct, float delay);
    }
}
