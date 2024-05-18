using UnityEngine;

public class MoveToTarget : MonoBehaviour
{
    public Transform target; // Цель, к которой будет двигаться тележка (роборука)
    public Transform table; // Стол, к которому будет двигаться тележка
    public Transform startPosition; // Исходная позиция тележки
    public float speed = 5f; // Скорость движения
    public float stopDistance = 0.1f; // Дистанция, на которой тележка останавливается у цели
    public float tableStopDistance = 1f; // Дистанция, на которой тележка останавливается от стола
    public GameObject cube; // Cube (1), который должен двигаться вместе с тележкой
    public RobotArmController robotArmController; // Ссылка на скрипт управления роборукой

    private bool isMovingToTable = false; // Флаг для отслеживания этапа движения к столу
    private bool isMovingBackToStart = false; // Флаг для отслеживания этапа движения в исходную позицию

    void Start()
    {
        // Присоединяем Cube (1) к тележке перед началом движения
        if (cube != null)
        {
            cube.transform.SetParent(transform);
        }
    }

    void Update()
    {
        // Проверка, задана ли цель
        if (target != null && !isMovingToTable && !isMovingBackToStart)
        {
            // Рассчитываем направление к цели
            Vector3 direction = (target.position - transform.position).normalized;

            // Перемещаем тележку в сторону цели
            transform.position += direction * speed * Time.deltaTime;

            // Останавливаем движение, когда тележка достаточно близко к цели
            if (Vector3.Distance(transform.position, target.position) < stopDistance)
            {
                transform.position = target.position;
                Debug.Log("Cart has arrived at the target."); // Отладочное сообщение
                OnArrivedAtTarget();
            }
        }
        else if (isMovingToTable && table != null)
        {
            // Рассчитываем направление к столу
            Vector3 direction = (table.position - transform.position).normalized;

            // Перемещаем тележку в сторону стола
            transform.position += direction * speed * Time.deltaTime;

            // Останавливаем движение, когда тележка достаточно близко к столу
            if (Vector3.Distance(transform.position, table.position) < tableStopDistance)
            {
                Debug.Log("Cart has arrived near the table."); // Отладочное сообщение
                OnArrivedAtTable();
            }
        }
        else if (isMovingBackToStart && startPosition != null)
        {
            // Рассчитываем направление к исходной позиции
            Vector3 direction = (startPosition.position - transform.position).normalized;

            // Перемещаем тележку в сторону исходной позиции
            transform.position += direction * speed * Time.deltaTime;

            // Останавливаем движение, когда тележка достаточно близко к исходной позиции
            if (Vector3.Distance(transform.position, startPosition.position) < stopDistance)
            {
                transform.position = startPosition.position;
                Debug.Log("Cart has returned to the start position."); // Отладочное сообщение
                OnArrivedAtStart();
            }
        }
    }

    void OnArrivedAtTarget()
    {
        // Отсоединяем Cube (1) от тележки после остановки и включаем гравитацию
        if (cube != null)
        {
            cube.transform.SetParent(null);
            Rigidbody rb = cube.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false; // Включаем физику для Cube (1)
                rb.useGravity = true; // Включаем гравитацию для Cube (1)
            }

            // Изменяем цвет Cube (1) на черный
            Renderer cubeRenderer = cube.GetComponent<Renderer>();
            if (cubeRenderer != null)
            {
                cubeRenderer.material.color = Color.black;
            }
        }

        // Запускаем функцию роборуки для захвата и перемещения Cube (1)
        if (robotArmController != null)
        {
            robotArmController.HandleCube();
        }

        // Устанавливаем флаг для начала движения к столу
        isMovingToTable = true;
    }

    void OnArrivedAtTable()
    {
        // Ожидание, пока роборука не переместит Cube (1) на стол
        robotArmController.onCubePlacedOnTable += OnCubePlacedOnTableHandler;
    }

    void OnCubePlacedOnTableHandler()
    {
        // Присоединяем Cube (1) обратно к тележке
        if (cube != null)
        {
            cube.transform.SetParent(transform);
        }

        // Устанавливаем флаг для начала движения в исходную позицию
        isMovingToTable = false;
        isMovingBackToStart = true;
    }

    void OnArrivedAtStart()
    {
        // Тележка вернулась в исходную позицию
        Debug.Log("Cart has completed its journey and returned to the start position."); // Отладочное сообщение
        enabled = false; // Отключаем скрипт, чтобы остановить тележку
    }
}
