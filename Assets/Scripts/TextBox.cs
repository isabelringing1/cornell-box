using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextBox : MonoBehaviour
{
    [SerializeField] public bool isClosed;
    [SerializeField] public bool isFile;
    [SerializeField] public TextMeshPro text;
    [SerializeField] public TextMeshPro arrow;
    
    public delegate void TextBoxClickedDelegate(TextBox textBox);
    public TextBoxClickedDelegate OnTextBoxClicked;

    private void OnMouseDown()
    {
        OnTextBoxClicked?.Invoke(this);
    }
}
