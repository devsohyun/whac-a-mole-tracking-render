using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchInfo
{
    public string id;
    public float x;
    public float y;
    public float z;
}

public class OscManager : MonoBehaviour
{

    #region Variables

    [Header("OSC")]
    public bool isEnabled;
    
    [Header("Dependencies")]
    public AppManager appManager;
    public OSC osc;
    public TouchManager touchManager;

    #endregion


    #region OSC
    
    public void Init(){
        isEnabled = appManager.globalSettings.osc_status;
        osc.inPort = int.Parse(appManager.globalSettings.osc_port);
        osc.gameObject.SetActive(isEnabled);
        if(isEnabled) osc.SetAddressHandler("/touches", OnReceiveTouches);
        // manage cursor
        #if !UNITY_EDITOR
            Cursor.visible = !isEnabled;
        #endif
    }

    public void ManageOscSatus() {
        bool _status = appManager.gs_oscIsListening.isOn;
        appManager.globalSettings.osc_status = _status;
        isEnabled = _status;
        osc.gameObject.SetActive(_status);
        if(_status) osc.SetAddressHandler("/touches", OnReceiveTouches);
    }

    void OnReceiveTouches(OscMessage _touches)
    {
        // return for no value sent
        if (_touches.values.Count == 0) return;
       
        // clear my ocs logs
        appManager.gs_oscLogs.text = "";
        
        // manage touch data
        for (int i = 0; i < _touches.values.Count; i++) {
            if(appManager.settingsPannel.activeSelf) appManager.gs_oscLogs.text +=  _touches.values[i].ToString() + "\n";
            touchManager.OnTouch(JsonUtility.FromJson<TouchInfo>( _touches.values[i].ToString()));   
        }
    }

    #endregion

}
