using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuizManager : MonoBehaviour
{
    // Toggle references
    public Toggle q1TrueToggle;
    public Toggle q2FocusToggle;
    public Toggle q3a, q3b, q3c;

    // TMP Text for results
    public GameObject resultTextObject;
    public TextMeshProUGUI resultText;

    public void SubmitQuiz()
    {
        bool isCorrect = true;

        // Question 1: True
        isCorrect &= q1TrueToggle.isOn;

        // Question 2: c) Focus
        isCorrect &= q2FocusToggle.isOn;

        // Question 3: a & b are correct, c is incorrect
        isCorrect &= q3a.isOn && q3b.isOn && !q3c.isOn;

        resultText.text = isCorrect ? "All answers correct!" : "Some answers are incorrect. Try again!";
        resultTextObject.SetActive(true);
    }

    public void CloseQuiz()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f; // Resume game if paused
    }
}
