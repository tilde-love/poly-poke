using System.Threading;
using Cysharp.Threading.Tasks;
using Ossify;
using Ossify.Ballots;
using UnityEngine;

namespace PolyPoke
{
    [CreateAssetMenu(menuName = "Poly Poke/Raycast 2D Poke")]
    public class Raycast2DPoke : Poke
    {
        [SerializeField] private LayerMask layers;
        [SerializeField] private bool enableDragThroughPickup = true;
        [SerializeField] private Impulse begin;
        [SerializeField] private Impulse end;

        [SerializeField] private Dispenser<Whizz> noHitWhizz;
        [SerializeField] private Dispenser<Whizz> pickedWhizz;  

        /// <inheritdoc />
        public override async UniTask PokeAsync(IPoker poker, IPokeArbitrator pokeArbitrator, Camera camera, CancellationToken cancel)
        {
            while (cancel.IsCancellationRequested == false)
            {
                if (poker.IsPressed == false)
                {
                    await UniTask.NextFrame(cancel);
                    continue;
                }

                Collider2D hit = await Pokecaster.WaitUntilObjectPickup(
                    poker,
                    pokeArbitrator,
                    camera,
                    enableDragThroughPickup,
                    layers,
                    noHitWhizz,
                    cancel
                );

                if (hit == null)
                {
                    await poker.WaitUntilReleased(cancel);

                    continue;
                }

                using ILicenseToPoke licenceToPoke = pokeArbitrator.GetLicense(hit.gameObject);

                var pokable = hit.gameObject.GetComponent<IPokable>();

                Vector2 worldPoint = camera.ScreenToWorldPoint(poker.Position);

                using IPokeSession pokeSession = pokable != null ? pokable.GetPokeSession() : new PickupCollider2DPokeSession(hit, worldPoint);

                var whizz = (pickedWhizz != null) ? pickedWhizz.Dispense() : null; 

                if (begin != null) begin.Invoke();               
                if (whizz != null) whizz.Begin(worldPoint);            

                while (cancel.IsCancellationRequested == false 
                    && poker.IsPressed 
                    && pokeSession.Disposed == false)
                {
                    await UniTask.NextFrame(PlayerLoopTiming.LastPostLateUpdate, cancel);

                    worldPoint = camera.ScreenToWorldPoint(poker.Position); 

                    pokeSession.Update(worldPoint);
                    if (whizz != null) whizz.During(worldPoint); 
                }

                if (end != null) end.Invoke();
                if (whizz != null) whizz.End(worldPoint);            
            }
        }
    }
}