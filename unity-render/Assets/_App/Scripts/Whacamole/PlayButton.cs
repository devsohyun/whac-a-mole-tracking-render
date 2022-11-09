using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayButton : MonoBehaviour
{
    #region Variables

    public WhacamoleSceneManager whacamoleSceneManager;

    #endregion

    #region Collider

    void OnTriggerEnter2D(Collider2D _col){
        if(_col.name == "trigger" || _col.name == "mouse"){
            whacamoleSceneManager.StartGame();
        }
    }

    #endregion
}
