using UnityEngine;
using System.Collections;

public class cameraMove : MonoBehaviour
{
    public static float sensitivity = 200;
    public float rotateSensitivity = 0.1f;
    public static float rotateSensi;
    public float accelerate;
    float forward = 1;
    float back = 1;
    float left = 1;
    float right = 1;
    float up = 1;
    float down = 1;
    public enum RotationAxes { MouseXAndY = 0 }
    public RotationAxes axes = RotationAxes.MouseXAndY;
    public static float sensitivityX = 20F;
    public float sensitivityY = 20F;
    float rotationY = 0F;
    float rotationX = 0F;
    public float minimumX = -360F;
    public float maximumX = 360F;

    public float minimumY = -360F;
    public float maximumY = 360F;
    public float moveSensitivity = 0.1F;
    public float scaleSensitivity = 0.1F;
    // Use this for initialization
    void Start()
    {
        //sensitivity = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        //控制相机位置
        if (Input.GetKey(KeyCode.D))
        {
            //right += accelerate;
            transform.Translate(new Vector3(sensitivity * right * Time.deltaTime, 0f, 0f), Space.Self);
        }
        else
        {
            //right = 0f;
        }

        if (Input.GetKey(KeyCode.A))
        {
            //left+= accelerate;
            transform.Translate(new Vector3(-sensitivity * left * Time.deltaTime, 0f, 0f), Space.Self);
        }
        else
        {
            //left = 0f;
        }

        if (Input.GetKey(KeyCode.W))
        {
            //forward+= accelerate;
            transform.Translate(new Vector3(0f, 0f, sensitivity * forward * Time.deltaTime), Space.Self);
        }
        else
        {
            //forward = 0f;
        }

        if (Input.GetKey(KeyCode.S))
        {
            //back+= accelerate;
            transform.Translate(new Vector3(0f, 0f, sensitivity * (-back) * Time.deltaTime), Space.Self);
        }
        else
        {
            //back = 0f;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            //up+=accelerate;
            transform.Translate(new Vector3(0f, sensitivity * up * Time.deltaTime, 0f), Space.Self);
        }
        else
        {
            //up = 0f;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            //down+=accelerate;
            transform.Translate(new Vector3(0f, sensitivity * (-down) * Time.deltaTime, 0f), Space.Self);
        }
        else
        {
            //down = 0f;
        }
        //Debug.Log (i);

        if (Input.GetKey(KeyCode.Q))
        {
            //down+=accelerate;
            transform.Rotate(new Vector3(0f, -rotateSensitivity * Time.deltaTime, 0f), Space.World);
        }

        if (Input.GetKey(KeyCode.E))
        {
            //down+=accelerate;
            transform.Rotate(new Vector3(0f, rotateSensitivity * Time.deltaTime, 0f), Space.World);
        }
        if (axes == RotationAxes.MouseXAndY)
        {

            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.Mouse0))
            {
                transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX * Time.deltaTime, 0);

                rotationY += Input.GetAxis("Mouse Y") * sensitivityY * Time.deltaTime;
                rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);
                transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);

            }
        }
        if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.Mouse2))
        {

            //float y= (float) ((3.14159/180*transform.eulerAngles.y));
            //locationX += (float)0.01*Mathf.Cos( x);
            //locationZ += (float)0.01*Mathf.Sin( x);
            //transform.Translate((float)1*Mathf.Cos( y),0,-(float)1*Mathf.Sin( y));
            //Vector3 a= Vector3(-1F,0F,0F);

            transform.Translate(new Vector3((float)-1 * Input.GetAxis("Mouse X") * Mathf.Abs(transform.position.z * moveSensitivity) * Time.deltaTime, 0F, 0F), Space.Self);
            transform.Translate(Vector3.down * Input.GetAxis("Mouse Y") * Mathf.Abs(transform.position.z * moveSensitivity) * Time.deltaTime, Space.Self);

        }
        Ray ray1 = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        sensitivityX = sensitivityY;
        if (!Physics.Raycast(ray1, out hit))
        {//控制滚轮滚动时的镜头变换
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
                transform.Translate(0, 0, (float)0.1 * Mathf.Pow((Mathf.Abs(transform.position.z) * Time.deltaTime), 9 / 8f) * scaleSensitivity / 5);
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
                transform.Translate(0, 0, (float)-0.1 * Mathf.Pow((Mathf.Abs(transform.position.z) * Time.deltaTime), 9 / 8f) * scaleSensitivity / 5);
        }
        else
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                //camera0.transform.Translate(ray1.direction,Space.World);
                transform.position = new Vector3(transform.position.x - (transform.position.x - hit.point.x) / 2f * Time.deltaTime * scaleSensitivity / 3, transform.position.y - (transform.position.y - hit.point.y) / 2f * Time.deltaTime * scaleSensitivity / 3, transform.position.z - (transform.position.z - hit.point.z) / 2f * Time.deltaTime * scaleSensitivity / 3);
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                transform.position = new Vector3(transform.position.x + (transform.position.x - hit.point.x) / 2f * Time.deltaTime * scaleSensitivity / 3, transform.position.y + (transform.position.y - hit.point.y) / 2f * Time.deltaTime * scaleSensitivity / 3, transform.position.z + (transform.position.z - hit.point.z) / 2f * Time.deltaTime * scaleSensitivity / 3);
            }
        }

        rotateSensi = rotateSensitivity;

    }
}
