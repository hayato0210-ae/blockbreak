using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// public class GameManager : MonoBehaviour {
public class GameManager : SingletonMonoBehaviour<GameManager> {
    
    // public AudioClip test;
    // private AudioSource audioSource;

    // BGMの管理
    const string BGM_OUTGAME = "To_tomorrow";
    const string BGM_INGAME = "Star_Memories";
    
    // スコアの最大値
    private　const int SCORE_MAX = 99999;

    // 1秒間に加算される最大値
    private　const float SCORE_ADD = 50.0f;

    // アタックアイテムの発動時間
    private　const float ATTACK_MAX = 10.0f;

    // Debug用
    public bool isDebug = false;
    public int debugStage = 1;

    // ステージ番号の管理
    public static int stage = 1;
    
    // ゲームの状態管理用
    public enum STATE { PAUSE, GAME, CLEAR, GAMEOVER };

    // テキスト管理用
    public Text textStage;
    public Text textScore;

    // bar のゲームオブジェクト管理
    public GameObject bar;

    // ball のゲームオブジェクト管理
    public List<GameObject> balls;

    // blocks の親ゲームオブジェクト管理
    public GameObject blocks;

    // canvasResult の親ゲームオブジェクト管理
    public GameObject canvasStageClear;
    public GameObject canvasGameOver;

    // オーディオマネージャのPrefab
    public GameObject audioManagerPrefab;

    // ボールのプレファブ
    public GameObject ballPrefab;

    // ゲームの状態管理
    // 値を直接いじれると不具合の原因になるため、Inspectorに表示されないようにする
    [HideInInspector] 
    public STATE state = STATE.PAUSE;

    // スコア関連
    private float score = 0.0f;
    private int scoreMax = 0;

    // アタックの時間をカウント
    public bool isAttack = false;
    private float attackTime = 0.0f;
    
