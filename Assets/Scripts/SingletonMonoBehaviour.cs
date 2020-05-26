using UnityEngine;
 
public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour {

    // instance 保持用
    private static T instance; 
    
    // 外部からアクセス用
    public static T Instance {
        get {
            if (instance == null) {
                // シーン内に同じクラス型のスクリプトがあるかを探す
                instance = (T)FindObjectOfType(typeof(T));
 
                // instance が空ならシーンに同じクラス型のスクリプトがない
                if (instance == null) {
                    Debug.Log(typeof(T) + "が見つかりません");
                }
            }
 
            // インスタンスを返す
            return instance;
        }
    }
 
}
