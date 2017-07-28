using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroBehaviourScript : MonoBehaviour {

    public int health = 20;
    public TextMesh healthText;
    public int guard = 0;
    public TextMesh guardText;
    public int damage = 0;
    public TextMesh damageText;
    public bool canMove = true;

    public enum Team { My, AI };
    public Team team = Team.My;

    public Vector2 tmpPos;
    public Vector3 newPos;

    // Use this for initialization
    void Start ()
    {
        newPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }

    // Update is called once per frame
    void Update () {

    }

    void FixedUpdate ()
    {
        transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * 3);
        healthText.text = health.ToString();
        guardText.text = guard.ToString();
        damageText.text = damage.ToString();
    }
    
    void OnMouseDrag () // save temp position
    {
        if ( canMove == true )
        {
            tmpPos = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
        }
    }

    void OnMouseUp () // move left or right according to distance from current position to temp position
    {
        Debug.Log("mouse on");
        if ( team == Team.My && canMove == true && System.Math.Abs(tmpPos.x - transform.position.x) > 1.0 )
        {
            Debug.Log("success");
            if ( transform.position.x > tmpPos.x ) // move left
            {
                newPos = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z);
            }
            else // move right
            {
                newPos = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
            }
            canMove = false; // move once in a turn
        }
    }
}
