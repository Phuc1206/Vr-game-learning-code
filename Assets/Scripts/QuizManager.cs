using UnityEngine;
using UnityEngine.SceneManagement;

public class QuizManager : MonoBehaviour
{
    public GameObject[] questionCanvases; // Array of question canvases
    public GameObject settingCanvas; // The settings menu canvas

    private int currentQuestionIndex = 0;

    void Start()
    {
        ShowQuestion(currentQuestionIndex);
    }

    public void CheckAnswer(bool isCorrect)
    {
        if (isCorrect)
        {
            currentQuestionIndex++;
            if (currentQuestionIndex < questionCanvases.Length)
            {
                ShowQuestion(currentQuestionIndex);
            }
            else
            {
                Debug.Log("Quiz Completed! Loading next scene...");
                LoadNextScene(); // Load scene with index 2 if all questions are answered correctly
            }
        }
        else
        {
            ShowSettingMenu();
        }
    }

    void ShowQuestion(int index)
    {
        for (int i = 0; i < questionCanvases.Length; i++)
        {
            questionCanvases[i].SetActive(i == index);
        }
        settingCanvas.SetActive(false);
    }

    void ShowSettingMenu()
    {
        foreach (var canvas in questionCanvases)
        {
            canvas.SetActive(false);
        }
        settingCanvas.SetActive(true);
    }

    public void Replay()
    {
        currentQuestionIndex = 0;
        ShowQuestion(currentQuestionIndex);
    }

    public void ExitToStartScene()
    {
        Debug.Log("Exiting to Start Scene"); // Debug log to ensure method is called
        SceneManager.LoadScene(0); // Using index 0 for Start Scene
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(2); // Using index 2 for the next scene
    }
}
