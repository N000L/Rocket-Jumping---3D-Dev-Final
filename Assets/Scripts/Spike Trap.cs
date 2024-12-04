using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    public float moveDistance = 5f;
    public float moveSpeed = 2f;
    public float pauseTime = 1f;

    private Vector3 originalPosition;

    // Start is called before the first frame update
    void Start()
    {
        // Save the OG position
        originalPosition = transform.position;

        // Start coroutine that handles movement

        StartCoroutine(MoveObject());
    }


    IEnumerator MoveObject()
    {
        while (true) // Loop it
        {
            // Move up
            yield return StartCoroutine(MoveVertically(originalPosition + Vector3.up * moveDistance));

            // Pause
            yield return new WaitForSeconds(pauseTime);

            // Move up again
            yield return StartCoroutine(MoveVertically(originalPosition + Vector3.up * moveDistance * 2));

            // Pause again
            yield return new WaitForSeconds(pauseTime);

            // Return back to original position
            yield return StartCoroutine(MoveVertically(originalPosition));

            // Pause one final time
            yield return new WaitForSeconds(pauseTime);
        } 

        
    }

    IEnumerator MoveVertically(Vector3 targetPosition)
    {
        float journeyLength = Vector3.Distance(transform.position, targetPosition);
        float startTime = Time.time;

        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            float distanceCovered = (Time.time - startTime) * moveSpeed;
            float fractionOfJourney = distanceCovered / journeyLength;

            transform.position = Vector3.Lerp(transform.position, targetPosition, fractionOfJourney);
            yield return null;
        }

        //Ensure object reaches target position
        transform.position = targetPosition;
    }
}