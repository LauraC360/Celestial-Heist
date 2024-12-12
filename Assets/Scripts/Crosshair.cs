using UnityEngine;

public class Crosshair : MonoBehaviour
{
    private static readonly int Show = Animator.StringToHash("Show");
    [SerializeField] private GameObject crosshair;
    [SerializeField] private Animator crosshairAnimator;
    [SerializeField] private Transform screenSurface;
    [SerializeField] private Transform min;
    [SerializeField] private Transform max;

    public void SetToTarget(Transform target)
    {
        if (target == null)
        {
            crosshairAnimator.SetBool(Show, false);
            return;
        }
        
        var cameraPos = Camera.main.transform.position;
        RaycastHit hit;
        Physics.Raycast(cameraPos, target.transform.position - cameraPos, out hit, 100, LayerMask.GetMask("Screen"));
        if (hit.transform == screenSurface)
        {
            crosshair.transform.position = hit.point;
            crosshair.transform.localPosition = new Vector3(Mathf.Clamp(crosshair.transform.localPosition.x, min.localPosition.x, max.localPosition.x), Mathf.Clamp(crosshair.transform.localPosition.y, min.localPosition.y, max.localPosition.y ), 0);
            crosshairAnimator.SetBool(Show, true);
        }
    }
}
