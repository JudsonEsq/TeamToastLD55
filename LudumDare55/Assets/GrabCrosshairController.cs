using UnityEngine;
using Image = UnityEngine.UI.Image;

public class GrabCrosshairController : MonoBehaviour
{
    public Image CrosshairImage;
    public Sprite GrabSprite;
    public Sprite DefaultSprite;
    public Color HoverColor = new Color(235/255f, 61/255f, 0f, 1);
    public Color InactiveColor = new Color(235/255f, 0, 141/255f, 1);
    public float LerpSpeed = 5.6f;
    
    Color _targetColor;
    DynamicObjectMover _mover;
    CameraController _cameraController;
    
    // Start is called before the first frame update
    void Start()
    {
        _cameraController = GameObject.FindObjectOfType<CameraController>();
        _mover = GameObject.FindObjectOfType<DynamicObjectMover>();
    }

    // Update is called once per frame
    void Update()
    {

        if (_mover && _cameraController) {
            if (!_mover.HasObject) {
                if (_cameraController.DynamicObjectRaycast(out _, _mover.MaximumGrabDistance)) {
                    _targetColor = Color.Lerp(_targetColor, HoverColor, LerpSpeed * Time.deltaTime);
                }
                else {
                    _targetColor = Color.Lerp(_targetColor, InactiveColor, LerpSpeed * Time.deltaTime);
                }
            }
            else {
                _targetColor = Color.Lerp(_targetColor, HoverColor, LerpSpeed * Time.deltaTime);
            }
            CrosshairImage.sprite = _mover.HasObject ? GrabSprite : DefaultSprite;
            CrosshairImage.color = _targetColor;
        }
    }
}
