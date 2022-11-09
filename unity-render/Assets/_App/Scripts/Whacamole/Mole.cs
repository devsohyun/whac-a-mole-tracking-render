using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Mole : MonoBehaviour
{
    # region Variables

    [Header("GUI")]
    // sprites
    public Sprite mole;
    public Sprite moleHardHat;
    public Sprite moleHatBroken;
    public Sprite moleHit;
    public Sprite moleHatHit;

    // components
    SpriteRenderer spriteRenderer;
    Animator animator;
    BoxCollider2D boxCollider2D;
    Vector2 boxOffset;
    Vector2 boxSize;
    Vector2 boxOffsetHidden;
    Vector2 boxSizeHidden;
    
    [Header("Settings")]
    public float showDuration = 0.5f;
    float duration = 1f;
    bool hittable = true;
    Vector2 startPosition = new Vector2(0f, -2.56f);
    Vector2 endPosition = new Vector2(0f, 0f);
    public enum MoleType {Standard, HardHat, Bomb};

    MoleType moleType;
    float hardMoleRate = 0.25f;
    float bombRate = 0f;
    int lives;
    int moleIndex = 0;
    public bool isReadyToHit = false;

    [Header("Dependencies")]
    public AppManager appManager;
    public WhacamoleSceneManager whacamoleSceneManager;

    #endregion
    
    #region Standard Functions

    void Awake(){
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        boxCollider2D.isTrigger = true;
        // set collidar size
        boxOffset = boxCollider2D.offset;
        boxSize = boxCollider2D.size;
        boxOffsetHidden = new Vector2(boxOffset.x, -startPosition.y / 2f);
        boxSizeHidden = new Vector2(boxSize.x, 0f);
    }

    #endregion

    #region Mole Settings

    void CreateMoles(){
        float random = Random.Range(0f, 1f);
        if(random < bombRate){
            moleType = MoleType.Bomb;
            animator.enabled = true;
        }else{
            animator.enabled = false;
            random = Random.Range(0f, 1f);
            if(random < hardMoleRate) {
                moleType = MoleType.HardHat;
                spriteRenderer.sprite = moleHardHat;
                lives = 2;
            }else{
                moleType = MoleType.Standard;
                spriteRenderer.sprite = mole;
                lives = 1;
            }
        }
        hittable = true;
    }

    IEnumerator ShowHide(Vector2 _start, Vector2 _end){
        transform.localPosition = _start;

        // show the mole
        float elapsed = 0f;
        while (elapsed < showDuration) {
            transform.localPosition = Vector2.Lerp(_start, _end, elapsed/showDuration);
            boxCollider2D.offset = Vector2.Lerp(boxOffsetHidden, boxOffset, elapsed/showDuration);
            boxCollider2D.size = Vector2.Lerp(boxSizeHidden, boxSize, elapsed/showDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        // end position
        transform.localPosition = _end;
        boxOffset = boxCollider2D.offset;
        boxSize = boxCollider2D.size;
        yield return new WaitForSeconds(duration);
        // hide the mole
        elapsed = 0f;
        while (elapsed < showDuration) {
            transform.localPosition = Vector2.Lerp(_end, _start, elapsed / showDuration);
            boxCollider2D.offset = Vector2.Lerp(boxOffset, boxOffsetHidden, elapsed/showDuration);
            boxCollider2D.size = Vector2.Lerp(boxSize, boxSizeHidden, elapsed/showDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        // back to start position
        transform.localPosition = _start;
        boxCollider2D.offset = boxOffsetHidden;
        boxCollider2D.size = boxSizeHidden;

        // got to the end, it is still hittable then you misse it.
        if(hittable){
            hittable = false;
            whacamoleSceneManager.Missed(moleIndex, moleType != MoleType.Bomb);
        }
    }

    IEnumerator QuickHide(){
        yield return new WaitForSeconds(0.25f);
        // check hittable to prevent flickering
        if(!hittable){
            Hide();
        }
    }

    public void Hide(){
        transform.localPosition = startPosition;
        boxCollider2D.offset = boxOffsetHidden;
        boxCollider2D.size = boxSizeHidden;
    }

    public void Activate(int _level) {
        SetLevel(_level);
        CreateMoles();
        StartCoroutine(ShowHide(startPosition, endPosition));
    }

    void SetLevel(int _level){
        bombRate = Mathf.Min(_level * 0.025f, 0.25f);
        hardMoleRate = Mathf.Min(_level * 0.025f, 1f);
        // Duration bounds get quicker as we progress. No cap on insanity.
        float durationMin = Mathf.Clamp(1 - _level * 0.1f, 0.01f, 1f);
        float durationMax = Mathf.Clamp(2 - _level * 0.1f, 0.01f, 2f);
        duration = Random.Range(durationMin, durationMax);
    }

    public void SetIndex(int _index){
        moleIndex = _index;
    }

    public void StopGame() {
        hittable = false;
        StopAllCoroutines();
    }

    #endregion

    #region Collider

    void OnTriggerEnter2D(Collider2D _col){
        Debug.Log(_col.name);
        if(hittable && (_col.name == "trigger" || _col.name == "mouse")){
            switch (moleType){
                case MoleType.Standard:
                    spriteRenderer.sprite = moleHit;
                    // add score
                    whacamoleSceneManager.AddScore(moleIndex, 1);
                    StopAllCoroutines();
                    StartCoroutine(QuickHide());
                    hittable = false;
                    break;
                case MoleType.HardHat:
                    if(lives == 2){
                        spriteRenderer.sprite = moleHatBroken;
                        lives--;
                    }else{
                        spriteRenderer.sprite = moleHatHit;
                        whacamoleSceneManager.AddScore(moleIndex, 2);
                        StopAllCoroutines();
                        StartCoroutine(QuickHide());
                        hittable = false;
                    }
                    break;
                case MoleType.Bomb:
                    whacamoleSceneManager.GameOver(1);
                    break;
            }
        }
    }

    #endregion

}


