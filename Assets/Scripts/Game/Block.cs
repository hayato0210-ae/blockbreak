using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {

    // HPの範囲制限
    const int HP_MIN = 0;
    const int HP_MAX = 3;

    // 壊れるときの透明度の変化速度
    const float BREAK_ALPHA_SPEED = 5.0f;
    const float BREAK_SCALE_SPEED = 1.0f;

    // ブロック 1/3 の確立でアイテムを出現させる
    const int ITEM_RANDOM = 3;
    
    // ブロックの種類 GRAY 破壊不可オブジェクト
    public enum TYPE {BLUE, RED, GRAY};

    // アイテムの種類を設定
    public TYPE type = TYPE.BLUE;

    // ブロックの体力
    public int hp = 0;

    // 各色後にスプライトを保持しておく
    public List<Sprite> blueSprites;
    public List<Sprite> redSprites;
    public List<Sprite> graySprites;

    // アイテムのプレファブを格納
    public GameObject itemPrefab;

    // SpriteRenderer を保持
    private SpriteRenderer spriteRenderer;

    // 壊れる処理をするか判定
    private bool isBreak = false;

    void Awake() {
        // SpriteRenderer を保持
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start() {
        // HPの値に応じて画像を変える
        SetSprite();
    }

    // Update is called once per frame
    void Update() {
        // 壊れる処理をするか判定
        if(isBreak){
            BreakEffect();
        }        
    }

    // スプライト画像を type と hp に応じ変える
    void SetSprite(){
        // 初期化必須のため null値 を入れておく 
        Sprite sprite = null;
        
        switch(type){
            case TYPE.BLUE:
                if(hp == 1) sprite = blueSprites[0];
                if(hp == 2) sprite = blueSprites[1];
                if(hp == 3) sprite = blueSprites[2];
                break;
            case TYPE.RED:
                if(hp == 1) sprite = redSprites[0];
                if(hp == 2) sprite = redSprites[1];
                if(hp == 3) sprite = redSprites[2];
                break;
            case TYPE.GRAY:
                sprite = graySprites[0];
                break;
            default:
                // 本来はこないがおまじない
                Debug.Log("Block : SetSprite " + type + " " + hp);
                break;
        }

        // 画像を変える
        GetComponent<SpriteRenderer>().sprite = sprite;
    }

    // ブロックにボールが当たったら呼ぶ
    public void HitBlock(){
        // 壊れないブロックなら処理をやめる
        if(type == TYPE.GRAY){
            return;
        }

        // HPを減らす
        hp--;

        // 画像を変更する
        if(hp > 0){
            SetSprite();
        }

        // HP が 0以下になったら
        if(hp <= 0){         
            // ブロックを壊す処理   
            BreakBlock();
            // SEを鳴らす
            AudioManager.Instance.PlaySE("blockbreak");
        } else {
            // HPが残っている場合
            // SEを鳴らす
            AudioManager.Instance.PlaySE("blockhit");
        }
    }

    // ブロックを壊す処理
    public void BreakBlock(){
        // オブジェクトを破棄
        // Destroy(gameObject);

        // コリジョン判定を無くす
        SetTrigger(true);
        isBreak = true;
        //GetComponent<BoxCollider2D>().isTrigger = true;

        // 確率でアイテムの生成 
        // using Systemのものと混同しないように UnityEngine と明記しておく
        if(UnityEngine.Random.Range(0, ITEM_RANDOM) == 0){
            
            // アイテムを生成
            GameObject obj = Instantiate(
                itemPrefab,
                transform.localPosition,
                Quaternion.identity
            ) as GameObject;
            
            // obj.GetComponent<Item>().ChangeRandom();

            // Debug.Log(obj.GetComponent<Item>().type);
            
        }

        // ブロックの個数カウントを減らす
        //GameManager.Instance.MinusBlocks(1);
    }

    // 当たり判定の管理
    public void SetTrigger(bool flag){
        // 壊れるエフェクト中 か 壊れないオブジェクトなら処理中止
        if(isBreak || type == TYPE.GRAY){
            return;
        }
        // コリジョン判定を無くす
        GetComponent<BoxCollider2D>().isTrigger = flag;
    }

    // 壊れるエフェクトの処理
    private void BreakEffect(){
        // スケールを拡大していく
        Vector3 scale = transform.localScale;
        scale.x += BREAK_SCALE_SPEED * Time.deltaTime;
        scale.y += BREAK_SCALE_SPEED * Time.deltaTime;
        transform.localScale = scale;

        // 透明度を徐々に薄くしていく
        Color color = spriteRenderer.color;
        color.a -= BREAK_ALPHA_SPEED * Time.deltaTime;
        
        // 透明度が 0 以下になったらオブジェクト削除
        if(color.a < 0.0f){
            color.a = 0.0f;
            Destroy(gameObject);
        }

        // 値を格納する
        spriteRenderer.color = color;
    }

    // Inspector の値が変更されたら呼ばれる
    private void OnValidate(){        
        // 範囲制限をかける
        hp = Mathf.Clamp(hp, HP_MIN, HP_MAX);
        // HPの値に応じて画像を変える
        SetSprite();
    }
}