    void Awake() {
        // デバッグ中なら
        if(isDebug){
            // static 変数は Inspector に表示されないため別手法で値を代入
            stage = debugStage;
        }
    
        // audioSource = Camera.main.GetComponent<AudioSource>();
        
    }

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
            AudioManager.Instance.PlayBGM(BGM_INGAME);
        }

        // ボリュームを下げる
        AudioManager.Instance.BgmVolume(0.5f);

        // ステージを更新
        SetStage();
    
        // スコアを更新
        SetScore();

        // ボールの動きを停止
        for(int i=0; i<balls.Count; i++){
            balls[i].GetComponent<Ball>().Stop();
        }
        
        // canvasStageClear を非表示にする
        canvasStageClear.SetActive(false);

        // canvasGameOver を非表示にする
        canvasGameOver.SetActive(false);

    }

    // Update is called once per frame
    void Update() {
        
        // if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Fire1")) {
        //     audioSource.PlayOneShot(test);
        // }

        // スコアを加算するような演出にする
        score += SCORE_ADD * Time.deltaTime;
        if(score >= scoreMax){
            score = scoreMax;
        }

        // スコアを更新
        SetScore();

        // 各 State ごとの処理
        switch(state){
            case STATE.PAUSE:
                // スペースキー、各プラットフォームに応じたタップをしたら
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Fire1")) {
                      // ボールの動きを開始                    
                    for(int i=0; i<balls.Count; i++){
                        balls[i].GetComponent<Ball>().Move();
                    }
                    state = STATE.GAME;
                }                
                break;                
            case STATE.GAME:

                // テスト用
                if (Input.GetKeyDown(KeyCode.A)) {
                    scoreMax += 10;
                }   
                
                // debugモードなら
                if(isDebug){
                    // スペースキー、各プラットフォームに応じたタップをしたら
                    if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Fire1")) {
                        // ボールの動きを停止
                        for(int i=0; i<balls.Count; i++){
                            balls[i].GetComponent<Ball>().Stop();
                        }
                        state = STATE.PAUSE;
                        // クリアチェックを行わないように処理を一旦止める
                        return;
                    }
                }

                // アタック中なら
                if(isAttack){
                    // アタック中の時間を計測
                    attackTime += Time.deltaTime;
                    // アタックの終了時間になったら
                    if(attackTime >= ATTACK_MAX){
                        isAttack = false;
                        attackTime = 0;
                        
                        // ブロックの判定をもとに戻す
                        AllBlocksTrigger(false);     
                        
                        // Ball の TrailRenderer を白色に変更                        
                        for(int i=0; i<balls.Count; i++){
                            balls[i].GetComponent<Ball>().TrailGradient(Color.white);
                        }
                        //ball.GetComponent<Ball>().TrailGradient(Color.white);

                    }
                }

                // クリアチェック
                if(ClearCheck()){                    
                    // ステージクリア
                    StageClear();
                }

                // ゲームオーバーチェック
                if(CheckGameOver()){
                    // ゲームオーバー
                    GameOver();
                }    

                break;

            case STATE.CLEAR:
                // スペースキー、各プラットフォームに応じたタップをしたら
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Fire1")) {
                    // 次のステージへ
                    OnNext();
                }
                break;

            case STATE.GAMEOVER:
                // スペースキー、各プラットフォームに応じたタップをしたら
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Fire1")) {
                    // リトライ処理
                    OnRetry();
                }
                break;
        }

    }

    // ステージ番号を設定
    public void SetStage() {
        // テキストに変換
        textStage.text = "STAGE:" + stage.ToString();
    }

    // スコアを更新
    public void SetScore() {
        int scoreInt = (int)score;
        textScore.text = "SCORE:" + scoreInt.ToString();
    }

    // スコアを加算
    public void AddScore(int plus) {

        // スコアを加算して、最大値を超えた場合に制限        
        scoreMax += plus;
        if(scoreMax >= SCORE_MAX){
            scoreMax = SCORE_MAX;
        }

        // スコアを更新
        SetScore();
    }
    
    // クリアチェック
    private bool ClearCheck(){
        // ブロックが存在していなかったらクリア
        if(CountBlocks() == 0){
            return true;
        }
        return false;
    }
    
    // ステージクリア
    public void StageClear(){
        // canvasResult を表示にする
        canvasStageClear.SetActive(true);
        // State を CLEAR に変更する
        state = STATE.CLEAR;

        // ステージクリア数のセーブデータを保存        
        int clearMax = PlayerPrefs.GetInt ("StageClear", 0);
        if(stage > clearMax){
            PlayerPrefs.SetInt("StageClear", stage);
            PlayerPrefs.Save();        
        }

        // セーブデータを全削除できる
        // PlayerPrefs.DeleteAll();
        // PlayerPrefs.Save();
        // SEを鳴らす
        AudioManager.Instance.PlaySE("clear");
    }

    // ゲームオーバーかのチェック
    public bool CheckGameOver(){
        // ゲーム中でなければ処理をしない
        if(state != STATE.GAME){
            return false;
        }

        // ボールの数が 0 ならゲームオーバー判定
        if(CountBalls() == 0){
            return true;
        }        
        return false;
    }

    // ゲームオーバーの処理
    public void GameOver(){
        // canvasGameOver を表示にする
        // Unity 再生終了時のエラー回避 (MissingReferenceException)
        if(canvasGameOver){
            canvasGameOver.SetActive(true);
        }
 
        // State を GAMEOVER に変更する
        state = STATE.GAMEOVER;        
        // ボールの動きを停止                                        
        for(int i=0; i<balls.Count; i++){
            balls[i].GetComponent<Ball>().Stop();
        }        
        // SEを鳴らす
        AudioManager.Instance.PlaySE("gameover");
    }

    // ゲームオブジェクトを全件削除
    public void AllDestroy(GameObject obj){
        Destroy(obj);
        /*
        // 子オブジェクトだけを消す場合
        foreach ( Transform t in obj.transform ) {
            Destroy(t.gameObject);
        }
        */
    }

    // ステージを生成
    public void GenerateStage(GameObject obj){

        // Stage を生成
        GameObject blocks = Instantiate(
            obj,
            Vector3.zero,
            Quaternion.identity
        ) as GameObject;

        // blocks に生成した親を指定する
        GameManager.Instance.blocks = blocks;    
    }

    public int CountBlocks(){        
        // カウントを初期化
        int count = 0;        
        foreach ( Transform t in blocks.transform ) {
            // アクティブならブロック個数を追加
            if(t.gameObject.activeSelf){
                // 破壊できないオブジェクトなら次のループに移動、blocksCount は加算されない
                if(t.gameObject.GetComponent<Block>().type == Block.TYPE.GRAY){
                    continue;
                }
                count++;
            }
        }
        return count;
    }
    
    // Balls 子オブジェクトの active のものだけををカウント 
    public int CountBalls(){
        // ゲーム中でなければ処理をしない
        if(state != STATE.GAME){
            return -1;
        }
        // 仮用の配列を用意しておく ballsで初期化
        // balls を変えると temp も一緒に変わる  
        // List<string> temp = balls;   
        // balls を変えても temp は変わらない  
        // List<GameObject> temp = new List<GameObject>(balls);
        List<GameObject> temp = new List<GameObject>(balls);

        // ball 用リストを一旦初期化
        balls.Clear();

        // temp リスト分、ball に値を入れなおす
        for(int i=0; i<temp.Count; i++){
            // GameObjectが存在していたら
            if(temp[i]){
                balls.Add(temp[i]);
            }
        }

        // カウントを初期化
        int count = 0;        
        for(int i=0; i<balls.Count; i++){
            // アクティブならボール個数を追加
            if(balls[i].gameObject.activeSelf){
                count++;
            }
        }
        return count;
    }

    // ブロックすべてのあたり判定を変更
    public void AllBlocksTrigger(bool flag) {
        foreach ( Transform t in blocks.transform ) {
            t.gameObject.GetComponent<BoxCollider2D>().isTrigger = flag;
        }
    }

    // アタックの処理
    public void GetAttack() {        
        isAttack = true;
        attackTime = 0.0f;

        // ブロックの判定を Trigger に変更
        AllBlocksTrigger(true);

        // Ball の TrailRenderer を赤色に変更                          
        for(int i=0; i<balls.Count; i++){
            balls[i].GetComponent<Ball>().TrailGradient(Color.red);
        }
    }

    // ボールを獲得した処理
    public void GetBall(Vector3 pos){
        // ボールを を生成
        GameObject obj = Instantiate(
            ballPrefab,
            pos,
            Quaternion.identity
        ) as GameObject;

        // 移動させる
        obj.GetComponent<Ball>().Move();

        // アタックモードなら
        if(isAttack){
            // 新しく作るボールの TrailRenderer も変える
            obj.GetComponent<Ball>().TrailGradient(Color.red);
        }

        // blocks に生成した親を指定する
        GameManager.Instance.balls.Add(obj);
    }
    
    // バーを獲得した処理
    public void GetBar(){
        bar.GetComponent<Bar>().StartLong();
    }

    // 次のステージへ
    public void OnNext(){        
        // 次のステージへ
        GameManager.stage++;

        // 最大ステージを超えたらステージセレクト画面に戻る
        if(GameManager.stage >= StageManager.Instance.stagePrefabs.Count){
            GameManager.stage = 0;
            SceneManager.LoadScene("StageSelect");
        } else {
            SceneManager.LoadScene("Game");
        }

        // SEを鳴らす
        AudioManager.Instance.PlaySE("touch");
    }

    // リトライ処理
    public void OnRetry(){
        SceneManager.LoadScene("Game");
        // SEを鳴らす
        AudioManager.Instance.PlaySE("touch");
    }
    
    // Inspector の値が変更されたら呼ばれる
    private void OnValidate(){        
        // 範囲制限をかける
        if(isDebug){
            int max = StageManager.Instance.stagePrefabs.Count;
            debugStage = Mathf.Clamp(debugStage, 0, max);
        }
    }

    // ステージセレクト画面に遷移
    public void PressBack(){
        SceneManager.LoadScene("StageSelect");
        // BGMをとめる
        AudioManager.Instance.StopBGM();
        // SEを鳴らす
        AudioManager.Instance.PlaySE("touch");
    }
}
