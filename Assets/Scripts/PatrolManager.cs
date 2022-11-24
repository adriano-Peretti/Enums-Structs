using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PatrolManager : MonoBehaviour
{
    enum EnemyState
    {
        Patrolling,
        Stopped
    }

    enum PatrolDirection
    {
        Top,
        Left,
        Right,
        Bottom
    }

    [Serializable]
    struct PatrolData
    {
        public float MoveSpeed;
        public float PatrolDuration;
        public float MoveDirectDuration;
        public float PauseDuration;
    }

    //Variaveis
    [SerializeField] EnemyState currentEnemyState;
    [SerializeField] List<PatrolData> listPatrolData = new List<PatrolData>();
    [SerializeField] PatrolDirection currentPatrolDirection;
    private float directChangeTime;
    private float startPatrolTime;
    private int patrolType;
    public TMP_Text patrol;

    void Start()
    {
        currentEnemyState = EnemyState.Stopped;
        currentPatrolDirection = PatrolDirection.Left;
        directChangeTime = 0;
        patrolType = 0;
        listPatrolData.Add(new PatrolData() { PatrolDuration = 10, MoveSpeed = 1, MoveDirectDuration = 3f, PauseDuration = 0.3f });
        listPatrolData.Add(new PatrolData() { PatrolDuration = 15, MoveSpeed = 2, MoveDirectDuration = 1.5f, PauseDuration = 0.4f });
        listPatrolData.Add(new PatrolData() { PatrolDuration = 12, MoveSpeed = 3, MoveDirectDuration = 2f, PauseDuration = 0.5f });
        listPatrolData.Add(new PatrolData() { PatrolDuration = 8, MoveSpeed = 4, MoveDirectDuration = 4f, PauseDuration = 0.2f });
    }

    void Update()
    {
        ChangeRotine();
        ChangeEnemyState();
        AttText();
    }

    private void AttText()
    {
        patrol.text = $"Estado: {currentEnemyState} \n" +
                    $"Tipo de Patrulha: {patrolType} \n" +
                    $"Tempo de patrula: {listPatrolData[patrolType].PatrolDuration} \n" +
                    $"Direção: {currentPatrolDirection} \n" +
                    $"Tempo parado: {listPatrolData[patrolType].PauseDuration} \n" +
                    $"Velocidade: {listPatrolData[patrolType].MoveSpeed} ";
    }

    private void ChangeEnemyState()
    {
        switch (currentEnemyState)
        {
            default:
            case EnemyState.Stopped:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    currentEnemyState = EnemyState.Patrolling;
                    startPatrolTime = Time.time;
                }
                break;

            case EnemyState.Patrolling:

                if (Time.time > startPatrolTime + listPatrolData[patrolType].PatrolDuration)
                {
                    currentEnemyState = EnemyState.Stopped;
                    startPatrolTime = Time.time;
                }
                else
                {
                    PatrolRotine(listPatrolData[patrolType]);
                }
                break;
        }
    }

    private void ChangeRotine()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (patrolType == 3)
            {
                patrolType = 0;
            }
            else
            {
                patrolType++;
                patrolType = Mathf.Clamp(patrolType, 0, 4);
            }
        }
    }

    private void PatrolRotine(PatrolData patrolData)
    {
        directChangeTime += Time.deltaTime;
        switch (currentPatrolDirection)
        {
            case PatrolDirection.Right:
                ChangePatrolDirection(patrolData, PatrolDirection.Top);
                Translate(new Vector3(patrolData.MoveSpeed * Time.deltaTime, 0, 0), patrolData);
                break;

            case PatrolDirection.Top:
                ChangePatrolDirection(patrolData, PatrolDirection.Left);
                Translate(new Vector3(0, 0, patrolData.MoveSpeed * Time.deltaTime), patrolData);
                break;

            case PatrolDirection.Left:
                ChangePatrolDirection(patrolData, PatrolDirection.Bottom);
                Translate(new Vector3(-patrolData.MoveSpeed * Time.deltaTime, 0, 0), patrolData);
                break;

            case PatrolDirection.Bottom:
                ChangePatrolDirection(patrolData, PatrolDirection.Right);
                Translate(new Vector3(0, 0, -patrolData.MoveSpeed * Time.deltaTime), patrolData);
                break;
        }
    }

    private void ChangePatrolDirection(PatrolData patrolData, PatrolDirection newPatrolDirection)
    {
        if (directChangeTime > patrolData.MoveDirectDuration)
        {
            currentPatrolDirection = newPatrolDirection;
            directChangeTime = 0;
        }
    }

    void Translate(Vector3 translation, PatrolData patrolData)
    {
        if (directChangeTime > patrolData.PauseDuration)
        {
            transform.position = transform.position + translation;
        }
    }
}