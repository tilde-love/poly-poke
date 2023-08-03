using System.Threading;
using Cysharp.Threading.Tasks;
using Ossify;
using UnityEngine;

namespace PolyPoke
{
    public static class Pokecaster
    {
        public static async UniTask<Collider2D> WaitUntilObjectPickup(
            IPoker poker,
            IPokeArbitrator pokeArbitrator,
            Camera camera,
            bool enableDragThroughPickup,
            LayerMask layers,
            Dispenser<Whizz> whizzes,
            CancellationToken cancel)
        {
            Collider2D hit = null;
            
            Whizz whizz = null;
            Vector2 worldPoint = Vector2.zero;

            while (poker.IsPressed)
            {
                worldPoint = camera.ScreenToWorldPoint(poker.Position);

                if (whizz != null) whizz.During(worldPoint);

                hit = Physics2D.OverlapPoint(worldPoint, layers);                

                if (hit == null)
                {
                    if (enableDragThroughPickup == false) return null;

                    if (whizz == null && whizzes != null) 
                    {
                        whizz = whizzes.Dispense();
                        whizz.Begin(worldPoint);
                    }

                    await UniTask.NextFrame(cancel);

                    continue;
                }

                if (pokeArbitrator.IsLicenceAvailable(hit.gameObject) == false)
                {
                    if (enableDragThroughPickup == false) return null;

                    if (whizz == null && whizzes != null) 
                    {
                        whizz = whizzes.Dispense();
                        whizz.Begin(worldPoint);
                    }

                    await UniTask.NextFrame(cancel);

                    continue;
                }                

                break;
            }

            if (whizz != null) whizz.End(worldPoint);

            return hit;
        }
    }
}