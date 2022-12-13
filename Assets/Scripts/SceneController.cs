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
    [SerializeField] private GameObject _drawer;
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
    [SerializeField] private AudioClip _selectClip;
    [SerializeField] private AudioClip _keepClip;
    [SerializeField] private AudioClip _trashClip;
    [SerializeField] private AudioClip _nextClip;
    [SerializeField] private GameObject _black;
    [SerializeField] private GameObject _blackText;
    [SerializeField] private GameObject _suitcase;
    [SerializeField] private GameObject _fill;
    [SerializeField] private string _alternateTrashText = "I can only <color=#8F0000>trash</color> this.";

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
    private WaitForSeconds _rotationWait;
    private bool _textDone;
    private bool _textContinued;
    private bool _selectedChoice;
    private bool _instructionsShown;
    private Animation _cameraAnimation; 
    private Animation _lightAnimation;
    private Animation _deskAnimation;
    private bool _interactionEnabled;
    private bool _drawerPulled;
    private AudioSource _audioSource;
    private Coroutine _rotateCoroutine;
    private int _fullValue = 200;
    private bool _suitcaseFull;
    private int _objectsDone;

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

        if (_drawer != null && _drawer.GetComponent<Animation>() is Animation anim3)
        {
            _deskAnimation = anim3;
        }
        if (_suitcase != null)
        {
            _suitcase.SetActive(value: false);
        }
        
        _textShowWait = new WaitForSeconds(seconds: _textShowWaitSeconds);
        _textFillWait = new WaitForSeconds(seconds: _textFillSeconds);
        _introWait = new WaitForSeconds(seconds: _introWaitSeconds);
        _introAfterCameraWait = new WaitForSeconds(seconds: _introCameraAfterWaitSeconds);
        _rotationWait = new WaitForSeconds(seconds: .1f);
        _audioSource = GetComponent<AudioSource>();

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

                Vector3 rotationVector = new Vector3(yMov, 0, -xMov);
                t.Rotate(eulers: rotationVector * Time.deltaTime * _heldObject.RotateSpeed);
                Debug.LogWarning(message: "Rotating " + _heldObject.name);
                if (t.rotation.z < _heldObject.RotationBoundsX[0])
                {
                    Quaternion temp = t.rotation;
                    temp.z = _heldObject.RotationBoundsX[0];
                    t.rotation = temp;
                }
                else if (t.rotation.z > _heldObject.RotationBoundsX[1])
                {
                    Quaternion temp = t.rotation;
                    temp.z = _heldObject.RotationBoundsX[1];
                    t.rotation = temp;
                }
                if (t.rotation.x < _heldObject.RotationBoundsY[0])
                {
                    Quaternion temp = t.rotation;
                    temp.x = _heldObject.RotationBoundsY[0];
                    t.rotation = temp;
                } 
                else if (t.rotation.x > _heldObject.RotationBoundsY[1])
                {
                    Quaternion temp = t.rotation;
                    temp.x = _heldObject.RotationBoundsY[1];
                    t.rotation = temp;
                }
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
            //Debug.Log("current y is " + _floor.transform.position.z + " and z conversion is " + zConversion);

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
        if (_objectHeld ||!_drawerPulled || !_interactionEnabled) return;
        _heldObject = obj;
        _heldContainer.transform.position = _heldObject.ShowPosition;
        _heldObject.transform.SetParent(p: _heldContainer.transform);
        _heldObject.SetHeld();
        _objectHeld = true;
        _audioSource.clip = _selectClip;
        _audioSource.Play();
        if (!_instructionsShown && _instructions != null)
        {
            _instructions.SetActive(value: true);
            _instructionsShown = true;
        }
        _textboxClosed.gameObject.SetActive(value: true);
    }
    
    private void OnTextBoxClicked(TextBox box)
    {
        if (box.isClosed)
        {
            _instructions.SetActive(value: false);
            box.gameObject.SetActive(value: false);
            ShowText(text: _heldObject.Description, textBox: _filetextbox, showQuestionBox: true);
            _audioSource.clip = _nextClip;
            _audioSource.Play();
            //_rotateCoroutine = StartCoroutine(routine: RotateObjectAsync());
            return;
        }

        if (_textDone)
        {
            _audioSource.clip = _nextClip;
            _audioSource.Play();
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
        
        if (_suitcaseFull)
        {
            _questionbox.text.text = _alternateTrashText;
        }
        
        yield return new WaitUntil(() => _selectedChoice);
        
        _selectedChoice = false;
        _questionbox.gameObject.SetActive(value: false);
        _keepButton.gameObject.SetActive(value: false);
        _trashButton.gameObject.SetActive(value: false);
        
        _textboxShown = false;
    }

    private IEnumerator RotateObjectAsync()
    {
        while (true)
        {
            Quaternion tempRot = _heldContainer.transform.localRotation;
            tempRot.y += 1;
            _heldContainer.transform.localRotation = tempRot;
            yield return _rotationWait;
        }
    }

    public void onKeepClicked()
    {
        if (_fill.transform is RectTransform rectTransform)
        {
            Vector2 temp = rectTransform.sizeDelta;
            temp.y += _heldObject.SpaceValue;
            if (temp.y >= _fullValue)
            {
                temp.y = _fullValue;
                _suitcaseFull = true;
            }

            rectTransform.sizeDelta = temp;
        }
        
        _heldObject.gameObject.transform.parent = null;
        _heldObject.SetUnheld();
        _heldObject.gameObject.SetActive(value: false);
        _heldObject = null;
        _objectHeld = false;
        _selectedChoice = true;
        //StopCoroutine(routine: _rotateCoroutine);
        Quaternion rot = _heldContainer.transform.rotation;
        rot.y = 0;
        _heldContainer.transform.rotation = rot;
        _audioSource.clip = _keepClip;
        _audioSource.Play();
        _objectsDone++;
        if (_objectsDone >= _objects.Length)
        {
            StartCoroutine(routine: StartEndingAsync());
        }
    }

    public void onTrashClicked()
    { 
        _heldObject.SetUnheld();
        Destroy(obj: _heldObject.gameObject);
        _heldObject = null;
        _objectHeld = false;
        _selectedChoice = true;
        //StopCoroutine(routine: _rotateCoroutine);
        Quaternion rot = _heldContainer.transform.rotation;
        rot.y = 0;
        _heldContainer.transform.rotation = rot;
        _audioSource.clip = _trashClip;
        _audioSource.Play();
        _objectsDone++;
        if (_objectsDone >= _objects.Length)
        {
            StartCoroutine(routine: StartEndingAsync());
        }
    }

    private IEnumerator StartIntroAsync()
    {
        _black.SetActive(value: true);
        yield return _introWait;
        _audioSource.Play();
        _black.SetActive(value: false);
        yield return _introWait;
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
        _suitcase.SetActive(value: true);
    }

    private IEnumerator StartEndingAsync()
    {
        _interactionEnabled = false;
        yield return _introWait;
        yield return ShowTextAsync(text: _ending, textBox: _textbox, showQuestionBox: false);
        yield return _introWait;
        _audioSource.Play();
        yield return _introWait;
        _deskAnimation.Play();
        yield return _introWait;
        _black.SetActive(value: true);
        yield return _introWait;
        _blackText.SetActive(value: true);
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

