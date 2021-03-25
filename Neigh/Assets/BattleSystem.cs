using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }
public class BattleSystem : MonoBehaviour
{

    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    Unit playerUnit;
    Unit enemyUnit;

    public Text dialogueText;

    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;

    public BattleState state;

    void Start()
    {
        state = BattleState.START;
        SetupBattle();
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
        playerUnit = playerGO.GetComponent<Unit>();

       GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
        enemyUnit = enemyGO.GetComponent<Unit>();

        dialogueText.text = "A beautiful wild " + enemyUnit.unitName + " appears!";

        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);

        yield return new WaitForSeconds(2f);
        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    void PlayerTurn()
    {
        dialogueText.text = "What do you want to do?";
    }

    IEnumerator PlayerHeal()
    {
        playerUnit.Heal(Random.Range(2,4));
        playerHUD.SetHP(playerUnit.currentHP);
        dialogueText.text = "You regained some stamina...";

        yield return new WaitForSeconds(2f);

        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }

    IEnumerator PlayerAttack()
    {
       bool isDead = enemyUnit.TakeDamage(playerUnit.damage);

        enemyHUD.SetHP(enemyUnit.currentHP);
        dialogueText.text = "The horse is warming up to you...";

        yield return new WaitForSeconds(2f);

        if (isDead)
        {
            state = BattleState.WON;
            EndBattle();
        } 
        
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }

    }

    IEnumerator EnemyTurn()
    {
        dialogueText.text = enemyUnit.unitName + "is reacting! Woah there! Easy!";

        yield return new WaitForSeconds(1f);

        bool isDead = playerUnit.TakeDamage(enemyUnit.damage);

        playerHUD.SetHP(playerUnit.currentHP);

        yield return new WaitForSeconds(1f);

        if (isDead)
        {
            state = BattleState.LOST;
            EndBattle();

        }

        else

        {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }
    }

    void EndBattle()
    {
        if (state == BattleState.WON)
        {
            dialogueText.text = "You tamed the horse!";
        }
        else if (state == BattleState.LOST)
        {
            dialogueText.text = "The horse ran away!";
        }
    }

    public void onAttackButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerAttack());
    }

    public void onHealButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerHeal());
    }
}
