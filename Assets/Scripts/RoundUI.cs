using BrunoMikoski.AnimationSequencer;
using UnityEngine;

public class RoundUI : MonoBehaviour
{
    [SerializeField] private AnimationSequencerController revealSequencer;
    [SerializeField] private AnimationSequencerController completeSequencer;

    public void Reveal()
    {
        revealSequencer.Play();
    }
    
    public void Complete()
    {
        completeSequencer.Play();
    }
}