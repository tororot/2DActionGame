using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCollision : MonoBehaviour {
    [Header ("これを踏んだ時のプレイヤーが跳ねる高さ")] public float boundHeight;

    // このオブジェクトをプレイヤーが踏んだかどうか
    [HideInInspector] public bool playerStepOn;
    [Header ("アイテム取得時に鳴らすSE")] public AudioClip itemSE;

}