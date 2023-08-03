using System;
using System.Buffers;
using Ossify;
using Ossify.Ballots;
using Sirenix.OdinInspector;
using UnityEngine;

// todo: move to Ossify

[CreateAssetMenu(menuName = "Poly Poke/Honker")]
public class Honker : CustodianBackbone
{
    [SerializeField] private Ballot enabled;  
    private bool dontCheckEnabled = false;

    [SerializeField] private Impulse impulse;

    [SerializeField] private AudioSource honkSource;
    
    [SerializeField] private SampleDistribution honkPicker;

    [SerializeField] private ScriptableCollection<AudioClip> clips;

    private AudioSource source;
    private CallMeOneTime.Call call;
    private SampleHistory history;

    /// <inheritdoc />
    protected override void Begin()
    {
        if (clips == null || clips.Count == 0) return;

        if (honkSource == null) return;
 
        if (impulse == null) impulse = CreateInstance<Impulse>();

        if (enabled == null) dontCheckEnabled = true;

        if (honkPicker == null) honkPicker = CreateInstance<UniformDistribution>();

        source = Instantiate(honkSource);
        source.name = $"{name} (Honk source)";

        call = CallMeOneTime.Get(Honk); 
        
        history = honkPicker.CreateHistory();

        impulse.Pulsed += ImpulseOnPulsed;
    }
    
    [Button]
    private void ImpulseOnPulsed() => CallMeOneTime.Enqueue(call);

    [Button]
    void Honk() 
    {
        if (dontCheckEnabled == false && enabled.Active == false) return;

        source.PlayOneShot(clips.Items[honkPicker.Distribute(clips.Count, history)]);
    }

    /// <inheritdoc />
    protected override void End()
    {
        impulse.Pulsed -= ImpulseOnPulsed;        
    }
}