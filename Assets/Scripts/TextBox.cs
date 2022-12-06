using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextBox : MonoBehaviour
{
    [SerializeField] public bool isClosed;
    
    public delegate void TextBoxClickedDelegate(TextBox textBox);
    public TextBoxClickedDelegate OnTextBoxClicked;

    private void OnMouseDown()
    {
        OnTextBoxClicked?.Invoke(this);
    }
}
