using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bar : MonoBehaviour {

    // アイテム取得時の横幅最大サイズ
    const float LONG_MODE_X = 3.0f;

    // ロングモードになってから指定秒数で小さくなる
    const float LONG_TIME = 5.0f;

    // 移動速度
    public float speed = 0.05f;

    // ロングモード終了時に 1秒間 で小さくなる速度
    public float smallSpeed = 0.25f;

    // ロングモードかの管理
    private bool isLong = false;

    // ロングモードに入ってからの時間
    private float longTime = 0.0f;
    
    // 画面サイズを取得
    private Vector2 screen = Vector3.zero;

    // オブジェクトのサイズ取得
    private Vector3 size = Vector3.zero;    
    
    // 開始時のバーのサイズ
    private Vector3 startScale = Vector3.zero;
    
    
    // GameManager管理用
    // private GameManager gameManager;
    
    void Awake() {
        // GameManager管理用
        // gameManager = Camera.main.GetComponent<GameManager>();
    }
    
    // Start is called before the first frame update
    void Start() {        
        // 左下、右上の座標を取得
        Vector3 bottomLeft = Camera.main.ScreenToWorldPoint (Vector3.zero);
        Vector3 topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));

        // 画面サイズを取得
        screen.x =  Mathf.Abs(bottomLeft.x) + Mathf.Abs(topRight.x);
        screen.y =  Mathf.Abs(bottomLeft.y) + Mathf.Abs(topRight.y);

        // 画面サイズを出力
        Debug.Log(screen);
        
        // サイズ取得
        size = GetComponent<SpriteRenderer>().bounds.size;
        Debug.Log(size);

        // 開始時のバーのサイズ
        startScale = transform.localScale;
    }

    // Update is called once per frame
    void Update() {        
        
        // GameManager の state が GAME 以外なら処理を受け付けない
        //if(gameManager.state != GameManager.STATE.GAME){
        //    return;
        //}
        if(GameManager.Instance.state != GameManager.STATE.GAME){
            return;
        }        
    
        // 実行環境がUnityエディターかWindows、Mac、LinuxOSだった場合
        #if UNITY_EDITOR || UNITY_STANDALONE
        // 移動処理
        Move();

        // 実行環境がiPhoneかAndroidだった場合
        #elif UNITY_IPHONE || UNITY_ANDROID
        
        #endif

        // ロングモードなら
        if(isLong){
            LongMode();
        }
    }

    // 移動処理
    private void Move(){
        // 左キーを押したときの処理
        if (Input.GetKey(KeyCode.LeftArrow)) {
            transform.Translate(-speed, 0f, 0f);
        }

        // 右キーを押したときの処理
        if (Input.GetKey(KeyCode.RightArrow)) {
            transform.Translate(speed, 0f, 0f);
        }

        // 移動制限
        Vector3 pos = transform.localPosition;

        // 左端を超えたら
        if(pos.x < -screen.x/2 + size.x/2){
            pos.x = -screen.x/2 + size.x/2;
        }

        // 右端を超えたら
        if(pos.x > screen.x/2 - size.x/2){
            pos.x = screen.x/2 - size.x/2;
        }

        // 現在値をセットしなおす
        transform.localPosition = pos;
    }

    // ロングモードの処理
    public void StartLong() {        
        isLong = true;
        longTime = 0.0f;

        // スケール の設定
        Vector3 scale = startScale;
        scale.x = LONG_MODE_X;
        transform.localScale = scale;
    } 
    
    // ロングモード
    private void LongMode(){  
        // ロングモード中の時間を計測
        longTime += Time.deltaTime;
        if(longTime >= LONG_TIME){
            
            // スケール の設定
            Vector3 scale = transform.localScale;
            scale.x -= smallSpeed * Time.deltaTime;

            // 最初のスケールより小さくなったら、それ以下にならないようにする
            if(scale.x <= startScale.x){
                scale.x = startScale.x;
                isLong = false;
                longTime = 0.0f;
            }

            // スケール値の設定
            transform.localScale = scale;
        }

        // 移動制限範囲を再計算
        size = GetComponent<SpriteRenderer>().bounds.size;
    }
}
