using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonStage : MonoBehaviour {

    public GameObject textNumber;
    public int stage = 0;

    // 各種情報をセット
    public void SetStage(bool flag, int num){         
        // ステージ番号の変更
        stage = num;

        // 選択できるかの状態変更
        GetComponent<Button>().interactable = flag;
        
        // テキストの番号を変更
        textNumber.GetComponent<Text>().text = stage.ToString();
    }

    // ボタンをタップしたら
    public void PressButton(){     
        // StageSelectManager の PressStage にステージ番号を渡す
        Camera.main.GetComponent<StageSelectManager>().PressStage(stage);
    }   
    
}
