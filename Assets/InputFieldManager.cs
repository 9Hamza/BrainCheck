using TMPro;
using UnityEngine;

public class InputFieldManager : MonoBehaviour
{

    [SerializeField] private TMP_InputField inputField;

    // Update is called once per frame
    void Update()
    {
        Debug.Log($"text: {inputField.text}");
    }
}
