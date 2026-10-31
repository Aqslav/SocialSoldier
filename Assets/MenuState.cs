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
        Debug.Log("MenuState initialized.");
    }
    void OnEnable()
    {
        if (movingScript != null)
        {
            movingScript.InputMode = InputType.Menu;
            Debug.Log("MenuState enabled, switching to Menu mode.");
        }
    }
    void OnDisable()
    {
        if (movingScript != null)
        {
            movingScript.InputMode = InputType.Movement;
            Debug.Log("MenuState disabled, returning to Movement mode.");
        }
    }
}
