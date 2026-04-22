using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using System.Collections.Generic;

public enum CharState
{
    Idle,
    Walk,
    WalkToEnemy,
    Attack,
    WalkToMagicCast,
    MagicCast,
    Hit,
    Die
}

public abstract class Character : MonoBehaviour
{
    [SerializeField]
    protected List<Magic> magicSkills = new List<Magic>();
    public List<Magic> MagicSkills
    {  get { return magicSkills; } set { magicSkills = value; } }

    [SerializeField]
    protected Magic curMagicCast =null;
    public Magic CurMagicCast
    {  get { return curMagicCast; } set { curMagicCast = value; } }

    [SerializeField]
    protected bool isMagicMode = false;
    public bool IsMagicMode 
        { get { return isMagicMode; } set { isMagicMode = value; } }

    [Header ("Inventory")]

    [SerializeField]
    protected Item[] inventoryItems;
    public Item[] InventoryItems
    { get { return inventoryItems; } set { inventoryItems = value; } }

    [SerializeField]
    protected Item mainWeapon;
    public Item MainWeapon
    { get {  return mainWeapon; } set { mainWeapon = value; } }

    [SerializeField]
    protected Item shield;
    public Item Shield
    { get { return shield; } set { shield = value; } }

    [SerializeField]
    protected Transform shieldHand;

    [SerializeField]
    protected GameObject shielaobj;

    [SerializeField]
    protected int defensepower = 0;

[SerializeField]
protected Transform weaponHand;

[SerializeField]
protected GameObject weaponObj;

[SerializeField] 
protected int attackPower = 0;


    protected VFXManager vfxManager;
    protected UIManager uiManager;
    protected InventoryManager invManager;

    [SerializeField]
    protected int curHP = 10;
    public int CurHP { get { return curHP; } }

    [SerializeField]
    protected int maxHp = 100;
    public int MaxHP { get { return maxHp; } }

    [SerializeField]
    protected Character curCharTarget;
    public Character CurCharTarget { get { return curCharTarget; } set { curCharTarget = value; } } 
    public float AttackRange {get { return attackRange; }}

    [SerializeField]
    protected float attackRange = 2f;
    [SerializeField]
    protected int attackDamage = 3;

    [SerializeField]
    protected float attackCoolDown = 2f;

    [SerializeField]
    protected float attackTimer = 0;

    [SerializeField]
    protected float findingRange = 20f;
    public float FindingRange { get { return findingRange; }}



    protected NavMeshAgent navAgent;

    protected Animator anim;

    public Animator Anim { get { return anim; } }

    [SerializeField]
    protected GameObject ringSelection;
    public GameObject RingSelection { get { return ringSelection; } }

    [SerializeField]
    protected CharState state;

    public CharState State { get { return state; } }

    //public object CurCharTarget { get; internal set; }

