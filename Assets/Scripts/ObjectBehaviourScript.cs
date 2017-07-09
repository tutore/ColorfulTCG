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

    ObjectBehaviourScript target;

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other != null)
        { 
            if (other.gameObject.tag == "Object")
            {
                Debug.Log("obj trigger on");
                target = other.gameObject.GetComponent<ObjectBehaviourScript>();
                if (target.health > 0 && damage > 0) target.health -= damage;
                if (health > 0 && target.damage > 0) health -= target.damage;
            }
        }
    }

    void FixedUpdate()
    {
        healthText.text = health.ToString();
        guardText.text = guard.ToString();
        damageText.text = damage.ToString();
    }
}
