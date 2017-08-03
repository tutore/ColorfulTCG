using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBehaviourScript : MonoBehaviour {

    CardBehaviourScript card;

    public int health = 0;
    public TextMesh healthText;
    public int guard = 0;
    public TextMesh guardText;
    public int damage = 0;
    public TextMesh damageText;
    public bool isColl = true; // 충돌 가능 상태

    void Start()
    {
        if ( health <= 0 && guard <= 0 )
        {
            Invoke("DestroyObject", 5f);
        }
    }

    void FixedUpdate()
    {
        healthText.text = health.ToString();
        guardText.text = guard.ToString();
        damageText.text = damage.ToString();
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        // 오브젝트가 충돌하고 만약 충돌 가능 상태일 경우
        
        if (other.gameObject.tag == "Object" && isColl == true)
        {
            Debug.Log("obj collision");
            isColl = false; // 중복해서 충돌하지 않게 꺼준다
            ObjectBehaviourScript target = other.gameObject.GetComponent<ObjectBehaviourScript>();
            if (target.health > 0 && damage > 0) target.health -= damage;
            if (health > 0 && target.damage > 0) health -= target.damage;
            if (health <= 0 && guard <= 0 && this.gameObject != null) Invoke("DestroyObject", 1f);
            Invoke("CanColl", 10f);
        }
        else if (other.gameObject.tag == "Player" && isColl == true)
        {
            Debug.Log("player collision");
            isColl = false; // 중복해서 충돌하지 않게 꺼준다
            HeroBehaviourScript target = other.gameObject.GetComponent<HeroBehaviourScript>();
            if (target.health > 0 && damage > 0) target.health -= damage;
            if (health > 0 && target.damage > 0) health -= target.damage;
            if (health <= 0 && guard <= 0 && this.gameObject != null) Invoke("DestroyObject", 1f);
            Invoke("CanColl", 10f);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 오브젝트가 충돌하고 만약 충돌 가능 상태일 경우

        if (other.gameObject.tag == "Object" && isColl == true)
        {
            Debug.Log("obj trigger on");
            isColl = false; // 중복해서 충돌하지 않게 꺼준다
            ObjectBehaviourScript target = other.gameObject.GetComponent<ObjectBehaviourScript>();
            if (target.health > 0 && damage > 0) target.health -= damage;
            if (health > 0 && target.damage > 0) health -= target.damage;
            if (health <= 0 && guard <= 0 && this.gameObject != null) Invoke("DestroyObject", 1f);
            Invoke("CanColl", 10f);
        }
        else if (other.gameObject.tag == "Player" && isColl == true)
        {
            Debug.Log("player trigger on");
            isColl = false; // 중복해서 충돌하지 않게 꺼준다
            HeroBehaviourScript target = other.gameObject.GetComponent<HeroBehaviourScript>();
            if (target.health > 0 && damage > 0) target.health -= damage;
            if (health > 0 && target.damage > 0) health -= target.damage;
            if (health <= 0 && guard <= 0 && this.gameObject != null) Invoke("DestroyObject", 1f);
            Invoke("CanColl", 10f);
        }

    }

    void DestroyObject()
    {
        Destroy(this.gameObject);
    }

    void CanColl()
    {
        SetColl(true);
    }


    public void SetColl(bool coll)
    {
        isColl = coll;
    }
}
