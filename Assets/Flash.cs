using TMPro;
using UnityEngine;

public class Flash : MonoBehaviour
{
    public TextMeshProUGUI text;

    // Update is called once per frame
    void Update()
    {
        var color = text.color;
        color.a = Mathf.Sin(Time.realtimeSinceStartup * BlockGameConstants.TitleScreen.FlashSpeed) + BlockGameConstants.TitleScreen.AlphaModifier;
        text.color = color;
    }
}
