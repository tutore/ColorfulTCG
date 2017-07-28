using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBehaviourScript : MonoBehaviour {

    public int health = 0;
    public TextMesh healthText;
    public int guard = 0;
    public TextMesh guardText;
    public int damage = 0;
    public TextMesh damageText;
    bool isColl = true; // 충돌 가능 상태

    CardBehaviourScript card;

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other != null && isColl == true) // 오브젝트가 충돌하고 만약 충돌 가능 상태일 경우
        {
            if (health <= 0 && guard <= 0)
                Destroy(this.gameObject);
            isColl = false; // 중복해서 충돌하지 않게 꺼준다
            if (other.gameObject.tag == "Object")
            {
                Debug.Log("obj trigger on");
                ObjectBehaviourScript target = other.gameObject.GetComponent<ObjectBehaviourScript>();
                if (target.health > 0 && damage > 0) target.health -= damage;
                if (health > 0 && target.damage > 0) health -= target.damage;
            }
            else if (other.gameObject.tag == "Player")
            {
                Debug.Log("player trigger on");
                HeroBehaviourScript target = other.gameObject.GetComponent<HeroBehaviourScript>();
                if (target.health > 0 && damage > 0) target.health -= damage;
                if (health > 0 && target.damage > 0) health -= target.damage;
            }
            //StartCoroutine("DestroyObject");
        }
    }

    IEnumerator DestroyObject()
    {
        if (health <= 0 && guard <= 0)
        {
            yield return new WaitForSeconds(2f);
            Destroy(this.gameObject);
        }
    }

    void FixedUpdate()
    {
        healthText.text = health.ToString();
        guardText.text = guard.ToString();
        damageText.text = damage.ToString();
    }

    public void SetColl(bool coll)
    {
        isColl = coll;
    }
}
