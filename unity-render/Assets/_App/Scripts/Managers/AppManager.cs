using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using TMPro;
using System;
using UnityEngine.UI;

public enum RenderScene {Sample, Whacamole} // add a new Enum for each new scene

public class GlobalSettings {
    public string scene_id;
    public bool osc_status;
    public string osc_port;
}

public class AppManager : MonoBehaviour
{
    #region Variables

    [Header("Settings")]
    public RenderScene renderScene;
    public GlobalSettings globalSettings;
    public GameObject sceneManager;
    bool isBuild;

    [Header("GUI")]
    public GameObject settingsPannel;
    public TMP_Dropdown gs_scenesDropdown;

    public Toggle gs_oscIsListening;
    public TMP_InputField gs_oscPortIn;
    public TMP_Text gs_oscLogs;

    
    [Header("Dependencies")]
    public OscManager oscManager;
    public TouchManager touchManager;

    #endregion


    #region Standard Functions

    void Awake(){
        #if !UNITY_EDITOR
            isBuild = true;
        #endif
        LoadSettings();
    }

    void Update()
    {
        // keyboard shortcuts
        if (Input.GetKeyUp(KeyCode.Q)) QuitApp();
        if (Input.GetKeyUp(KeyCode.R)) RestartApp();
        if (Input.GetKeyUp(KeyCode.S)) ManageMainSettingPannel();
    }

    public void QuitApp(){
        if(settingsPannel.activeSelf) return;
        Application.Quit();
    }

    public void RestartApp(){ 
        if(settingsPannel.activeSelf) return;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void LoadSceneIfExisted(){
        int _sceneToLoad = (int)(RenderScene)Enum.Parse(typeof(RenderScene), globalSettings.scene_id);
        if(_sceneToLoad<SceneManager.sceneCountInBuildSettings) SceneManager.LoadScene(_sceneToLoad);
    }

    #endregion


    #region Global Settings GUI

    public void ManageMainSettingPannel(){
        bool _status = !settingsPannel.activeSelf;
        settingsPannel.SetActive(_status);
        // reset global settings on close GUI
        if(_status) return;
        gs_scenesDropdown.value = (int)renderScene;
        gs_oscPortIn.text = "";
        // manage mouse cursor
        #if !UNITY_EDITOR
            if(_status) Cursor.visible = true;
            else Cursor.visible = !oscManager.isEnabled;   
        #endif
    }

    #endregion


    #region Settings

    void LoadSettings(){
        // load global settings 
        string globalFile = File.ReadAllText(Application.dataPath + "/StreamingAssets/Config/global.json");
        globalSettings = JsonUtility.FromJson<GlobalSettings>(globalFile);
        if( isBuild && Enum.IsDefined(typeof(RenderScene), globalSettings.scene_id) && (globalSettings.scene_id != ((RenderScene)SceneManager.GetActiveScene().buildIndex).ToString()))
            LoadSceneIfExisted();
        InitGuiSettings();
        oscManager.Init();

        // load scene settings
        string sceneFile;
        switch(SceneManager.GetActiveScene().buildIndex)
        {
            case 0:
                sceneFile = File.ReadAllText(Application.dataPath + "/StreamingAssets/Config/sample.json");
                SampleSceneManager sampleSceneManager = sceneManager.GetComponent<SampleSceneManager>();
                sampleSceneManager.sampleSceneSettings = JsonUtility.FromJson<SampleSceneSettings>(sceneFile);
                touchManager.Init(sampleSceneManager.pointerPrefab, sampleSceneManager.sampleSceneSettings.pointer_lifetime, new Vector2 (sampleSceneManager.sampleSceneSettings.pointer_cameraDistMin, sampleSceneManager.sampleSceneSettings.pointer_cameraDistMax));
                sampleSceneManager.InitGuiSettings();
                break;

            case 1:
                sceneFile = File.ReadAllText(Application.dataPath + "/StreamingAssets/Config/whacamole.json");
                WhacamoleSceneManager whacamoleSceneManager = sceneManager.GetComponent<WhacamoleSceneManager>();
                whacamoleSceneManager.whacamoleSceneSettings = JsonUtility.FromJson<WhacamoleSceneSettings>(sceneFile);
                touchManager.Init(whacamoleSceneManager.pointerPrefab, whacamoleSceneManager.whacamoleSceneSettings.pointer_lifetime, new Vector2 (whacamoleSceneManager.whacamoleSceneSettings.pointer_cameraDistMin, whacamoleSceneManager.whacamoleSceneSettings.pointer_cameraDistMax));
                whacamoleSceneManager.InitGuiSettings();
                break;


                // add a new case per each scene
        }
    }
    
    void InitGuiSettings(){
        // disable main settings GUI
        settingsPannel.SetActive(false);
        // populate scene dropdown options
        List<string> m_DropOptions = new List<string>();
        foreach(RenderScene renderScene in RenderScene.GetValues(typeof(RenderScene)))
            m_DropOptions.Add(renderScene.ToString());
        gs_scenesDropdown.AddOptions(m_DropOptions);
        // apply JSON value
        renderScene = Enum.IsDefined(typeof(RenderScene), globalSettings.scene_id) ? (RenderScene)Enum.Parse(typeof(RenderScene), globalSettings.scene_id) : RenderScene.Sample;
        gs_scenesDropdown.value = (int)renderScene;
        gs_oscPortIn.placeholder.GetComponent<TMP_Text>().text = globalSettings.osc_port;
        gs_oscIsListening.isOn = globalSettings.osc_status;
        // clear osc logs
        gs_oscLogs.text = "";
    }

    public void SaveGlobalSettings(){
        globalSettings.scene_id = ((RenderScene)gs_scenesDropdown.value).ToString();
        globalSettings.osc_port = (gs_oscPortIn.text != "") ? gs_oscPortIn.text : globalSettings.osc_port;
        File.WriteAllText(Application.dataPath + "/StreamingAssets/Config/global.json", JsonUtility.ToJson(globalSettings));
        ManageMainSettingPannel();
        if(isBuild) RestartApp();
        else LoadSceneIfExisted();
    }

    #endregion

}
