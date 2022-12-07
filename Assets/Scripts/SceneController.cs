using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Bson;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private GameObject _floor;
    [SerializeField] private GameObject _heldContainer;
    [SerializeField] private Knob _knob;
    [SerializeField] private TextBox _textbox;
    [SerializeField] private TextBox _filetextbox;
    [SerializeField] private TextBox _textboxClosed;
    [SerializeField] private TextBox _questionbox;
    [SerializeField] private TextMeshPro _text;
    [SerializeField] private TextMeshPro _arrow;
    [SerializeField] private float _textShowWaitSeconds = 1f;
    [SerializeField] private float _textFillSeconds = .05f;
    [SerializeField] private Button _keepButton;
    [SerializeField] private Button _trashButton;
    [SerializeField] private GameObject _instructions;
    [SerializeField] private GameObject _pullInstructions;
    [SerializeField] private Light _spotlight;
    [SerializeField] private float _introWaitSeconds = 2f;
    [SerializeField] private float _introCameraAfterWaitSeconds = 1f;

    [SerializeField] private string[] _intro;
    [SerializeField] private string[] _introAfterCamera;
    [SerializeField] private string[] _ending;

    private Rigidbody _floorRigidbody;
    private bool _mouseDown;
    private Object _heldObject;
    private bool _objectHeld;
    private bool _textboxShown;
    private float _targetZ = -2.85f;
    private WaitForSeconds _textShowWait;
    private WaitForSeconds _textFillWait;
    private WaitForSeconds _introWait;
    private WaitForSeconds _introAfterCameraWait;
    private bool _textDone;
    private bool _textContinued;
    private bool _selectedChoice;
    private bool _instructionsShown;
    private Animation _cameraAnimation; 
    private Animation _lightAnimation;
    private bool _interactionEnabled;
    private bool _drawerPulled;

    private Object[] _objects;

    void Start()
    {
        if (_floor != null)
        {
            _floorRigidbody = _floor.GetComponent<Rigidbody>();
        }

        if (_knob != null)
        {
            _knob.OnKnobHeld += OnKnobHeld;
        }

        _objects = FindObjectsOfType<Object>();
        foreach (Object obj in _objects)
        {
            obj.OnObjectClicked += OnObjectClicked;
        }
        if (_textbox != null)
        {
            _textbox.gameObject.SetActive(value: false);
            _textbox.OnTextBoxClicked += OnTextBoxClicked;
        }
        if (_filetextbox != null)
        {
            _filetextbox.gameObject.SetActive(value: false);
            _filetextbox.OnTextBoxClicked += OnTextBoxClicked;
        }
        if (_questionbox != null)
        {
            _questionbox.gameObject.SetActive(value: false);
        }

        if (_textboxClosed != null)
        {
            _textboxClosed.gameObject.SetActive(value: false);
            _textboxClosed.OnTextBoxClicked += OnTextBoxClicked;
        }
        if (_keepButton != null)
        {
            _keepButton.gameObject.SetActive(value: false);
        }
        if (_trashButton != null)
        {
            _trashButton.gameObject.SetActive(value: false);
        }
        if (_instructions != null)
        {
            _instructions.SetActive(value: false);
        }
        if (_pullInstructions != null)
        {
            _pullInstructions.SetActive(value: false);
        }

        if (_spotlight != null && _spotlight.GetComponent<Animation>() is Animation anim1)
        {
            _lightAnimation = anim1;
        }
        if (_camera != null && _camera.GetComponent<Animation>() is Animation anim2)
        {
            _cameraAnimation = anim2;
        }

        _textShowWait = new WaitForSeconds(seconds: _textShowWaitSeconds);
        _textFillWait = new WaitForSeconds(seconds: _textFillSeconds);
        _introWait = new WaitForSeconds(seconds: _introWaitSeconds);
        _introAfterCameraWait = new WaitForSeconds(seconds: _introCameraAfterWaitSeconds);

        StartCoroutine(routine: StartIntroAsync());
    }

    public void Update()
    {
        if (!_interactionEnabled)
        {
            return;
        }
        
        if (_objectHeld)
        {
            if (_textboxShown) return;
            if (Input.GetMouseButton(0))
            {
                Transform t = _heldContainer.transform;
                float xMov = Input.GetAxis("Mouse X");
                float yMov = Input.GetAxis("Mouse Y");
                if ((t.rotation.z < _heldObject.RotationBoundsX[0] && xMov > 0) ||
                    (t.rotation.z > _heldObject.RotationBoundsX[1] && xMov < 0))
                {
                    xMov = 0;
                }

                if (t.rotation.x < _heldObject.RotationBoundsY[0] && yMov < 0 ||
                    t.rotation.x > _heldObject.RotationBoundsY[1] && yMov > 0)
                {
                    yMov = 0;
                }

                Vector3 rotationVector = new Vector3(yMov, 0, -xMov);
                t.Rotate(eulers: rotationVector * Time.deltaTime * _heldObject.RotateSpeed);
                return;
            }
        }

        // detect mouse press
        if (!_mouseDown && Input.GetMouseButtonDown(0))
        {
            _mouseDown = true;
        }
        else if (_mouseDown && Input.GetMouseButton(0))
        {
            float currentY = Input.mousePosition.y;
            float zConversion = (1 - (currentY / Screen.height)) * _targetZ;
            Debug.Log("current y is " + _floor.transform.position.z + " and z conversion is " + zConversion);

            if (zConversion > 0)
            {
                return;
            }
            if (_floor.transform.position.z < 0)
            {
                _drawerPulled = true;
                return;
            }

            Vector3 tempVect = new Vector3(0, 0, zConversion);
            tempVect = tempVect * (1.2f * Time.deltaTime);
            _floorRigidbody.MovePosition(_floor.transform.position + tempVect);
        }
        else if (_mouseDown && Input.GetMouseButtonUp(0))
        {
            _mouseDown = false;
        }
    }

    private void OnObjectClicked(Object obj)
    {
        if (_objectHeld) return;
        _heldObject = obj;
        _heldObject.transform.SetParent(p: _heldContainer.transform);
        _heldObject.SetHeld();
        _objectHeld = true;
        if (!_instructionsShown && _instructions != null)
        {
            _instructions.SetActive(value: true);
            _instructionsShown = true;
        }
        _textboxClosed.gameObject.SetActive(value: true);
    }
    
    private void OnTextBoxClicked(TextBox box)
    {
        Debug.Log(message: "Text box clicked: " + box.name);
        if (box.isClosed)
        {
            _instructions.SetActive(value: false);
            box.gameObject.SetActive(value: false);
            ShowText(text: _heldObject.Description, textBox: _filetextbox, showQuestionBox: true);
            return;
        }

        if (_textDone)
        {
            _textContinued = true;
        }
    }
    
    private void ShowText(string[] text, TextBox textBox, bool showQuestionBox)
    {
        StartCoroutine(routine: ShowTextAsync(text: text, textBox: textBox, showQuestionBox: showQuestionBox));
    }

    private IEnumerator ShowTextAsync(string[] text, TextBox textBox, bool showQuestionBox)
    {
        _textboxShown = true;
        yield return _textShowWait;
        textBox.gameObject.SetActive(value: true);
        
        foreach (var t in text)
        {
            textBox.text.text = "";
            textBox.arrow.gameObject.SetActive(value: false);
            _textDone = false;
            foreach (var c in t)
            {
                textBox.text.text += c;
                yield return _textFillSeconds;
            }

            textBox.arrow.gameObject.SetActive(value: true);
            _textDone = true;
            yield return new WaitUntil(() => _textContinued);
            _textContinued = false;
        }
        textBox.gameObject.SetActive(value: false);
        if (showQuestionBox)
        {
            ShowQuestionBox();
        }
        else
        {
            _textboxShown = false;
        }
    }

    private void ShowQuestionBox()
    {
        StartCoroutine(routine: ShowQuestionBoxAsync());
    }

    private IEnumerator ShowQuestionBoxAsync()
    {
        _questionbox.gameObject.SetActive(value: true);
        _keepButton.gameObject.SetActive(value: true);
        _trashButton.gameObject.SetActive(value: true);
        
        yield return new WaitUntil(() => _selectedChoice);
        
        _selectedChoice = false;
        _questionbox.gameObject.SetActive(value: false);
        _keepButton.gameObject.SetActive(value: false);
        _trashButton.gameObject.SetActive(value: false);
        
        _textboxShown = false;
    }

    public void onKeepClicked()
    {
        Debug.LogWarning(message: "Keep clicked");
        _heldObject.gameObject.transform.parent = null;
        _heldObject.SetUnheld();
        _heldObject.gameObject.SetActive(value: false);
        _heldObject = null;
        _objectHeld = false;
        _selectedChoice = true;
    }

    public void onTrashClicked()
    {
        Debug.LogWarning(message: "Trash clicked");
        _heldObject.SetUnheld();
        Destroy(obj: _heldObject.gameObject);
        _heldObject = null;
        _objectHeld = false;
        _selectedChoice = true;
    }

    private IEnumerator StartIntroAsync()
    {
        yield return _introWaitSeconds;
        yield return ShowTextAsync(text: _intro, textBox: _textbox, showQuestionBox: false);
        AnimateCamera();
        yield return new WaitUntil(() => !_cameraAnimation.isPlaying);
        _pullInstructions.SetActive(value: true);
        _interactionEnabled = true;
        yield return new WaitUntil(()=> _drawerPulled);
        _interactionEnabled = false;
        _pullInstructions.SetActive(value: false);
        yield return _introAfterCameraWait;
        yield return ShowTextAsync(text: _introAfterCamera, textBox: _textbox, showQuestionBox: false);
        _interactionEnabled = true;
    }

    private void AnimateCamera()
    {
        if (_cameraAnimation != null)
        {
            _cameraAnimation.Play();
        }

        if (_lightAnimation != null)
        {
            _lightAnimation.Play();
        }
    }
    
    private void OnKnobHeld()
    {
        Debug.LogWarning(message: "Knob held");
    }
}

