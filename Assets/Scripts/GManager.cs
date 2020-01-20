using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GManager : MonoBehaviour {

    public static GManager instance = null;

    public int score = 0;
    public int stageNum = 1;
    public int continueNum = 0;
    public int heartNum = 3;
    public bool isGameOver = false;
    public int defaultHeartNum = 3;

    private void Awake () {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad (this.gameObject);
        } else {
            Destroy (this.gameObject);
        }
    }

    // 残機を１つ増やす
    public void AddHeartNum () {
        if (heartNum < 99) {
            ++heartNum;
        }
    }

    // 残機を１つ減らす
    public void SubHeartNum () {
        if (heartNum > 0) {
            --heartNum;
        } else {
            isGameOver = true;
        }
    }

    // 最初から始める時の処理
    public void RetryGame () {
        isGameOver = false;
        heartNum = defaultHeartNum;
        score = 0;
        stageNum = 1;
        continueNum = 0;
    }
}