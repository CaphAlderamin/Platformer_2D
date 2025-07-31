using UnityEngine;

public class ParallaxController : MonoBehaviour
{

    public bool paralax;
    public bool scrolling;

    public float offsetBack;
    public float backGroundSize;
    public float paralaxSpeed;

    private Transform cameraTransform;
    private Transform[] layers;
    private int leftIndex;
    private int rightIndex;

    private float lastCameraX;

    void Start()
    {
        cameraTransform = Camera.main.transform;
        layers = new Transform[transform.childCount];
        lastCameraX = cameraTransform.position.x;

        for (int i = 0; i < transform.childCount; i++)
        {
            layers[i] = transform.GetChild(i);
        }

        leftIndex = 0;
        rightIndex = layers.Length - 1;
    }

    void FixedUpdate()
    {
        if (paralax)
        {
            float deltaX = cameraTransform.position.x - lastCameraX;
            transform.position -= Vector3.right * (deltaX * paralaxSpeed);
            lastCameraX = cameraTransform.position.x;
        }

        if (scrolling)
        {
            if (cameraTransform.position.x < (layers[leftIndex].transform.position.x + 5))
            {
                ScrollLeft();
            }
            if (cameraTransform.position.x > (layers[rightIndex].transform.position.x - 5))
            {
                ScrollRight();
            }
        }
    }

    private void ScrollLeft()
    {
        //int lastRight = rightIndex;
        //layers[rightIndex].position = Vector2.right * (layers[leftIndex].position.x - backGroundSize);
        layers[rightIndex].position = new Vector3(1 * (layers[leftIndex].position.x - backGroundSize), layers[rightIndex].position.y, offsetBack);
        leftIndex = rightIndex;
        rightIndex--;

        if (rightIndex < 0)
        {
            rightIndex = layers.Length - 1;
        }
    }

    private void ScrollRight()
    {
        //int lastLeft = leftIndex;
        //layers[leftIndex].position = Vector2.right * (layers[rightIndex].position.x + backGroundSize);
        layers[leftIndex].position = new Vector3(1 * (layers[rightIndex].position.x + backGroundSize), layers[rightIndex].position.y, offsetBack);
        rightIndex = leftIndex;
        leftIndex++;

        if (leftIndex == layers.Length)
        {
            leftIndex = 0;
        }
    } 
}
