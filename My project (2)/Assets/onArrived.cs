using UnityEngine;
using System.Collections;

public class RobotArmController : MonoBehaviour
{
    public Transform pressMachine; // Прессующий станок
    public Transform table; // Стол, на который кладется Cube (1)
    public Transform cube; // Cube (1)
    public Transform robotArm; // Рука робота, которая будет вращаться
    public Transform initialPosition; // Начальная позиция руки робота
    public float interactionDistance = 2f; // Максимальное расстояние для взаимодействия

    private Quaternion initialRotation;
    private bool isPressingFinished = false;

    public delegate void CubePlacedOnTableHandler();
    public event CubePlacedOnTableHandler onCubePlacedOnTable;

    void Start()
    {
        // Сохраняем начальную ротацию руки
        initialRotation = robotArm.rotation;
    }

    public void HandleCube()
    {
        StartCoroutine(HandleCubeCoroutine());
    }

    IEnumerator HandleCubeCoroutine()
    {
        // Поворот к Cube (1)
        Debug.Log("Rotating towards Cube (1)..."); // Отладочное сообщение
        yield return StartCoroutine(RotateTowards(cube.position));

        // Захват Cube (1)
        Debug.Log("Grabbing Cube (1)..."); // Отладочное сообщение
        cube.SetParent(robotArm);

        // Поворот к прессующему станку
        Debug.Log("Rotating towards Press Machine..."); // Отладочное сообщение
        yield return StartCoroutine(RotateTowards(pressMachine.position));

        // Перемещение Cube (1) к прессующему станку
        Debug.Log("Moving Cube (1) to Press Machine..."); // Отладочное сообщение
        yield return StartCoroutine(MoveCubeToPress());

        // Освобождение Cube (1)
        Debug.Log("Releasing Cube (1)..."); // Отладочное сообщение
        cube.SetParent(null);

        // Запуск процесса прессовки
        Debug.Log("Starting Pressing..."); // Отладочное сообщение
        pressMachine.GetComponent<PressMachineController>().StartPressing();

        // Ожидание завершения прессовки
        while (!isPressingFinished)
        {
            yield return null;
        }

        // Поворот к Cube (1) снова
        Debug.Log("Rotating towards Cube (1) again..."); // Отладочное сообщение
        yield return StartCoroutine(RotateTowards(cube.position));

        // Захват спрессованного Cube (1)
        Debug.Log("Grabbing compressed Cube (1)..."); // Отладочное сообщение
        cube.SetParent(robotArm);

        // Поворот к столу
        Debug.Log("Rotating towards Table..."); // Отладочное сообщение
        yield return StartCoroutine(RotateTowards(table.position));

        // Перемещение Cube (1) к столу
        Debug.Log("Moving Cube (1) to Table..."); // Отладочное сообщение
        yield return StartCoroutine(MoveCubeToTable());

        // Освобождение Cube (1) над столом
        Debug.Log("Releasing Cube (1) above the table..."); // Отладочное сообщение
        cube.position = new Vector3(table.position.x, table.position.y + 0.25f, table.position.z);
        cube.SetParent(null);

        // Сообщаем, что Cube (1) размещен на столе
        onCubePlacedOnTable?.Invoke();

        // Возврат руки в начальное положение
        Debug.Log("Returning to Initial Position..."); // Отладочное сообщение
        yield return StartCoroutine(ReturnToInitialPosition());

        Debug.Log("Cube (1) moved to table and arm returned to initial position."); // Отладочное сообщение
    }

    IEnumerator RotateTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - robotArm.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);

        // Поворачиваем только по оси Y
        lookRotation = Quaternion.Euler(0, lookRotation.eulerAngles.y, 0);

        float duration = 1f; // Время поворота
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            robotArm.rotation = Quaternion.Slerp(robotArm.rotation, lookRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        robotArm.rotation = lookRotation;
    }

    IEnumerator MoveCubeToPress()
    {
        Vector3 startPos = cube.position;
        Quaternion startRot = cube.rotation;
        Vector3 endPos = pressMachine.position;
        Quaternion endRot = pressMachine.rotation;

        float duration = 2f; // Время перемещения
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            cube.position = Vector3.Lerp(startPos, endPos, elapsedTime / duration);
            cube.rotation = Quaternion.Lerp(startRot, endRot, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cube.position = endPos;
        cube.rotation = endRot;
    }

    IEnumerator MoveCubeToTable()
    {
        Vector3 startPos = cube.position;
        Quaternion startRot = cube.rotation;
        Vector3 endPos = new Vector3(table.position.x, table.position.y + 0.25f, table.position.z);
        Quaternion endRot = table.rotation;

        float duration = 2f; // Время перемещения
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            cube.position = Vector3.Lerp(startPos, endPos, elapsedTime / duration);
            cube.rotation = Quaternion.Lerp(startRot, endRot, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cube.position = endPos;
        cube.rotation = endRot;
    }

    IEnumerator ReturnToInitialPosition()
    {
        float duration = 1f; // Время поворота
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            robotArm.rotation = Quaternion.Slerp(robotArm.rotation, initialRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        robotArm.rotation = initialRotation;
    }

    public void OnPressingFinished()
    {
        isPressingFinished = true;
    }
}
