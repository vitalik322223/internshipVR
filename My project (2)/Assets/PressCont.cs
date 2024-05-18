using UnityEngine;
using System.Collections;

public class PressMachineController : MonoBehaviour
{
    public Transform press; // Двигающаяся часть прессующего станка
    public Transform cube; // Cube (1)
    public Vector3 pressEndPosition; // Конечная позиция пресса для спрессовки
    public Vector3 cubeCompressedScale; // Масштаб Cube (1) после спрессовки
    public float pressDuration = 2f; // Длительность процесса прессовки
    public RobotArmController robotArmController; // Ссылка на скрипт управления роборукой

    private Vector3 pressStartPosition;
    private Vector3 cubeStartScale;

    void Start()
    {
        // Сохраняем начальные позиции и масштаб
        pressStartPosition = press.position;
        cubeStartScale = cube.localScale;
    }

    public void StartPressing()
    {
        StartCoroutine(PressCube());
    }

    IEnumerator PressCube()
    {
        // Опускаем пресс
        float elapsedTime = 0f;
        while (elapsedTime < pressDuration)
        {
            press.position = Vector3.Lerp(pressStartPosition, pressEndPosition, elapsedTime / pressDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        press.position = pressEndPosition;

        // Сжимаем Cube (1)
        elapsedTime = 0f;
        while (elapsedTime < pressDuration)
        {
            cube.localScale = Vector3.Lerp(cubeStartScale, cubeCompressedScale, elapsedTime / pressDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        cube.localScale = cubeCompressedScale;

        // Поднимаем пресс обратно
        elapsedTime = 0f;
        while (elapsedTime < pressDuration)
        {
            press.position = Vector3.Lerp(pressEndPosition, pressStartPosition, elapsedTime / pressDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        press.position = pressStartPosition;

        // Сообщаем роборуке, что прессовка завершена
        if (robotArmController != null)
        {
            robotArmController.OnPressingFinished();
        }
    }
}
