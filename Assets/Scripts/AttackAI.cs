using UnityEngine;

public class AttackAI : MonoBehaviour
{
    private Character myChar;

    [SerializeField]
    private Character curEnemy;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myChar = GetComponent<Character>();

        if (myChar != null)
            InvokeRepeating("FindAndAttackEnemy", 0f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FindAndAttackEnemy()
    {
        if (myChar.CurCharTarget == null)
        {
            curEnemy = Formula.FindClosestEnemyChar(myChar);
            if (curEnemy == null)
            {
                return;
            }
            if (myChar.IsmyEnemy(curEnemy.gameObject.tag))
            {
                myChar.ToAttackCharacter(curEnemy);
            }
        }
    }
}
