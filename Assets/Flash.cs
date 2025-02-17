using TMPro;
using UnityEngine;

public class Flash : MonoBehaviour
{
    public float speed = 3f;
    public TextMeshProUGUI text;

    // Update is called once per frame
    void Update()
    {
        var color = text.color;
        color.a = Mathf.Sin(Time.realtimeSinceStartup * speed) + 0.7f;
        text.color = color;
    }
}
