using UnityEngine;
using UnityEngine.UI;

public class PostureBar : MonoBehaviour
{
    [SerializeField] private Image poisebarSprite;

    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }
    public void UpdatePostureBar(float maxPoise, float currentPoise)
    {
        poisebarSprite.fillAmount = (currentPoise / maxPoise);
    }
    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);
    }
}
