using Unity.Mathematics;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Collider2D Target;
    [SerializeField] private Vector2 FocusAreaSize;
    [SerializeField] private float VerticalOffset;
    [SerializeField] private float LookAheadDstX;
    [SerializeField] private float LookSmoothX;

    [SerializeField] private bool IsDebug;
    [SerializeField] private Color ColorArea;

    private FocusArea focusArea;
    private Vector2 focusPosition = new Vector2();
    private float offsetZ = -10f;
    private float currentlookAheadX;
    private float targetLookAheadX;
    private float LookAheadDirX;
    private float smoothLookVelocityX;

    private void Start()
    {
        focusArea = new FocusArea(Target.bounds, FocusAreaSize);
    }

    private void LateUpdate()
    {
        focusArea.Update(Target.bounds);
        focusPosition = focusArea.Center + Vector2.up * VerticalOffset;

        if(focusArea.Velocity.x != 0)
        {
            LookAheadDirX = Mathf.Sign(focusArea.Velocity.x);
        }

        targetLookAheadX = LookAheadDirX * LookAheadDstX;
        currentlookAheadX = Mathf.SmoothDamp(currentlookAheadX, targetLookAheadX, ref smoothLookVelocityX, LookSmoothX);

        focusPosition += Vector2.right * currentlookAheadX;

        transform.position = (Vector3)focusPosition + Vector3.forward * offsetZ;
    }

    private void OnDrawGizmos()
    {
        if (IsDebug)
        {
            Gizmos.color = ColorArea;
            Gizmos.DrawCube(focusArea.Center, FocusAreaSize);
        }
    }
}
