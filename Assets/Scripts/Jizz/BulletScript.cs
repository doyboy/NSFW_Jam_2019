using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    float t = 0.0f;
    float direction = 1.0f;
    public float speed = 60.0f;
    [SerializeField]
    Player_MoToKo player;

    public SpriteRenderer sRenderer;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        sRenderer.flipX = !player.isFacingRight;
        if (player.isFacingRight){
            direction = 1.0f;
        } else {
            direction = -1.0f;
        }
    }


    void Update()
    {
        Vector3 newPos = new Vector3(direction*speed * Time.deltaTime, 0, 0);
        transform.Translate(newPos);
    }
}
