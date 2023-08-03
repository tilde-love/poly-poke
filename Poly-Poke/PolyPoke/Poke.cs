using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PolyPoke
{
    public abstract class Poke : ScriptableObject
    {
        public abstract UniTask PokeAsync(IPoker poker, IPokeArbitrator pokeArbitrator, Camera camera, CancellationToken cancel);
    }
}