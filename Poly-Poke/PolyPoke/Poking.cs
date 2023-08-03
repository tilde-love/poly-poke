using System.Buffers;
using System.Collections.Generic;
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
        // [SerializeField] private StringReference debugText = new () { Value = "NO VALUE" };

        /// <inheritdoc />
        public override async UniTask PulseAsync(CancellationToken cancellationToken)
        {
            var camera = Camera.main;

            var pokers = ListPool<IPoker>.Get();
            var tasks = ListPool<UniTask>.Get();

            var log = GenericPool<StringBuilder>.Get(); 
            log.Clear(); 

            try 
            {
                foreach (InputDevice device in InputSystem.devices)
                {
                    switch (device)
                    {
                        case Touchscreen touchscreen:
                            foreach (TouchControl control in touchscreen.touches)
                            {
                                log.AppendLine($"Touch: {control.touchId}, {control.device.name}"); 
                                pokers.Add(new TouchPoker(control));
                            }

                            break;
                        case Mouse mouse:
                            log.AppendLine($"Mouse: {mouse.device.name}");
                            pokers.Add(new MousePoker(mouse));
                            break;
                        case Pen pen:
                            log.AppendLine($"Pen: {pen.device.name}");
                            pokers.Add(new PenPoker(pen));
                            break;
                    }
                }            

                Debug.Log(log.ToString());

                IPokeArbitrator pokeArbitrator = new PokeArbitrator();

                foreach (IPoker poker in pokers)
                {
                    tasks.Add(poke.PokeAsync(poker, pokeArbitrator, camera, cancellationToken));
                }

                await UniTask.WhenAll(tasks);
            }
            finally
            {
                ListPool<UniTask>.Release(tasks);
                ListPool<IPoker>.Release(pokers);

                log.Clear();
                GenericPool<StringBuilder>.Release(log);               
            }
        }
    }
}