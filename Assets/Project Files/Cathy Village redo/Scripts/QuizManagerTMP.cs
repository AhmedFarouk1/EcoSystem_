using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class QuizManagerTMP : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text questionText;
    public Button[] choiceButtons;
    public TMP_Text endText;
    public Button nextQuizButton;
    public Button exitButton;
    public GameObject quizPanel; // The main panel to hide when done

    [Header("Quiz 1 Data")]
    public Question[] quiz1Questions;

    [Header("Quiz 2 Options")]
    public string[] storySettings;
    public string[] storyCharacters;
    public string[] storyMissions;

    private int currentQuestionIndex = 0;
    private int quizStage = 1;
    private Dictionary<string, int> typeCounts = new Dictionary<string, int>();

    private int storyStep = 0;
    private string chosenSetting, chosenCharacter, chosenMission;

    private Question[] currentQuiz;

    private void Start()
    {
        nextQuizButton.gameObject.SetActive(false);
        endText.gameObject.SetActive(false);
        currentQuiz = quiz1Questions;
        ShowQuestion();
    }

    void ShowQuestion()
    {
        questionText.gameObject.SetActive(true);
        endText.gameObject.SetActive(false);

        if (quizStage == 1)
        {
            if (currentQuestionIndex < currentQuiz.Length)
            {
                Question q = currentQuiz[currentQuestionIndex];
                questionText.text = q.questionText;

                for (int i = 0; i < choiceButtons.Length; i++)
                {
                    int index = i;
                    TMP_Text btnText = choiceButtons[i].GetComponentInChildren<TMP_Text>();
                    if (btnText != null) btnText.text = q.choices[i];

                    choiceButtons[i].onClick.RemoveAllListeners();
                    choiceButtons[i].onClick.AddListener(() => OnAnswerSelected(index));
                }
            }
            else
            {
                EndQuiz1();
            }
        }
        else if (quizStage == 2)
        {
            ShowStoryBuilderStep();
        }
    }

    void OnAnswerSelected(int index)
    {
        if (quizStage == 1)
        {
            string selectedType = currentQuiz[currentQuestionIndex].choiceTypes[index];
            if (typeCounts.ContainsKey(selectedType))
                typeCounts[selectedType]++;
            else
                typeCounts[selectedType] = 1;

            currentQuestionIndex++;
            ShowQuestion();
        }
        else if (quizStage == 2)
        {
            if (storyStep == 0)
                chosenSetting = storySettings[index];
            else if (storyStep == 1)
                chosenCharacter = storyCharacters[index];
            else if (storyStep == 2)
                chosenMission = storyMissions[index];

            storyStep++;

            if (storyStep < 3)
                ShowStoryBuilderStep();
            else
                ShowStoryResult();
        }
    }

    void EndQuiz1()
    {
        questionText.gameObject.SetActive(false);
        foreach (Button btn in choiceButtons)
            btn.gameObject.SetActive(false);

        endText.gameObject.SetActive(true);

        string resultType = null;
        int maxCount = 0;
        foreach (var pair in typeCounts)
        {
            if (pair.Value > maxCount)
            {
                maxCount = pair.Value;
                resultType = pair.Key;
            }
        }

        string resultMessage = GetResultMessage(resultType);
        endText.text = $"Quiz 1 Result:\n\n{resultMessage}";

        nextQuizButton.gameObject.SetActive(true);
        nextQuizButton.onClick.RemoveAllListeners();
        nextQuizButton.onClick.AddListener(StartStoryBuilder);
    }

    void StartStoryBuilder()
    {
        quizStage = 2;
        storyStep = 0;
        nextQuizButton.gameObject.SetActive(false);
        endText.gameObject.SetActive(false);

        foreach (Button btn in choiceButtons)
            btn.gameObject.SetActive(true);

        ShowStoryBuilderStep();
    }

    void ShowStoryBuilderStep()
    {
        string[] options = null;
        string question = "";

        if (storyStep == 0)
        {
            options = storySettings;
            question = "Pick a Setting:";
        }
        else if (storyStep == 1)
        {
            options = storyCharacters;
            question = "Pick a Character:";
        }
        else if (storyStep == 2)
        {
            options = storyMissions;
            question = "Pick a Mission:";
        }

        questionText.text = question;

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            TMP_Text btnText = choiceButtons[i].GetComponentInChildren<TMP_Text>();
            if (i < options.Length)
            {
                choiceButtons[i].gameObject.SetActive(true);
                btnText.text = options[i];
                int index = i;
                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() => OnAnswerSelected(index));
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
    }

    void ShowStoryResult()
    {
        questionText.gameObject.SetActive(false);
        foreach (Button btn in choiceButtons)
            btn.gameObject.SetActive(false);

        endText.gameObject.SetActive(true);
        endText.text = $"You are a {chosenCharacter} in the {chosenSetting}.\nYour mission: {chosenMission}!\n\nYour adventure begins...";

        exitButton.gameObject.SetActive(true);
        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener(() =>
        {
            quizPanel.SetActive(false);
        });

    }

    string GetResultMessage(string resultType)
    {
        switch (resultType)
        {
            case "A":
                return "Nature Nomad:\nGo barefoot in the grass, watch a sunset, go on a tech-free hike.";
            case "B":
                return "Homebody Zen:\nRun a bath, meditate, nap, read a book.";
            case "C":
                return "Sociable Relaxer:\nGo out for dinner, take a tech-free holiday, chat without screens.";
            default:
                return "Couldn't determine a result.";
        }
    }
}

[System.Serializable]
public class Question
{
    public string questionText;
    public string[] choices;
    public string[] choiceTypes;
}