    private void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public void Recover(int n)
    {
        curHP += n;

        if (curHP > maxHp)
            curHP = maxHp;
    }
    public void charInit(VFXManager vfxM, UIManager uiM, InventoryManager invM)
    {
        vfxManager = vfxM;
        uiManager = uiM;
        invManager = invM;

        inventoryItems = new Item[InventoryManager.MAXSLOT];
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetState(CharState s)
    {
        state = s;

        if (state == CharState.Idle)
        {
            navAgent.isStopped = true;
            navAgent.ResetPath();
        }
    }

    public void WalkToPosition(Vector3 dest)
    {
        if (navAgent != null)
        {
            navAgent.SetDestination(dest);
            navAgent.isStopped = false;

        }

        SetState(CharState.Walk);
    }

    protected void WalkUpdate()
    {
        float distance = Vector3.Distance(transform.position, navAgent.destination);
        Debug.Log(distance);

        if (distance <= navAgent.stoppingDistance)
        {
            SetState(CharState.Idle);

        }
    }

    public void ToggleRingSelection(bool flag)
    {
        ringSelection.SetActive(flag);
    }

    public void ToAttackCharacter(Character target)
    {
        if (curHP <= 0 || state == CharState.Die)
            return;
        curCharTarget = target;

        navAgent.SetDestination(target.transform.position);
        navAgent.isStopped = false;

        if(isMagicMode)
            SetState(CharState.WalkToMagicCast);
        else
        SetState(CharState.WalkToEnemy);
    }

    protected void WalkToEnemyUpdate()
    {
        if (curCharTarget == null)
        {
            SetState(CharState.Idle);
            return;
        }

        navAgent.SetDestination(curCharTarget.transform.position);

        float distance = Vector3.Distance
            (transform.position, curCharTarget.transform.position);

        if (distance <= attackRange)
        {
            SetState(CharState.Attack);
            Attack();
        }
    }

    protected void Attack()
    {
        transform.LookAt(transform.position);

        anim.SetTrigger("Attack");

        AttackLogic();
    }

    protected void AttackUpdate()
    {
        if (curCharTarget == null)
            return;

        if (curCharTarget.CurHP <= 0)
        {

            SetState(CharState.Idle);
            return;
        }
        navAgent.isStopped = true;
        attackTimer += Time.deltaTime;
        if (attackTimer >= attackCoolDown)
        {
            attackTimer = 0f;
            Attack();
        }

        float distance = Vector3.Distance(transform.position, curCharTarget.transform.position);

        if (distance > attackRange)
        {
            SetState(CharState.WalkToEnemy);
            navAgent.SetDestination(curCharTarget.transform.position);
            navAgent.isStopped = false;
        }


    }

    protected virtual IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

    protected virtual void Die()
    {

        navAgent.isStopped = true;
        SetState(CharState.Die);

        anim.SetTrigger("Die");

        invManager.SpawnDropInventory(inventoryItems, transform.position);

        StartCoroutine(DestroyObject());
    }

    public void ReceiveDamage(int damage)
    {
        if (curHP <= 0 || state == CharState.Die)
            return;

        int damageAfter = damage - defensepower;

        if(damageAfter < 0)
            damageAfter = 0;

        curHP -= damageAfter;

        curHP -= damage;
        if(curHP <= 0)
        {
            curHP = 0;
            Die();
        }
    }

    protected void AttackLogic()
    {
        Character target = curCharTarget.GetComponent<Character>();

        if (target != null) 
            target.ReceiveDamage(attackDamage);
    }

    public bool IsmyEnemy(string targetTag)
    {
        string myTag = gameObject.tag;

        if ((myTag == "Hero" ||  myTag == "Player") && targetTag == "Enemy")
        return true;

        if (myTag == "Enemy" && (targetTag == "Hero" ||  targetTag == "Player"))
        return true;

        return false;
                
    }

    protected void MagicCastlogic(Magic magic)
    {
        Character target = curCharTarget.GetComponent<Character>();

        if(target != null)
            target.ReceiveDamage(magic.Power);
    }

    private IEnumerator ShootMagicCast(Magic curMagicCast)
    {
        Vector3 offsetPos = transform.position + new Vector3(0, 1f, 0);
        Vector3 targetPos = curCharTarget.transform.position + new Vector3(0, 1f, 0);

        if (vfxManager != null)
            vfxManager.ShootMagic(curMagicCast.ShootID, 
                offsetPos,
                targetPos,
                curMagicCast.ShootTime);

        yield return new WaitForSeconds(curMagicCast.ShootTime);

        MagicCastlogic(curMagicCast);
        isMagicMode = false;

        SetState(CharState.Idle);
        if (uiManager != null)
            uiManager.IsonCurToggleMagic(false);
    }

    private IEnumerator LoadMagicCast(Magic curMagicCast)
    {
        Vector3 offsetPos = transform.position + new Vector3(0, 1f, 0);

        if (vfxManager != null)
            vfxManager.LoadMagic(curMagicCast.LoadID,
                offsetPos,
                curMagicCast.LoadTime);

        yield return new WaitForSeconds(curMagicCast.LoadTime);

        StartCoroutine(ShootMagicCast(curMagicCast));
    }

    private void  MagicCast(Magic curMagicCast)
    {
        transform.LookAt(CurCharTarget.transform);
        anim.SetTrigger("MagicAttack");

        StartCoroutine(LoadMagicCast(curMagicCast));
    }

    protected void WalkToMagicCastUpdate()
    {
        if (curCharTarget == null || curMagicCast == null)
        {
            SetState(CharState.Idle);
            return;
        }

        navAgent.SetDestination(curCharTarget.transform.position);

        float distance = Vector3.Distance(transform.position,
            curCharTarget.transform.position);

        if(distance <= curMagicCast.Range)
        {
            navAgent.isStopped =true;
            SetState(CharState.MagicCast);

            MagicCast(curMagicCast);
        }
    }

    public void EquipShield(Item item)
    {
        shielaobj = Instantiate(invManager.itemPrefab[item.PrefabID], shieldHand);

        shielaobj.transform.localPosition = new Vector3(-8.5f, -4f, 3f);
        shielaobj.transform.Rotate(-90f, 0f , 180f, Space.Self);

        defensepower += item.Power;
        shield = item;
    }

    public void UnEquipShieid()
    {
        if (shielaobj != null)
        {
            defensepower -= shield.Power;
            shield = null;
            Destroy(shielaobj);
        }
    }

    public void EquipWeapon(Item item)
    {
        weaponObj = Instantiate(invManager.itemPrefab[item.PrefabID], weaponHand);

        weaponObj.transform.localPosition = new Vector3(7.5f, 2f, 8f);
        weaponObj.transform.Rotate(0f, 90f , -90f, Space.Self);

        attackPower += item.Power;
        mainWeapon = item;
    }

     public void UnEquipWeapon()
    {
        if (weaponObj != null)
        {
            mainWeapon = null;
            Destroy(weaponObj);
        }
    }

}
