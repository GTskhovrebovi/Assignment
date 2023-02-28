using BrunoMikoski.AnimationSequencer;
using UnityEngine;

public class RoundUI : MonoBehaviour
{
    [SerializeField] private AnimationSequence revealSequencer;
    [SerializeField] private AnimationSequence completeSequencer;

    public void Reveal()
    {
        revealSequencer.Play();
    }
    
    public void Complete()
    {
        completeSequencer.Play();
    }
}