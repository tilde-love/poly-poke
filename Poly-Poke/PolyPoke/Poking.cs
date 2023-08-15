using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Ossify;
using Ossify.Variables;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Pool;

namespace PolyPoke
{
    [CreateAssetMenu(menuName = "Poly Poke/Poking Feature")]
    public class Poking : AsyncBackbone
    {
        [SerializeField] private Poke poke;

        /// <inheritdoc />
        public override async UniTask PulseAsync(CancellationToken cancellationToken)
        {
            var camera = Camera.main;

            var pokers = ListPool<IPoker>.Get();

            try 
            {
                foreach (InputDevice device in InputSystem.devices)
                {
                    switch (device)
                    {
                        case Touchscreen touchscreen:
                            pokers.AddRange(touchscreen.touches.Select(control => new TouchPoker(control)));
                            break;
                        case Mouse mouse:                            
                            pokers.Add(new MousePoker(mouse));
                            break;
                        case Pen pen:                            
                            pokers.Add(new PenPoker(pen));
                            break;
                    }
                }            

                using IPokeArbitrator pokeArbitrator = new PokeArbitrator();

                await UniTask.WhenAll(Enumerable.Select(pokers, poker => poke.PokeAsync(poker, pokeArbitrator, camera, cancellationToken)));
            }
            finally
            {
                ListPool<IPoker>.Release(pokers);            
            }
        }
    }
}