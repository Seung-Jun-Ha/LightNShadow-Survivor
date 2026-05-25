using UnityEngine;
using UnityEngine.InputSystem;

public class StageManager : MonoBehaviour
{
    public int totalStages = 3;
    private int currentStage = 0;

    public TimeOfDayManager timeOfDayManager;

    public void CompleteStage()
    {
        currentStage++;
        Debug.Log($"Stage {currentStage} cleared!");

        if (currentStage >= totalStages)
        {
            if (timeOfDayManager != null)
            {
                timeOfDayManager.ClearAllStages();
            }
            Debug.Log("All stages cleared! Transitioning to morning...");
        }
    }

    // For testing
    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.lKey.wasPressedThisFrame)
        {
            CompleteStage();
        }
    }
}
