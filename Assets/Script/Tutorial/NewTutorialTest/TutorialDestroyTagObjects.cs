using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDestroyTagObjects : TutorialBase
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Vector3 enemySpawnPos;
    //[SerializeField] private string tagName;
    private GameObject aliveEnemy;

    public override void Enter()
    {
        playerController.isMoveAble = true;

        aliveEnemy = Instantiate(enemyPrefab, enemySpawnPos, Quaternion.identity);
    }

    public override void Execute(TutorialController controller)
    {
        //GameObject[] objects = GameObject.FindGameObjectsWithTag(tagName);
        if (aliveEnemy == null)
        {
            controller.SetNextTutorial();
        }
    }

    public override void Exit()
    {
        playerController.isMoveAble = false;
    }
}
