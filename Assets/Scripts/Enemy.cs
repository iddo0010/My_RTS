using UnityEditor;
using UnityEngine;


public class Enemy : UnitEngine
{
    private Animator anm;
    float searchRadius;

    private void Awake()
    {
        anm = GetComponent<Animator>();
        unit = new Unit();
        unit.hp = 120f;
        unit.dmg = 20f; //Change based on weapon
        searchRadius = 6f;


    }
    // Start is called before the first frame update
    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        #region Test Death
        //if(Input.GetKeyDown(KeyCode.Alpha4))
        //{
        //    Dead();
        //}
        #endregion
        
       //FindTarget(FindObjectOfType<UnitEngine>().gameObject)
       //{

       //}
    }

    public float RecieveDmg(float dmg)
    {
        unit.hp -= dmg;
        if (unit.hp <= 0)
            Dead();
        return unit.hp;
    }

    public void Dead()
    {
        GameObject body = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/EXPLORER - Stone Age/Prefabs/Avatars/Dead_Body_3A.prefab");
        Instantiate(body, transform.position, transform.rotation * Quaternion.Euler(0f, -180f, 0f)); //Allign body prefab to enemy look direction
        Destroy(gameObject);
        //anm.Play("Dead");
    }

    

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, searchRadius);
    }
}


