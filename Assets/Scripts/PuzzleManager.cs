using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System.Linq;

[System.Serializable]
public class Question
{
    public string questionText;
    public string[] answers;
    public int correctAnswerIndex;
}

[System.Serializable]
public class BlockCanvasMapping
{
    public int blockID;
    public GameObject canvas;
    public List<Question> questions;
    public Text questionTextUI;
    public Button[] answerButtonsUI;
    public TextMeshProUGUI questionProgressTextUI;
}

public class PuzzleManager : MonoBehaviour
{
    public Block[] blocks;
    private bool sceneLoading = false;
    public GameObject levelUpCube;
    public Transform vrCameraTransform;
    private Vector3 initialCameraPosition;
    private Quaternion initialCameraRotation;
    public GameObject gameOverMenu;
    public GameObject questionMenu;
    public int restartSceneIndex = 2;
    public int nextScene = 3;
    public bool timeIsUp = false;
    public BlockCanvasMapping[] blockCanvasMappings;
    private int currentQuestionIndex = 0;
    private int totalCorrectAnswer = 0;
    private int correctAnswerCount = 0; // Added to keep track of correct answers
    private int currentBlockID = -1;
    public TextMeshProUGUI resultTextUI;
    private int ShowResultsCount = 0;

    void Start()
    {
        initialCameraPosition = vrCameraTransform.position;
        initialCameraRotation = vrCameraTransform.rotation;
        if (resultTextUI != null)
        {
            resultTextUI.gameObject.SetActive(false);
        }
        
        foreach (BlockCanvasMapping mapping in blockCanvasMappings)
        {
            if (mapping.canvas != null)
            {
                mapping.canvas.SetActive(false);
            }
        }
    }

    void Update()
    {
        if (!timeIsUp && AllBlocksInPlace() && !sceneLoading && ShowResultsCount == 4)
        {
            sceneLoading = true;
            StartCoroutine(ReturnCameraAndShowLevelUp());
        }
    }

    private bool AllBlocksInPlace()
    {
        foreach (Block block in blocks)
        {
            if (!block.isInPlace)
            {
                return false;
            }
        }
        return totalCorrectAnswer >= 10; 
    }

    public void OnCountdownEnd()
    {
        timeIsUp = true; // Đặt cờ báo hết thời gian
        

        if (AllBlocksInPlace())
        {
            if(totalCorrectAnswer >= 10){
                StartCoroutine(ReturnCameraAndShowLevelUp());
            }
            else{
                ShowGameOverMenu();
            }
        }
        else
        {
            ShowGameOverMenu();
        }
    }

    private IEnumerator ReturnCameraAndShowLevelUp()
    {
        vrCameraTransform.position = initialCameraPosition;
        vrCameraTransform.rotation = initialCameraRotation;

        yield return new WaitForSeconds(0.5f);

        ShowLevelUpCube();

        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene(nextScene);
    }

    private void ShowLevelUpCube()
    {
        PositionCubeInFrontOfPlayer();
        levelUpCube.SetActive(true);
    }

    private void PositionCubeInFrontOfPlayer()
    {
        Vector3 newPosition = vrCameraTransform.position + vrCameraTransform.forward * 2.0f;
        levelUpCube.transform.position = newPosition;
    }

