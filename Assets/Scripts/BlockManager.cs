using UnityEngine;
using System.Collections;
public class Block : MonoBehaviour
{
    public int blockID;  // ID của khối hộp
    public bool isInPlace = false; // Trạng thái của khối hộp
    private PuzzleManager puzzleManager;

    private void Start()
    {
        puzzleManager = FindObjectOfType<PuzzleManager>();
    }

    private void OnMouseDown()
    {
        if (puzzleManager.timeIsUp)
        {
            return; // Không cho phép tương tác nếu thời gian đã hết
        }

        // Logic để di chuyển khối
        // ...
    }

    private void OnTriggerEnter(Collider other)
    {
        if (puzzleManager.timeIsUp)
        {
            return; // Không cho phép khối được đặt vào chỗ nếu thời gian đã hết
        }

        Slot slot = other.GetComponent<Slot>();
        if (slot != null && slot.slotID == blockID)
        {
            // Khối hộp này khớp với vị trí trống
            Debug.Log("Khối hộp đã được đặt đúng chỗ!");
            // Cố định khối hộp vào vị trí
            this.transform.position = slot.transform.position;
            this.transform.rotation = slot.transform.rotation;
            this.GetComponent<Rigidbody>().isKinematic = true;

            // Đánh dấu khối hộp đã đúng vị trí
            isInPlace = true;
            StartCoroutine(ShowQuestionWithDelay());
    }
}
private IEnumerator ShowQuestionWithDelay()
    {
        yield return new WaitForSeconds(3f); // Wait for 3 seconds
        puzzleManager.ShowQuestion(blockID);
    }
}
