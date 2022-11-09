using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public class SampleSceneSettings {
    public float pointer_lifetime;
    public float pointer_cameraDistMin;
    public float pointer_cameraDistMax;
}


public class SampleSceneManager : MonoBehaviour
{

    #region Variables

    [Header("Settings")]
    public GameObject pointerPrefab;
    public SampleSceneSettings sampleSceneSettings;
    AppManager appManager;

    [Header("GUI")]
    public TMP_InputField inputLifetime;
    public TMP_InputField inputCameraDistMin;
    public TMP_InputField inputCameraDistMax;

    #endregion


    #region Standard Funtions

    void Awake(){
       appManager = this.transform.parent.GetComponent<AppManager>();
    }

    #endregion


    #region Settings

    public void InitGuiSettings() {
        inputLifetime.placeholder.GetComponent<TMP_Text>().text = sampleSceneSettings.pointer_lifetime.ToString();
        inputCameraDistMin.placeholder.GetComponent<TMP_Text>().text = sampleSceneSettings.pointer_cameraDistMin.ToString();
        inputCameraDistMax.placeholder.GetComponent<TMP_Text>().text = sampleSceneSettings.pointer_cameraDistMax.ToString();
    }

    public void SaveSceneSettings(){
        sampleSceneSettings.pointer_lifetime = (inputLifetime.text != "") ? float.Parse(inputLifetime.text) : sampleSceneSettings.pointer_lifetime;
        sampleSceneSettings.pointer_cameraDistMin = (inputCameraDistMin.text != "") ? float.Parse(inputCameraDistMin.text) : sampleSceneSettings.pointer_cameraDistMin;
        sampleSceneSettings.pointer_cameraDistMax = (inputCameraDistMax.text != "") ? float.Parse(inputCameraDistMax.text) : sampleSceneSettings.pointer_cameraDistMax;
        File.WriteAllText(Application.dataPath + "/StreamingAssets/Config/sample.json", JsonUtility.ToJson(sampleSceneSettings));
        appManager.ManageMainSettingPannel();
        appManager.RestartApp();
    }

    #endregion

}