using UnityEngine;

public class QuizTrigger : MonoBehaviour
{
    public GameObject quizUIPanel; // Assign in Inspector
    public float delay = 196f;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;
            StartCoroutine(ActivateQuizAfterDelay());
        }
    }

    private System.Collections.IEnumerator ActivateQuizAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        quizUIPanel.SetActive(true);
        //Time.timeScale = 0f; // Optional: pause the game
    }
}
