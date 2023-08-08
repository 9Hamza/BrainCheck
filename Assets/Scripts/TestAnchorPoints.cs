using UnityEngine;

public class TestAnchorPoints : MonoBehaviour
{

    private RectTransform _myRectTransform;
    [SerializeField] private Transform target;

    private void Awake()
    {
        _myRectTransform = GetComponent<RectTransform>();
        
        // Convert the image's position to screen space
        Vector2 ImagePosInScreenStartingFromBottomLeftAsAnchor = RectTransformUtility.WorldToScreenPoint(Camera.main, _myRectTransform.position);
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(RectTransformUtility.WorldToScreenPoint(Camera.main, target.position));
        // Debug.Log($" anchoredPosition: {_myRectTransform.anchoredPosition} -" +
        //           $" position: {_myRectTransform.position} -" +
        //           $" localPosition: {_myRectTransform.localPosition} -" +
        //           $" screen width: {Screen.width} -" +
        //           $" screen height: {Screen.height} -" +
        //           $" bullseye: {Camera.main.WorldToScreenPoint(bullseye.position)} -" +
        //           $" square: {RectTransformUtility.WorldToScreenPoint(Camera.main, _myRectTransform.position)}");
    }
}
