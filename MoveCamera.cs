using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [SerializeField]
    GameObject postionThirdPersonCamera, positonTopDownCamera;
    bool firstPosition = true;

    Camera camera;

    public GameObject CameraFollowObj;
    public float CameraMoveSpeed = 120.0f;

    private float rotY = 0.0f;
    private float rotX = 0.0f;
    float inputSensitivity = 90.0f;
    float clampAngle = 80.0f;
    float mouseX;
    float mouseY;
    float finalInputX;
    float finalInputZ;
    // Start is called before the first frame update
    void Start()
    {
        camera = GetComponentInChildren<Camera>();
        MoveAtoB(gameObject, postionThirdPersonCamera, 2);
    }

    // Update is called once per frame
    void Update()
    {
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        rotY += mouseX * inputSensitivity * Time.deltaTime;
        rotX += mouseY * inputSensitivity * Time.deltaTime;

        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

        Quaternion localRotation = Quaternion.Euler(-rotX, rotY, 0.0f);
        transform.rotation = localRotation;

        if (InputSystem.instance.IsCameraButtonPressed)
        {
            SetPositionCamera();
            firstPosition = !firstPosition;
        }
    }

    void LateUpdate()
    {
        CameraUpdater();
    }

    void CameraUpdater()
    {
        // set the target object to follow
        Transform target = CameraFollowObj.transform;

        //move towards the game object that is the target
        float step = CameraMoveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);
    }

    public void SetPositionCamera()
    {
        if (firstPosition)
        {
            StartCoroutine(MoveAtoB(camera.gameObject, positonTopDownCamera, 1));
        }
        else
        {
            StartCoroutine(MoveAtoB(camera.gameObject, postionThirdPersonCamera, 1));
        }
    }

    private IEnumerator MoveAtoB(GameObject posA, GameObject posB, float duration)
    {
        float currentTime = 0;
        //lấy góc quay của camera ở vị trí tiếp theo
        float startDepth = posA.GetComponent<Camera>().fieldOfView;
        float desDepth = posB.GetComponent<Camera>().fieldOfView;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            posA.transform.position = Vector3.Slerp(posA.transform.position, posB.transform.position, currentTime / duration);
            posA.transform.rotation = Quaternion.Slerp(posA.transform.rotation, posB.transform.rotation, currentTime / duration);
            camera.fieldOfView = Mathf.Lerp(startDepth, desDepth, currentTime / duration);
            yield return 0;
        }
    }
}
