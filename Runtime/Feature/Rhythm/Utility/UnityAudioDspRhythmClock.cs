using MyArchitecture.Core;
using UnityEngine;

namespace MyArchitecture.Feature.Rhythm
{
    public sealed class UnityAudioDspRhythmClock :
        Utility,
        IRhythmClock
    {
        public double DspTime => AudioSettings.dspTime;
    }
}
