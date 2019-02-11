using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
//[RequireComponent(typeof(Rigidbody2D))]
public class Jizz : MonoBehaviour
{
    float timeElapsed = Mathf.Epsilon;

    [HideInInspector]
    public float jizzLifetime, jizzSpeed;

    private Player_Doy playerScript;

    private bool firedRight = false, firedLeft = false;

    // Start is called before the first frame update
    void Start()
    {
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Doy>();

        print(playerScript);
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed >= jizzLifetime) Destroy(gameObject);

        if (!playerScript.isFacingRight && !firedRight)
        {
            firedLeft = true;
        }
        else if (playerScript.isFacingRight && !firedLeft)
        {
            firedRight = true;
        }

        if (firedRight) transform.Translate(Vector3.right * jizzSpeed / 10f);
        else if (firedLeft) transform.Translate(Vector3.left * jizzSpeed / 10f);
    }
}
