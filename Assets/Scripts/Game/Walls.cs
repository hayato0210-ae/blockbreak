using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walls : MonoBehaviour {

    // 各 Wall の設定
    public GameObject wallTop;
    public GameObject wallBottom;
    public GameObject wallLeft;
    public GameObject wallRight;
  
    // 画面サイズを取得
    private Vector2 screen = Vector3.zero;

    // オブジェクトのサイズ取得
    private Vector3 size = Vector3.zero;    

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
        // Debug.Log(screen);

        // Wall 位置の設定
        SetWall();

        // debug 状態でなければ下の壁を非表示にする
        //if(!gameManager.isDebug){
        //    wallBottom.SetActive(false);
        //}
        // debug 状態でなければ下の壁を非表示にする
        if(!GameManager.Instance.isDebug){
            wallBottom.SetActive(false);
        }
    }

    // Wall 位置の設定
    public void SetWall() { 
        // 上の壁
        Vector3 top = new Vector3(0.0f, screen.y / 2, 0.0f);
        top.y += wallTop.GetComponent<SpriteRenderer>().bounds.size.y / 2;              
        wallTop.transform.localPosition = top;

        // 下の壁
        Vector3 bottom = new Vector3(0.0f, -screen.y / 2, 0.0f);
        bottom.y -= wallBottom.GetComponent<SpriteRenderer>().bounds.size.y / 2;              
        wallBottom.transform.localPosition = bottom;

        // 左の壁
        Vector3 left = new Vector3(-screen.x / 2, 0.0f, 0.0f);
        left.x -= wallLeft.GetComponent<SpriteRenderer>().bounds.size.x / 2;              
        wallLeft.transform.localPosition = left;

        // 右の壁
        Vector3 right = new Vector3(screen.x / 2, 0.0f, 0.0f);
        right.x += wallRight.GetComponent<SpriteRenderer>().bounds.size.x / 2;              
        wallRight.transform.localPosition = right;
    }
    
}
