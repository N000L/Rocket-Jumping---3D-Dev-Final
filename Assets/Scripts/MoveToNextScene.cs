using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveToNextScene : MonoBehaviour
{
    [Header("Scene Load Settings")]
    [SerializeField] private SceneAsset nextScene; // Drag the scene into this field in the inspector.
    public float delayBeforeLoad = 2f; // Time to wait before loading the next scene.

    private bool isTriggered = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isTriggered)
        {
            isTriggered = true;
            StopPlayerMovement(collision.gameObject);
            StartCoroutine(LoadNextSceneAfterDelay());
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
            SceneManager.LoadScene(nextScene.name);
        }
        else
        {
            Debug.LogError("Next Scene reference is missing! Drag a scene into the inspector.");
        }
    }
}
