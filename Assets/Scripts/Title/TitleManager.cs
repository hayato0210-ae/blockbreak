using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour {
    
    // BGMの管理
    const string BGM_OUTGAME = "To_tomorrow";

    // オーディオマネージャのPrefab
    public GameObject audioManagerPrefab;

    // Start is called before the first frame update
    void Start() {     
        // オーディオマネージャーが存在するかの確認
        if(!AudioManager.Instance){ 
            // 存在しなければ生成
            Instantiate(audioManagerPrefab, Vector3.zero, Quaternion.identity);
            Debug.Log("AudioManager.Instanceを生成します");
        }
   
        // BGMがなっていなければ、ゲーム用のBGMを鳴らす
        if(!AudioManager.Instance.IsPlayingBGM()){
            AudioManager.Instance.PlayBGM(BGM_OUTGAME);
        }
        
        // ボリュームを下げる
        AudioManager.Instance.BgmVolume(0.5f);
    }

    // Update is called once per frame
    void Update() {
        
    }
    
    // ステージセレクトへ遷移
    public void PressStart(){
        SceneManager.LoadScene("StageSelect");
        // SEを鳴らす
        AudioManager.Instance.PlaySE("touch");
    }
    
}