    public void ShowQuestion(int id)
{
    currentBlockID = id;
    if (resultTextUI != null)
    {
        resultTextUI.gameObject.SetActive(false);
    }
    
    foreach (BlockCanvasMapping mapping in blockCanvasMappings)
    {
        if (mapping.blockID == id)
        {
            if (mapping.questions == null || mapping.questions.Count == 0)
            {
                Debug.LogError("Questions list is null or empty.");
                return;
            }

            // Đảm bảo chỉ số câu hỏi hiện tại không vượt quá số lượng câu hỏi
            if (currentQuestionIndex >= mapping.questions.Count)
            {
                currentQuestionIndex = 0; // Reset chỉ số câu hỏi nếu vượt quá
            }

            Question currentQuestion = mapping.questions[currentQuestionIndex];
            if (mapping.questionTextUI == null)
            {
                Debug.LogError("Question Text UI is not assigned.");
                return;
            }

            mapping.questionTextUI.text = currentQuestion.questionText;

            // Cập nhật số câu hỏi đã trả lời trước khi gán listeners mới
            if (mapping.questionProgressTextUI != null)
            {
                mapping.questionProgressTextUI.text = "Question " + (currentQuestionIndex + 1) + "/" + mapping.questions.Count;
            }
            else
            {
                Debug.LogError("Question Progress Text UI is not assigned.");
            }

            // Xóa các listeners cũ trước khi gán listeners mới
            foreach (Button answerButton in mapping.answerButtonsUI)
            {
                answerButton.onClick.RemoveAllListeners();
                answerButton.gameObject.SetActive(true);
            }

            for (int i = 0; i < currentQuestion.answers.Length; i++)
            {
                int answerIndex = i; // Capture the current index in a local variable for the lambda
                if (mapping.answerButtonsUI[i] != null)
                {
                    mapping.answerButtonsUI[i].onClick.AddListener(() => OnAnswerSelected(answerIndex));
                    TMPro.TextMeshProUGUI answerText = mapping.answerButtonsUI[i].GetComponentInChildren<TMPro.TextMeshProUGUI>();
                    if (answerText != null)
                    {
                        answerText.text = currentQuestion.answers[i];
                        
                    }
                    else
                    {
                        Debug.LogError("Answer Text component not found in Button at index " + i);
                    }
                }
                else
                {
                    Debug.LogError("Answer Button UI at index " + i + " is not assigned.");
                }
            }

            if (mapping.canvas == null)
            {
                Debug.LogError("Canvas is not assigned.");
                return;
            }

            mapping.canvas.SetActive(true);
            mapping.questionTextUI.gameObject.SetActive(true);
            // Không tăng currentQuestionIndex ở đây để hiển thị câu hỏi chính xác
            return; // Kết thúc việc hiển thị câu hỏi
        }
    }
}




    private IEnumerator HideCanvasAfterDelay(GameObject canvas, float delay)
    {
        yield return new WaitForSeconds(delay);

        // Kiểm tra null tránh lỗi
        if (canvas != null)
        {
            canvas.SetActive(false);
        }
    }

    private void ShowResults()
    {
        BlockCanvasMapping currentMapping = blockCanvasMappings.FirstOrDefault(mapping => mapping.blockID == currentBlockID);
        if (currentMapping != null)
        {
            int totalQuestions = currentMapping.questions.Count;
            string resultText = correctAnswerCount + "/" + totalQuestions;

            if (resultTextUI != null)
            {
                resultTextUI.gameObject.SetActive(true);
                resultTextUI.text = "You answered correctly " + resultText + " questions.";
                StartCoroutine(HideCanvasAfterDelay(currentMapping.canvas, 3f));
                ShowResultsCount++;
            }
            else
            {
                Debug.LogError("Result Text UI is not assigned.");
            }

            correctAnswerCount = 0;

            HideQuestionComponents(currentMapping);
        }
        else
        {
            Debug.LogError("Current Block Mapping is not found.");
        }
    }
    private void HideQuestionComponents(BlockCanvasMapping mapping)
    {
        // Ẩn các thành phần câu hỏi trong mapping
        if (mapping.questionTextUI != null)
        {
            mapping.questionTextUI.gameObject.SetActive(false);
        }
        if (mapping.questionProgressTextUI != null)
        {
            mapping.questionProgressTextUI.gameObject.SetActive(false);
        }
        foreach (Button button in mapping.answerButtonsUI)
        {
            if (button != null)
            {
                button.gameObject.SetActive(false);
            }
        }
    }

    public void OnAnswerSelected(int selectedAnswerIndex)
    {   Debug.LogError(selectedAnswerIndex);
        BlockCanvasMapping currentMapping = blockCanvasMappings.FirstOrDefault(mapping => mapping.blockID == currentBlockID);
        if (currentMapping != null)
        {
            Question currentQuestion = currentMapping.questions[currentQuestionIndex];
            if (selectedAnswerIndex == currentQuestion.correctAnswerIndex)
            {
                correctAnswerCount++;
                Debug.Log("Correct Answer");
                totalCorrectAnswer++;
            }

            // Move to the next question or show results if it was the last question
            currentQuestionIndex++;
            if (currentQuestionIndex < currentMapping.questions.Count)
            {
                ShowQuestion(currentBlockID); // Show next question
            }
            else
            {   
                
                ShowResults(); // Show results if it was the last question
                currentQuestionIndex = 0; // Reset for next round
            }
        }
    }

    private void ShowGameOverMenu()
    {
        gameOverMenu.SetActive(true); // Hiển thị menu Game Over
    }

    public void ExitGame()
    {
        SceneManager.LoadScene(0); // Load scene 0
    }

    public void ReplayGame()
    {
        SceneManager.LoadScene(restartSceneIndex);
    }
}
