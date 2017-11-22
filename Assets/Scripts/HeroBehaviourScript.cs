using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.tutore.ColofulTCG
{
    public class HeroBehaviourScript : MonoBehaviour
    {
        /*
        public enum Class { Fighter, Knight, Magician, Supporter, Hunter, Gardener };
        public Class heroClass = Class.Hunter;
        // 이동 가능 변수
        public bool classMove = true;
        public bool heroMove = true;
        */
        public PhotonView heroPhotonView;

        public PunTeams.Team team;

        // 스테이터스
        public GameObject image;
        public int health = 20;
        public TextMesh healthText;
        public int guard = 0;
        public TextMesh guardText;
        public int damage = 0;
        public TextMesh damageText;

        public int update_period = 0;
        public int update_health = 0, update_guard = 0, update_damage = 0;

        public bool basicMove = true;

        public Vector2 direction = Vector2.right;
        Rigidbody2D heroRigidBody;

        public Vector2 tmpPos;

        public bool isColl = true;

        // Use this for initialization
        void Start()
        {
            heroPhotonView = GetComponent<PhotonView>();
            heroRigidBody = GetComponent<Rigidbody2D>();

            heroPhotonView.RPC("UpdateHeroStatus", PhotonTargets.All, this.health, this.guard, this.damage);
            if (team == PunTeams.Team.blue)
            {
                direction = Vector2.right;
            }
            else if(team == PunTeams.Team.red) // 레드팀의 오브젝트면 오브젝트의 좌우를 뒤집는다
            {
                direction = Vector2.right * (-1);
                image.transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
                //수정 필요transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        void FixedUpdate()
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.identity, Time.deltaTime * 3); // 쓰러져도 다시 일어서게
        }

        [PunRPC]
        public void UpdateHeroStatus(int health, int guard, int damage)
        {
            this.health = health;
            this.guard = guard;
            this.damage = damage;
            healthText.text = health.ToString();
            guardText.text = guard.ToString();
            damageText.text = damage.ToString();
        }

        [PunRPC]
        public void SetHeroTeam(PunTeams.Team getTeam)
        {
            this.team = getTeam;
        }

        [PunRPC]
        public void TempStatusUpdate(int update_period, int update_health, int update_guard, int update_damage)
        {
            this.update_period = update_period;
            this.update_health = update_health;
            this.update_guard = update_guard;
            this.update_damage = update_damage;
            this.health += update_health;
            this.guard += update_guard;
            this.damage += update_damage;
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
                if (target.health > 0 && damage - target.guard > 0) target.health -= damage - target.guard;                    
                if (health < 0 && this.gameObject != null) Invoke("DestroyObject", 0.5f);

                // 계산 결과를 갱신
                target.objPhotonView.RPC("UpdateObjectStatus", PhotonTargets.All, target.health, target.guard, target.damage);
                
                Invoke("CanColl", 2f);
            }
            else if (other.gameObject.tag == "Player" && isColl == true)
            {
                Debug.Log("player collision");
                isColl = false; // 중복해서 충돌하지 않게 꺼준다
                HeroBehaviourScript target = other.gameObject.GetComponent<HeroBehaviourScript>();

                // 체력 계산
                if (target.health > 0 && damage - target.guard > 0) target.health -= damage - target.guard;

                // 계산 결과를 갱신
                target.heroPhotonView.RPC("UpdateHeroStatus", PhotonTargets.All, target.health, target.guard, target.damage);                
            }
        }

        void OnMouseDrag() // save temp position
        {
            tmpPos = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
        }

        void OnMouseUp() // move left or right according to distance from current position to temp position
        {
            Debug.Log("Mouse on");
            if (basicMove == true && System.Math.Abs(tmpPos.x - transform.position.x) > 1.0)
            {
                Debug.Log("basic move");
                heroRigidBody.AddForce(Vector2.up * 6000);
                if (transform.position.x > tmpPos.x) // move left
                {
                    heroRigidBody.AddForce(Vector2.right * (-6000));
                }
                else // move right
                {
                    heroRigidBody.AddForce(Vector2.right * 6000);
                }
                basicMove = false; // move once in a turn
                //StartCoroutine("DelayBasicMove");
            }
            /*
            else if (heroMove == true && System.Math.Abs(tmpPos.x - transform.position.x) < 1.0 && tmpPos.y - transform.position.y > 1.0)
            {
                Debug.Log("hero move");

                heroMove = false; // move once in a turn
                StartCoroutine("DelayHeroMove");
            }
            else if (classMove == true && System.Math.Abs(tmpPos.x - transform.position.x) < 1.0 && tmpPos.y - transform.position.y < -1.0)
            {
                Debug.Log("class move");

                classMove = false; // move once in a turn
                StartCoroutine("DelayClassMove");
            }
            */
            tmpPos = Vector2.zero;
        }
        /*
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
        */
    }

}