using Ossify;
using Ossify.Ballots;
using Ossify.Variables;
using Sirenix.OdinInspector;
using UnityEngine;
// todo: move to Ossify
//
// [CreateAssetMenu(menuName = "Poly Poke/Blower")]
// public class Blower : CustodianBackbone
// {
//     [SerializeField] private Ballot enabled;  
//     private bool dontCheckEnabled = false;
//
//     [SerializeField] private FloatReference intensity = new () { Value = 0f };
//
//     [SerializeField] private AudioSource honkSource;
//     
//     [SerializeField] private SampleDistribution honkPicker;
//
//     [SerializeField] private ScriptableCollection<AudioClip> clips;
//
//     private AudioSource[] sources;
//     private CallMeOneTime.Call call;
//     private int[] history;
//     private int historyIndex;
//
//     /// <inheritdoc />
//     protected override void Begin()
//     {
//         if (clips == null || clips.Count == 0) return;
//
//         if (honkSource == null) return;
//
//         if (enabled == null) dontCheckEnabled = true;
//
//         if (honkPicker == null) honkPicker = CreateInstance<RandomDistribution>();
//
//         source = Instantiate(honkSource);
//         source.name = $"{name} (Honk source)";
//
//         call = CallMeOneTime.Get(Honk); 
//         
//         history = new int [honkPicker.HistorySize];
//
//         impulse.Pulsed += ImpulseOnPulsed;
//     }
//
//     private void ImpulseOnPulsed() => CallMeOneTime.Enqueue(call);
//
//     [Button]
//     void Honk() 
//     {
//         if (dontCheckEnabled == false && enabled.Active == false) return;
//
//         source.PlayOneShot(clips.Items[honkPicker.Pick(clips.Count, history, ref historyIndex)]);
//     }
//
//     /// <inheritdoc />
//     protected override void End()
//     {
//         impulse.Pulsed -= ImpulseOnPulsed;        
//     }
// }