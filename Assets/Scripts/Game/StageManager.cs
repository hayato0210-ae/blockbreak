using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : SingletonMonoBehaviour<StageManager> {

    public List<GameObject> stagePrefabs;

    // Start is called before the first frame update
    void Start() {

        // ステージ番号を取得
        // static 変数なので Instance をつけない
        // ステージ番号は1から始まるが、Listは0番から始まるため -1 する
        int stage = GameManager.stage - 1;
        
        // ステージ番号がカウントを超えたらエラー
        if(stage >= stagePrefabs.Count){
            Debug.Log("ERROR : StageManager > Start => stagePrefabs.Count");
            return;
        }

        // blocks を取得
        GameObject blocks = GameManager.Instance.blocks;

        // blocks の子オブジェクトを全件削除
        GameManager.Instance.AllDestroy(blocks);

        // ステージを生成
        GameManager.Instance.GenerateStage(stagePrefabs[stage]);
        
        // blocks の子オブジェクト active なものだけををカウント
        GameManager.Instance.CountBlocks();
    }    
}