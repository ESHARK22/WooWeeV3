using UnityEngine;
using UnityEngine.CrashReportHandler;

public static class DiagnosticsConfig
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ConfigureDiagnostics()
    {
// Only enable chash reports when not editing in unity
#if UNITY_EDITOR
        CrashReportHandler.enableCaptureExceptions = false;
#else
        CrashReportHandler.enableCaptureExceptions = true;
#endif
    }
}