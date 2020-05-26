using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour {

    // ボールの最大数 たくさん表示すると処理が止まるため
    const int BALL_MAX = 5;

    // アイテムの種類 MAX は計算用
    public enum TYPE { ATTACK, BALL, BAR, MAX };

    // アイテムの種類を設定
    public TYPE type = TYPE.ATTACK;
  
    // アイテムが落ちてくる速度
    public float speed = 1.0f;

    // 画像を保持しておく NONEには画像を設定しない
    public List<Sprite> sprites;

    // Start is called before the first frame update
    void Start() {
        // ランダムなものに変わる
        ChangeRandom(); 
    }

    // Update is called once per frame
    void Update() {        
        // アイテムを落下させる
        Vector3 pos = transform.localPosition;
        pos.y -= speed * Time.deltaTime;        
        transform.localPosition = pos;
    }

    // ランダムなアイテムに変わる
    public void ChangeRandom(){
        // 出現させるアイテムもランダムにする 
        // アイテムとして使わない NONE があるため 1 から MAX までのランダム値
        int i = UnityEngine.Random.Range(0, (int)TYPE.MAX);

        // ゲーム中のボール個数がボールの最大値を超えていたら再抽選
        if(GameManager.Instance.balls.Count >= BALL_MAX){
            while(i==(int)TYPE.BALL){
                i = UnityEngine.Random.Range(0, (int)TYPE.MAX);
            }
        }

        // ランダムな値に応じて type をセットし直す
        type = (TYPE)i;
        
        // 画像を変更する
        SetSprite();
    }

    // カメラに表示されなくなったタイミングで呼ばれる
    void OnBecameInvisible(){
        // アイテムを破棄
        Destroy(gameObject);
	}

    // スプライト画像を type に応じて変える
    void SetSprite(){       
        // 画像を変更する
        int num = (int)type; // enum 型を int の値に変換
        GetComponent<SpriteRenderer>().sprite = sprites[num];
    }

    // Inspector の値が変更されたら呼ばれる
    void OnValidate(){
        // スプライト画像を type に応じて変える
        SetSprite();
    }
}
