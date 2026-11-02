using UnityEngine;

public class MenuState : MonoBehaviour
{
    private Moving movingScript;
    void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            movingScript = player.GetComponent<Moving>();
            if (movingScript == null)
            {
                Debug.LogError("Moving script not found on the player object.");
            }
        }
        else
        {
            Debug.LogError("Player object with tag 'Player' not found.");
        }
    }
    void OnEnable()
    {
        if (movingScript != null)
        {
            movingScript.InputMode = InputType.Menu;
        }
    }
    void OnDisable()
    {
        if (movingScript != null)
        {
            movingScript.InputMode = InputType.Movement;
        }
    }
}
