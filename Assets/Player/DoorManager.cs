using UnityEngine;

public class DoorManager : MonoBehaviour
{
    public doorSafe[] doors;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
     foreach (doorSafe d in doors)
        {
            d.isSafeDoor = false;

        }   

        int safeDoor = Random.Range(0,doors.Length);
        doors[safeDoor].isSafeDoor = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
