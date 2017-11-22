using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.tutore.ColofulTCG
{
    public class ObjectBehaviourScript : MonoBehaviour
    {
        CardBehaviourScript card;

        public enum ObjectElement { Null, Fire, Earth, Water, Wood };
        public ObjectElement objectElement;

        public GameObject image;
        public int health = 0;
        public TextMesh healthText;
        public int guard = 0;
        public TextMesh guardText;
        public int damage = 0;
        public TextMesh damageText;
        public bool isColl = true; // 충돌 가능 상태

        public bool isHealer = false;

        public PhotonView objPhotonView;

        void Start()
        {
            objPhotonView = GetComponent<PhotonView>();
            objPhotonView.RPC("UpdateObjectStatus", PhotonTargets.All, this.health, this.guard, this.damage);
            /*
             * real time function
            if (health <= 0 && guard <= 0)
            {
                Invoke("DestroyObject", 5f);
            }
            */
        }

        void FixedUpdate()
        {

        }

        void OnCollisionEnter2D(Collision2D other)
        {
            // 오브젝트가 충돌하고 만약 충돌 가능 상태일 경우

            if (other.gameObject.tag == "Object" && isColl == true)
            {
                Debug.Log("obj collision");
                isColl = false; // 중복해서 충돌하지 않게 꺼준다
                ObjectBehaviourScript target = other.gameObject.GetComponent<ObjectBehaviourScript>();

                // 체력 계산
                if (objectElement != ObjectElement.Fire || target.objectElement != ObjectElement.Earth) // 불 속성은 땅 속성에 데미지를 주지 못한다
                {
                    if (target.health > 0 && damage - target.guard > 0) target.health -= damage - target.guard;
                    if (objectElement == ObjectElement.Water && target.objectElement == ObjectElement.Earth) target.guard -= damage; // 물 속성은 땅 속성의 방어를 깎는다
                }
                if (health < 0 && this.gameObject != null) Invoke("DestroyObject", 0.5f);
                if (isHealer == true && target.health > 0) target.health++; 

                // 계산 결과를 갱신
                target.objPhotonView.RPC("UpdateObjectStatus", PhotonTargets.All, target.health, target.guard, target.damage);

                // 체력도 방어도 0인 오브젝트가 무언가에 부딪힐 경우 1초 뒤 사라진다
                if (health <= 0 && guard <= 0 && this.gameObject != null) Invoke("DestroyObject", 0.5f);
                objPhotonView.RPC("UpdateObjectStatus", PhotonTargets.All, health, guard, damage);
                Invoke("CanColl", 2f);
            }
            else if (other.gameObject.tag == "Player" && isColl == true)
            {
                Debug.Log("player collision");
                isColl = false; // 중복해서 충돌하지 않게 꺼준다
                HeroBehaviourScript target = other.gameObject.GetComponent<HeroBehaviourScript>();

                // 체력 계산
                if (target.health > 0 && damage - target.guard > 0) target.health -= damage - target.guard;
                if (isHealer && target.team == PhotonNetwork.player.GetTeam()) target.health++;

                // 계산 결과를 갱신
                target.heroPhotonView.RPC("UpdateHeroStatus", PhotonTargets.All, target.health, target.guard, target.damage);

                // 체력도 방어도 0인 오브젝트가 무언가에 부딪힐 경우 1초 뒤 사라진다
                if (health <= 0 && guard <= 0 && this.gameObject != null) Invoke("DestroyObject", 0.5f);
                objPhotonView.RPC("UpdateObjectStatus", PhotonTargets.All, health, guard, damage);
                Invoke("CanColl", 2f);
            }
            else if (other.gameObject.tag == "Ground" && isColl == true)
            {
                Debug.Log("ground collision");
                isColl = false; // 중복해서 충돌하지 않게 꺼준다

                // 체력도 방어도 0인 오브젝트가 무언가에 부딪힐 경우 1초 뒤 사라진다
                if (health <= 0 && guard <= 0 && this.gameObject != null) Invoke("DestroyObject", 0.5f);
                objPhotonView.RPC("UpdateObjectStatus", PhotonTargets.All, health, guard, damage);
                Invoke("CanColl", 2f);
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

                // 체력 계산
                if (objectElement != ObjectElement.Fire || target.objectElement != ObjectElement.Earth) // 불 속성은 땅 속성에 데미지를 주지 못한다
                {
                    if (target.health > 0 && damage - target.guard > 0) target.health -= damage - target.guard;
                    if (objectElement == ObjectElement.Water && target.objectElement == ObjectElement.Earth) target.guard -= damage; // 물 속성은 땅 속성의 방어를 깎는다
                }

                // 계산 결과를 갱신
                target.objPhotonView.RPC("UpdateObjectStatus", PhotonTargets.All, target.health, target.guard, target.damage);

                // 체력도 방어도 0인 오브젝트가 무언가에 부딪힐 경우 1초 뒤 사라진다
                if (health <= 0 && guard <= 0 && this.gameObject != null) Invoke("DestroyObject", 0.5f);
                objPhotonView.RPC("UpdateObjectStatus", PhotonTargets.All, health, guard, damage);
                Invoke("CanColl", 2f);
            }
            else if (other.gameObject.tag == "Player" && isColl == true)
            {
                Debug.Log("player trigger on");
                isColl = false; // 중복해서 충돌하지 않게 꺼준다
                HeroBehaviourScript target = other.gameObject.GetComponent<HeroBehaviourScript>();

                // 체력 계산
                if (target.health > 0 && damage - target.guard > 0) target.health -= damage - target.guard;
                

                // 계산 결과를 갱신
                target.heroPhotonView.RPC("UpdateHeroStatus", PhotonTargets.All, target.health, target.guard, target.damage);

                // 체력도 방어도 0인 오브젝트가 무언가에 부딪힐 경우 1초 뒤 사라진다
                if (health <= 0 && guard <= 0 && this.gameObject != null) Invoke("DestroyObject", 0.5f);
                objPhotonView.RPC("UpdateObjectStatus", PhotonTargets.All, health, guard, damage);
                Invoke("CanColl", 2f);
            }

        }

        [PunRPC]
        public void UpdateObjectStatus(int health, int guard, int damage)
        {
            this.health = health;
            this.guard = guard;
            this.damage = damage;
            healthText.text = health.ToString();
            guardText.text = guard.ToString();
            damageText.text = damage.ToString();
        }
        
        public void DestroyObject()
        {
            PhotonNetwork.Destroy(this.gameObject);
        }

        // 부딪칠 때 여러번 충돌 판정 되지 않게 막음
        void CanColl()
        {
            objPhotonView.RPC("SetColl", PhotonTargets.All, true);
        }

        [PunRPC]
        public void SetColl(bool coll)
        {
            isColl = coll;
        }
    }
}