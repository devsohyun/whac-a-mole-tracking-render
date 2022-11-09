using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clap : MonoBehaviour
{
    #region Variables

    public GameObject triggerPrefab;
    public bool isClapped;
    float clapDistance = 2.5f;

    [Header("Dependencies")]
    public AppManager appManager;

    #endregion


    public void CheckClap(){
        // clap only when touches are in the pool
        if(this.transform.childCount < 2) return;
        if(this.transform.childCount >= 2){
            GameObject trigger;
            float distance = Vector3.Distance (this.transform.GetChild(0).transform.localPosition, this.transform.GetChild(1).transform.localPosition);
            // create trigger
            if(!isClapped && distance <= clapDistance) {
                trigger = Instantiate(triggerPrefab, getTouchesMidPoint(), Quaternion.identity);
                trigger.name = "trigger";
                trigger.transform.parent = this.transform;
                this.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.red;
                this.transform.GetChild(1).GetComponent<Renderer>().material.color = Color.red;
                isClapped = true;
            // remove trigger
            }else if(isClapped && distance > clapDistance){
                trigger = this.transform.Find("trigger").gameObject;
                if(trigger != null) Destroy(trigger);
                this.transform.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.yellow;
                this.transform.transform.GetChild(1).GetComponent<Renderer>().material.color = Color.yellow;
                isClapped = false;
            }
            // update trigger position
            // - box collider trigger is working better when colliding object is moving)
            if(isClapped) {
                trigger = this.transform.Find("trigger").gameObject;
                trigger.transform.localPosition = new Vector3(getTouchesMidPoint().x, getTouchesMidPoint().y, trigger.transform.localPosition.z);
            }  
        }
    }

    Vector2 getTouchesMidPoint(){
        return new Vector2( this.transform.GetChild(0).transform.localPosition.x + (this.transform.GetChild(1).transform.localPosition.x - this.transform.GetChild(0).transform.localPosition.x) / 2, this.transform.GetChild(0).transform.localPosition.y + (this.transform.GetChild(1).transform.localPosition.y - this.transform.GetChild(0).transform.localPosition.y) / 2);
    }

}
