using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreItem : MonoBehaviour {

    [Header ("加算スコア")] public int point;

    private string playerTag = "Player";

    private void OnTriggerEnter2D (Collider2D collision) {
        if (collision.tag == playerTag) {
            if (GManager.instance != null) {
                GManager.instance.score += point;
                Destroy (this.gameObject);
            }
        }
    }
}