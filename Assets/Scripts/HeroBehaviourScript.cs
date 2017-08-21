using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroBehaviourScript : MonoBehaviour {

    public enum Class { Fighter, Knight, Magician, Supporter, Hunter, Gardener};
    public Class heroClass = Class.Hunter;

    public enum Team { My, AI };
    public Team team = Team.My;

    // 스테이터스
    public int health = 20;
    public TextMesh healthText;
    public int guard = 0;
    public TextMesh guardText;
    public int damage = 0;
    public TextMesh damageText;

    // 이동 가능 변수
    public bool basicMove = true;
    public bool classMove = true;
    public bool heroMove = true;

    public Vector2 direction = Vector2.right;
    Rigidbody2D heroRigidBody;

    public Vector2 tmpPos;

    // Use this for initialization
    void Start ()
    {
        heroRigidBody = GetComponent<Rigidbody2D>();
        
        if (team == Team.My) direction = Vector2.right;
        else direction = Vector2.right * (-1);
    }

    // Update is called once per frame
    void Update () {

    }

    void FixedUpdate ()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.identity, Time.deltaTime * 3); // 쓰러져도 다시 일어서게
        healthText.text = health.ToString();
        guardText.text = guard.ToString();
        damageText.text = damage.ToString();
    }
    
    void OnMouseDrag () // save temp position
    {
        tmpPos = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
    }

    void OnMouseUp () // move left or right according to distance from current position to temp position
    {
        Debug.Log("Mouse on");
        if ( heroMove == true && System.Math.Abs(tmpPos.x - transform.position.x) < 1.0  && tmpPos.y - transform.position.y > 1.0 )
        {
            Debug.Log("hero move");

            heroMove = false; // move once in a turn
            StartCoroutine("DelayHeroMove");
        }
        else if ( classMove == true && System.Math.Abs(tmpPos.x - transform.position.x) < 1.0 && tmpPos.y - transform.position.y < -1.0 )
        {
            Debug.Log("class move");

            classMove = false; // move once in a turn
            StartCoroutine("DelayClassMove");
        }
        else if ( basicMove == true && System.Math.Abs(tmpPos.x - transform.position.x) > 1.0 )
        {
            Debug.Log("basic move");
            heroRigidBody.AddForce(Vector2.up * 6000);
            if ( transform.position.x > tmpPos.x ) // move left
            {
                heroRigidBody.AddForce(Vector2.right * (-6000));
            }
            else // move right
            {
                heroRigidBody.AddForce(Vector2.right * 6000);
            }
            basicMove = false; // move once in a turn
            StartCoroutine("DelayBasicMove");
        }
        tmpPos = Vector2.zero;
    }

    IEnumerator DelayHeroMove()
    {
        yield return new WaitForSeconds(20f);
        heroMove = true;
    }
    IEnumerator DelayClassMove()
    {
        yield return new WaitForSeconds(10f);
        classMove = true;
    }
    IEnumerator DelayBasicMove()
    {
        yield return new WaitForSeconds(10f);
        basicMove = true;
    }
}
