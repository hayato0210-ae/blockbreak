using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {

    // 加算するスコア
    const int ADD_SCORE = 10;

    // ボールのスピード
    public float speed = 50.0f;

    // ボールの初回移動方向
    public Vector2 dir = new Vector2(1, 1);
    
    // Rigidbody2D を取得用
    private Rigidbody2D rigid;

    // 移動しているかチェック
    private bool moveFlag;    

    // 停止前の移動量を保持
    private Vector2 lastVelocity;

    // GameManager管理用
    // private GameManager gameManager;

    void Awake() {
        // Ball の Rigidbody2D を取得しておく
        rigid = GetComponent<Rigidbody2D>();   
        // GameManager管理用
        // gameManager = Camera.main.GetComponent<GameManager>();     
    }

    // Start is called before the first frame update
    void Start() {        
        // 停止処理
        Stop();
    }

    // 移動
    public void Move(){
        // 移動しているかフラグ管理
        moveFlag = true;

        // 移動していたことがなければ初期値を設定
        if(lastVelocity == Vector2.zero){            
            // 真上の単位ベクトル
            Vector2 dir = new Vector2(1, 1);
            
            // 移動量を加算
            rigid.AddForce(dir * speed);
        }else{        
            // 停止前に移動していた方向に設定
            rigid.velocity = lastVelocity;
        }
    }

    // 停止
    public void Stop(){
        // 移動しているかフラグ管理
        moveFlag = false;

        // 停止前に移動量を保持
        lastVelocity = rigid.velocity;

        // 移動量を 0 に設定
        rigid.velocity = Vector2.zero;
    }
    
    // ボール軌跡のグラデーション色を変更する
    public void TrailGradient(Color color){
        // カラーキーの配列
        GradientColorKey[] colorKeys = new [] {
            new GradientColorKey( color, 0 ),
            new GradientColorKey( color, 1 ),
        };

        // アルファ値の配列
        GradientAlphaKey[] alphaKeys = new [] {
            new GradientAlphaKey( 1, 0 ),
            new GradientAlphaKey( 0, 1 ),
        };

        // グラデーションを設定する
        Gradient gradient = new Gradient();
        gradient.SetKeys( colorKeys, alphaKeys );

        // TrailRenderer にグラデーションを設定する
        GetComponent<TrailRenderer>().colorGradient = gradient;

        // ボール自身の色も変える
        GetComponent<SpriteRenderer>().color = color;
    }

    // 他のコリジョンに当たった時
    void OnCollisionEnter2D(Collision2D other) {
        
        // 当たったオブジェクト名を出力
        // Debug.Log("OnCollisionEnter2D : " + other.gameObject.name);
        
        if(other.gameObject.tag == "Block"){
            // ブロックを当たった時の処理
            other.gameObject.GetComponent<Block>().HitBlock(); 
            // スコアを加算する
            // gameManager.AddScore(ADD_SCORE);    
            GameManager.Instance.AddScore(ADD_SCORE);    
        }
    }

    // 他のコリジョンに当たった時
    void OnTriggerEnter2D(Collider2D other) {

        // 当たったオブジェクト名を出力
        // Debug.Log("OnTriggerEnter2D : " + other.gameObject.name);
        
        // アタック処理中なら
        if(GameManager.Instance.isAttack) {
            // ブロックと当たった際
            if(other.gameObject.tag == "Block") {           
                // 当たった先のオブジェクトを削除
                //Destroy(other.gameObject);
                other.gameObject.GetComponent<Block>().BreakBlock();
                // スコアを 5倍 加算する
                GameManager.Instance.AddScore(ADD_SCORE * 5);
            }
        }

        // アイテムと当たった際
        if(other.gameObject.tag == "Item") {        
            // 当たった先のオブジェクトを削除
            Destroy(other.gameObject);
            switch(other.gameObject.GetComponent<Item>().type){
                // アタックの処理
                case Item.TYPE.ATTACK:                
                    GameManager.Instance.GetAttack();
                    break;
                
                // ボールが増える処理
                case Item.TYPE.BALL:
                    GameManager.Instance.GetBall(transform.position);
                    break;

                // バーが伸びる処理
                case Item.TYPE.BAR:
                    GameManager.Instance.GetBar();
                    break;
            }    
           
            // SEを鳴らす
            AudioManager.Instance.PlaySE("get");
            
        }        
    }

    // カメラに表示されなくなったタイミングで呼ばれる
    void OnBecameInvisible(){        
        // BallDestroy を1秒後に呼び出す
        // TrailRenderer用
        Invoke("BallDestroy", 1.0f);     
	}

    // ボールを破棄
    void BallDestroy(){
        Destroy(gameObject);
    }
}
