using UnityEngine;

public class DiagnosticsTest : MonoBehaviour
{
    void Update()
    {
        // Press the 'K' key to trigger a test crash / exception
        if (Input.GetKeyDown(KeyCode.K))
        {
            TriggerTestCrash();
        }
    }

    private void TriggerTestCrash()
    {
        Debug.Log("[DiagnosticsTest] Intentionally throwing an exception...");
        
        // This causes an Unassigned / NullReferenceException
        string nullString = null;
        int length = nullString.Length; 
    }
}