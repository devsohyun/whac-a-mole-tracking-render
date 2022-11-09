using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerBehavior : MonoBehaviour
{

    #region Variables

    [HideInInspector]
    public float lifetime;
    TouchManager touchManager; 
    [HideInInspector]
    public bool isReady;

    #endregion


    #region Standard Functions

    public void Init (float _lifetime, TouchManager _touchManager) {
        lifetime = _lifetime;
        touchManager = _touchManager;
        isReady = true;
    }

    void FixedUpdate(){

        if(!isReady) return;

        if(lifetime <= 0){
            if(touchManager.pointersDict.ContainsKey(this.gameObject.name)) touchManager.pointersDict.Remove(this.gameObject.name);
            Destroy(this.gameObject);
        } else {
            lifetime -= Time.deltaTime;
        }
    }

    #endregion

}
