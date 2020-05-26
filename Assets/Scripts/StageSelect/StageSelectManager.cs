using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelectManager : MonoBehaviour {
    
    // BGMの管理
    //const string BGM_OUTGAME = "To_tomorrow";

    // ステージを生成する個数
    public int stageMax = 20;

    // オーディオマネージャのPrefab
    //public GameObject audioManagerPrefab;

    // ボタンのプレファブ
    public GameObject buttonPrefab;

    // ボタンを生成する場所
    public GameObject contents;

    // Start is called before the first frame update
    void Start() {        
        
        // オーディオマネージャーが存在するかの確認
        //if(!AudioManager.Instance){ 
            // 存在しなければ生成
           // Instantiate(audioManagerPrefab, Vector3.zero, Quaternion.identity);
            //Debug.Log("AudioManager.Instanceを生成します");
        //}
        
        // BGMがなっていなければ、ゲーム用のBGMを鳴らす
        //if(!AudioManager.Instance.IsPlayingBGM()){
         //   AudioManager.Instance.PlayBGM(BGM_OUTGAME);
        //}
        
        // ボリュームを下げる
        //AudioManager.Instance.BgmVolume(0.5f);

        // ステージクリア数
        int clearMax = PlayerPrefs.GetInt ("StageClear", 0);
        
        // 子オブジェクトを全削除
        foreach ( Transform n in contents.transform ) {
            GameObject.Destroy(n.gameObject);
        }

         // stageMax 分 buttonPrefab を生成
        for(int i=0; i<stageMax; i++){
            
            // contents 内に buttonPrefab を生成
            GameObject obj = Instantiate(
                buttonPrefab,
                Vector3.zero,
                Quaternion.identity,
                contents.transform
            ) as GameObject;

            // ボタンの情報を変更
            ButtonStage button = obj.GetComponent<ButtonStage>();

            // ステージをセット
            int stage = i+1;

            // クリアを確認
            bool flag = false;
            
            // クリアしているステージより下なら選択できるようにする
            if(i <= clearMax){
                flag = true;
            }

            // ボタンを設定
            // Debug.Log(stage);
            button.SetStage(flag, stage);
        }

    }

    // Update is called once per frame
    void Update() {
        
    }
    
    // タイトル画面に遷移
    public void PressBack(){
        SceneManager.LoadScene("Title");

        // SEを鳴らす
        //AudioManager.Instance.PlaySE("touch");
    }

    // ゲーム画面へ遷移
    public void PressStage(int number){
        GameManager.stage = number;
        SceneManager.LoadScene("Game");

        // BGMをとめる
        //AudioManager.Instance.StopBGM();

        // SEを鳴らす
        //AudioManager.Instance.PlaySE("touch");
    }

}
