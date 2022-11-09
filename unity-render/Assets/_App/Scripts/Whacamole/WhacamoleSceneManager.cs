using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;

public class WhacamoleSceneSettings {
    public float pointer_lifetime;
    public float pointer_cameraDistMin;
    public float pointer_cameraDistMax;
    public float mole_standardHitDepth;
    public float mole_hardHitDepth;
    public float molehole_scale;
}
public class WhacamoleSceneManager : MonoBehaviour
{
    #region Variables

    [Header("Settings")]
    public GameObject pointerPrefab;

    [Header("Setting Panel")]
    // scene setting panel
    public TMP_InputField inputLifetime;
    public TMP_InputField inputCameraDistMin;
    public TMP_InputField inputCameraDistMax;
    public TMP_InputField inputStandardHitDepth;
    public TMP_InputField inputHardHitDepth;
    public TMP_InputField inputMoleHoleScale;

    [Header("UI")]
    public Transform moleHoleContainer;
    public List<Mole> moles;
    public TMP_Text scoreText;
    public TMP_Text timeText;
    public Transform gameOverContainer;

    [Header("Dependencies")]
    public WhacamoleSceneSettings whacamoleSceneSettings;

    // game settings
    public int score;
    public float startingTime = 50;
    List<Mole> currentMoles = new List<Mole>();
    float timeRemaining;
    bool isPlaying = false;

    [Header("Dependencies")]
    public AppManager appManager;
    public Clap clap;


    #endregion

    #region Standard Functions

    void Awake(){
        Init();
    }

    void Init(){
        // hide all moles before start 
        foreach (Mole mole in moles) mole.gameObject.SetActive(false);
        scoreText.text = score.ToString();
        timeText.text = timeRemaining.ToString();  
        gameOverContainer.transform.GetChild(0).gameObject.SetActive(false);
    }

    void Update(){
        clap.CheckClap();
      
        if(isPlaying){
            timeRemaining -= Time.deltaTime;
            if(timeRemaining <= 0){
                timeRemaining = 0;
                GameOver(0);
            }
            timeText.text = ((int)timeRemaining).ToString();
            if (currentMoles.Count <= (score / 10)) {
                // Choose a random mole.
                int index = Random.Range(0, moles.Count);
                // Doesn't matter if it's already doing something, we'll just try again next frame.
                if (!currentMoles.Contains(moles[index])) {
                  currentMoles.Add(moles[index]);
                  moles[index].Activate(score / 10);
                }
            }
        }
    
    }

    #endregion

    #region Settings

    public void StartGame(){
        foreach (Mole mole in moles) mole.gameObject.SetActive(true);
        
        for (int i = 0; i < moles.Count; i++)
        {
            moles[i].Hide();
            moles[i].SetIndex(i);
        }
        gameOverContainer.transform.gameObject.SetActive(false);
        currentMoles.Clear();
        timeRemaining = startingTime;
        score = 0;
        scoreText.text = score.ToString();
        isPlaying = true;
    }

    public void GameOver(int _type){
        if(_type == 0){
            // out of time text
        }else{
            // bomb text
        }

        foreach (Mole mole in moles)
        {
            mole.StopGame();
        }
        gameOverContainer.transform.gameObject.SetActive(true);
        gameOverContainer.transform.GetChild(0).gameObject.SetActive(true);
        isPlaying = false;
    }

    public void AddScore(int _moleIndex, int _moleScore){
        score += _moleScore;
        scoreText.text = score.ToString();
        currentMoles.Remove(moles[_moleIndex]);
    }

    public void Missed(int _moleIndex, bool _isMole){
        if(_isMole){
            timeRemaining -= 2;
        }
        currentMoles.Remove(moles[_moleIndex]);
    }

    #endregion

    #region Scene Settings

    // set up JSON
    public void InitGuiSettings() {
        inputLifetime.placeholder.GetComponent<TMP_Text>().text = whacamoleSceneSettings.pointer_lifetime.ToString();
        inputCameraDistMin.placeholder.GetComponent<TMP_Text>().text = whacamoleSceneSettings.pointer_cameraDistMin.ToString();
        inputCameraDistMax.placeholder.GetComponent<TMP_Text>().text = whacamoleSceneSettings.pointer_cameraDistMax.ToString();
        inputStandardHitDepth.placeholder.GetComponent<TMP_Text>().text = whacamoleSceneSettings.mole_standardHitDepth.ToString();
        inputHardHitDepth.placeholder.GetComponent<TMP_Text>().text = whacamoleSceneSettings.mole_hardHitDepth.ToString();
        inputMoleHoleScale.placeholder.GetComponent<TMP_Text>().text = whacamoleSceneSettings.molehole_scale.ToString();
    }

    public void SaveSceneSettings(){
        whacamoleSceneSettings.pointer_lifetime = (inputLifetime.text != "") ? float.Parse(inputLifetime.text) : whacamoleSceneSettings.pointer_lifetime;
        whacamoleSceneSettings.pointer_cameraDistMin = (inputCameraDistMin.text != "") ? float.Parse(inputCameraDistMin.text) : whacamoleSceneSettings.pointer_cameraDistMin;
        whacamoleSceneSettings.pointer_cameraDistMax = (inputCameraDistMax.text != "") ? float.Parse(inputCameraDistMax.text) : whacamoleSceneSettings.pointer_cameraDistMax;
        whacamoleSceneSettings.mole_standardHitDepth = (inputStandardHitDepth.text != "") ? float.Parse(inputStandardHitDepth.text) : whacamoleSceneSettings.mole_standardHitDepth;
        whacamoleSceneSettings.mole_hardHitDepth = (inputHardHitDepth.text != "") ? float.Parse(inputHardHitDepth.text) : whacamoleSceneSettings.mole_hardHitDepth;
        whacamoleSceneSettings.molehole_scale = (inputMoleHoleScale.text != "") ? float.Parse(inputMoleHoleScale.text) : whacamoleSceneSettings.molehole_scale;
        File.WriteAllText(Application.dataPath + "/StreamingAssets/Config/whacamole.json", JsonUtility.ToJson(whacamoleSceneSettings));
        appManager.ManageMainSettingPannel();
        appManager.RestartApp();
    }

    #endregion
}
