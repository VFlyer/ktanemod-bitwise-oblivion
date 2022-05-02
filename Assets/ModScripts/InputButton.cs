using UnityEngine;

public class InputButton : MonoBehaviour
{
    void Start()
    {
        var module = transform.parent.parent.parent.parent.GetComponent<qkBitwiseOblivion>();
        var text = GetComponentInChildren<TextMesh>().text;
        var selectable = GetComponent<KMSelectable>();
        module.InputButtons.Add(text[0], selectable);
        selectable.OnInteract += () =>
        {
            module.Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
            if(module.InputText.text.Length < 8)
                module.InputText.text += text;
            return false;
        };
    }
}