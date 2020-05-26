using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : SingletonMonoBehaviour<AudioManager> {

    public List<AudioClip> bgmList;
    public List<AudioClip> seList;

    // AudioSource を BGM と SE で2つ用意する
    private AudioSource bgmAudioSource;
    private AudioSource seAudioSource;

    private void Awake (){
	
        // インスタンスを保持していなければ破棄
    	if (this != Instance) {
			Destroy (this);
			return;
		}

        // AudioSource を BGM と SE で2つ用意する
        bgmAudioSource = gameObject.AddComponent<AudioSource>();
        seAudioSource = gameObject.AddComponent<AudioSource>();

        // BGM はループするように設定
        bgmAudioSource.loop = true;

        // シーン切り替え時に破棄しないようにする
		DontDestroyOnLoad (this.gameObject);
    }
    
    // ファイル名から BGM が登録している番号を探す
    private int GetBgmIndex(string bgmName){
        int index = -1;        
        // 引数と同じファイル名のものがあるかを検索
        for(int i=0; i<bgmList.Count; i++){
            if(bgmName == bgmList[i].name){
                index = i;
            }
        }
        // 見つからなければエラーを出力
        if(index == -1){
            Debug.Log("AudioManager > GetBgmIndex : " + bgmName + " は設定されていません。");
        }
        return index;
    }

    // BGMを鳴らす
    public void PlayBGM(string bgmName) {         
        // 引数と同じファイル名のものがあるかを検索
        int i = GetBgmIndex(bgmName);
        // i の値が0以上なら存在している
        if(i>= 0){
            bgmAudioSource.clip = bgmList[i];
            bgmAudioSource.Play();
        }
    }

    // BGMをとめる
    public void StopBGM() {        
        bgmAudioSource.Stop();  
		bgmAudioSource.clip = null;    
    }

    // BGMのボリュームを変える
    public void BgmVolume(float volume) {
        // 0~1までの範囲で設定できるようにする
        float vol = Mathf.Clamp(volume, 0, 1.0f);
        bgmAudioSource.volume = vol;
    }	
    
    // BGMが再生されているかをチェック
	public bool IsPlayingBGM() {
		return bgmAudioSource.isPlaying;
	}

    // ファイル名から SE が登録している番号を探す
    private int GetSeIndex(string seName){
        int index = -1;        
        // 引数と同じファイル名のものがあるかを検索
        for(int i=0; i<seList.Count; i++){
            if(seName == seList[i].name){
                index = i;
            }
        }
        // 見つからなければエラーを出力
        if(index == -1){
            Debug.Log("AudioManager > GetSeIndex : " + seName + " は設定されていません。");
        }
        return index;
    }

    // SEを鳴らす
    public void PlaySE (string seName){    
        int i = GetSeIndex(seName);
        if(i>= 0){
            seAudioSource.PlayOneShot(seList[i]);
        }
    }
    
	// SEをとめる
	public void StopSE() {
        seAudioSource.Stop();  
		seAudioSource.clip = null;   
	}

    // SEのボリュームを変える
    public void SeVolume(float volume) {
        // 0~1までの範囲で設定できるようにする
        float vol = Mathf.Clamp(volume, 0, 1.0f);
        seAudioSource.volume = vol;
    }
    
}
