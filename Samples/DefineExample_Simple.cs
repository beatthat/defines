using UnityEngine;

namespace BeatThat.Defines.Samples
{
    [EditDefine("SAMPLES_DEFINE", @"A simple define that can be enabled/disabled from Define Scripting Symbols window. 
Delete Assets/Samples/packages/beatthat/defines/DefinesExample.cs to stop seeing this.")]
    public class DefineExample_Simple : MonoBehaviour
    {
        void Start()
        {
#if SAMPLES_DEFINE
            // some custom behavior
            Debug.Log("SAMPLE_DEFINE is defined");
#endif
        }
    }
}
