using UnityEngine;

// Animates the trigger from the pressed to the released position.
public class AnimateTrigger : MonoBehaviour
{
    [SerializeField]
    private Transform trigger;

    [SerializeField]
    private Transform triggerPressed;

    private Vector3 releasedPosition;
    private Vector3 pressedPosition;

    private void Start()
    {
        releasedPosition = trigger.localPosition;
        pressedPosition = triggerPressed.localPosition;
    }

    // Moves the trigger to the pressed position.
    public void PressTrigger()
    {
        trigger.localPosition = pressedPosition;
    }

    // Moves the trigger to the released position.
    public void ReleaseTrigger()
    {
        trigger.localPosition = releasedPosition;
    }
}
