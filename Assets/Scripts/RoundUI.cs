using BrunoMikoski.AnimationSequencer;
using UnityEngine;

public class RoundUI : MonoBehaviour
{
    [SerializeField] private Sequencer revealSequencer;
    [SerializeField] private Sequencer completeSequencer;

    public void Reveal()
    {
        revealSequencer.Play();
    }
    
    public void Complete()
    {
        completeSequencer.Play();
    }
}