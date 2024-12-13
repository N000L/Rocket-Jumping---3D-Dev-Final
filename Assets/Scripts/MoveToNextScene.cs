using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveToNextScene : MonoBehaviour
{
    [Header("Scene Load Settings")]
    [SerializeField] private Scene nextScene; // Drag the scene into this field in the inspector.
    public float delayBeforeLoad = 2f; // Time to wait before loading the next scene.

    private bool isTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
        isTriggered = true;
        StopPlayerMovement(other.gameObject);
        StartCoroutine(LoadNextSceneAfterDelay());
        Debug.Log(isTriggered);
        }

    }

    private void StopPlayerMovement(GameObject player)
    {
        var playerMovement = player.GetComponent<PlayerControllerScript>(); // Insert the name of your player movement script in the < >
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }
    }

    private IEnumerator LoadNextSceneAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeLoad);

        if (nextScene != null)
        {
            SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            Debug.LogError("Next Scene reference is missing! Drag a scene into the inspector.");
        }
    }
}
