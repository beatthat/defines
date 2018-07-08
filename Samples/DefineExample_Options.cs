using UnityEngine;

namespace BeatThat.Defines.Samples
{
    [EditDefine(new string[] {
        "SAMPLES_ENV_PRODUCTION",
        "SAMPLES_ENV_DEV",
        "SAMPLES_ENV_DEVLOCAL"
    }, @"Example of options for toggling the host in a server-driven app.
Delete Assets/Samples/packages/beatthat/defines/DefinesExample.cs to stop seeing this.")]
    public class DefineExample_Options : MonoBehaviour
    {
        public const string HOST =
#if SAMPLES_ENV_DEVLOCAL
        "http://localhost:3001";
#elif SAMPLES_ENV_DEV
        "http://dev.mydomain.com";
#else
        "http://mydomain.com";
#endif

    }
}
