using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TouchManager : MonoBehaviour
{

    #region Variables
    
    [Header("Pointers")]

    public Transform pointersPool;
    GameObject pointerPrefab;
    Vector2 pointerCameraDist;
    float pointerLifetime;
    float trackingGuiScale;
    public Texture trackingGuiTexture;
    bool guiDebug;
    public Dictionary<string, PointerBehavior> pointersDict = new Dictionary<string, PointerBehavior>();
    float mouseWheel;

    [Header("Dependencies")]
    public AppManager appManager;
    public OscManager oscManager;

    #endregion


    #region Standard Functions

    void Awake(){
        trackingGuiScale = (int)System.Math.Round((double)(Screen.width/15f), 0);
    }

    void Update() {

        // draw GUI to debug position of pointer
        if (Input.GetKeyUp(KeyCode.D)) guiDebug = !guiDebug;

        // return if OSC enabled or settings pannel opened
        if(oscManager.isEnabled || appManager.settingsPannel.activeSelf) return;

        // mouse debuging
        if(Input.GetMouseButton(2)) {
            TouchInfo debugTouch = new TouchInfo();
            debugTouch.id = "mouse";
            debugTouch.x = normalizedTrackingValue(Input.mousePosition.x, Screen.width);
            debugTouch.y = mouseScrollWheelValue(.1f);
            debugTouch.z = System.Math.Abs(1 - normalizedTrackingValue(Input.mousePosition.y, Screen.height));
            OnTouch(debugTouch);
        }

    }

    #endregion


    #region Pointer

    public void Init(GameObject _prefab, float _lifetime, Vector2 _cameraDist){
        pointerPrefab = _prefab;
        pointerLifetime = _lifetime;
        pointerCameraDist = _cameraDist;
    }

    public void OnTouch(TouchInfo _touchInfo) {
        // get pointer gameobject
        GameObject pointer;
        if(pointersDict.ContainsKey(_touchInfo.id)){
            pointer = pointersDict[_touchInfo.id].gameObject;
            pointersDict[_touchInfo.id].lifetime = pointerLifetime;
        } else {
            pointer =  Instantiate(pointerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            pointer.name = _touchInfo.id;
            pointer.transform.parent = pointersPool;
            PointerBehavior _pointerBehavior = pointer.transform.GetComponent<PointerBehavior>();
            _pointerBehavior.Init(pointerLifetime, this);
            pointersDict.Add(_touchInfo.id, _pointerBehavior);
        }
        // update position of my pointer;
        UpdatePointerPosition (pointer, new Vector3(_touchInfo.x*Screen.width, (1-_touchInfo.z)*Screen.height, _touchInfo.y));
    }

    void UpdatePointerPosition(GameObject _pointer, Vector3 _position) {
        Vector3 _pos = _position;
        _pos.z = pointerCameraDist.x + ((pointerCameraDist.y-pointerCameraDist.x)*_position.z); 
        _pointer.transform.position = Camera.main.ScreenToWorldPoint(_pos);
    }

    void OnGUI(){
        if(!guiDebug) return;
        foreach( KeyValuePair<string, PointerBehavior> pointer in pointersDict) {
            Vector3 _screenPos = Camera.main.WorldToScreenPoint(pointer.Value.transform.localPosition);
            GUI.DrawTexture(new Rect(_screenPos.x - (trackingGuiScale/2), Screen.height -_screenPos.y - (trackingGuiScale/2), trackingGuiScale, trackingGuiScale), trackingGuiTexture, ScaleMode.ScaleAndCrop, true, 0.0F);
        }
    }

    #endregion


    #region Utils

    float mouseScrollWheelValue(float _speed){
        mouseWheel += Input.mouseScrollDelta.y * _speed;
        if(mouseWheel>1) mouseWheel = 1;
        else if(mouseWheel<0) mouseWheel = 0;
        return mouseWheel;
    }

    float normalizedTrackingValue(float _value, float _ref){
        float normalizedValue = _value/_ref;
        if(normalizedValue<0) normalizedValue = 0f;
        else if(normalizedValue>1) normalizedValue = 1f;
        return normalizedValue;
    }

    #endregion

}