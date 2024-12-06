using UnityEngine;
using TMPro;
using System.Collections;

public class CountdownTimer : MonoBehaviour
{
    public Transform vrCameraTransform; // Tham chiếu đến transform của camera VR
    public float countdownTime = 60f; // Thời gian đếm ngược tính bằng giây
    public TextMeshProUGUI textMesh; // Tham chiếu đến TextMeshProUGUI cho Text UI
    public float heightOffset = 0.1f; // Giá trị điều chỉnh độ cao
    private PuzzleManager puzzleManager; // Tham chiếu đến PuzzleManager

    private float remainingTime; // Thời gian còn lại

    private void Start()
    {
        puzzleManager = FindObjectOfType<PuzzleManager>();
        remainingTime = countdownTime;
        StartCoroutine(Countdown());
    }

    private void Update()
    {
        // Cập nhật vị trí của đối tượng này để luôn theo sau camera VR
        Vector3 newPosition = vrCameraTransform.position + vrCameraTransform.forward * 2.0f;
        newPosition.y += heightOffset; // Điều chỉnh vị trí y để văn bản cao hơn
        transform.position = newPosition;

        // Quay đối tượng này để hướng về camera VR
        Vector3 direction = vrCameraTransform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = rotation * Quaternion.Euler(0, 90, 0); // Điều chỉnh trục Y nếu cần thiết
    }

    private IEnumerator Countdown()
    {
        while (remainingTime > 0)
        {
            textMesh.text = remainingTime.ToString("F1"); // Cập nhật TextMeshPro với thời gian còn lại
            yield return new WaitForSeconds(1f); // Chờ 1 giây
            remainingTime -= 1f; // Giảm thời gian còn lại
        }
        textMesh.text = "0";
        puzzleManager.OnCountdownEnd(); // Gọi hàm khi đếm ngược kết thúc
    }
}
