using UnityEngine;

public class TestUIToWorld : MonoBehaviour
{

    public RectTransform image;
    
    // Start is called before the first frame update
    void Start()
    {
        transform.position = image.position;
    }
}
