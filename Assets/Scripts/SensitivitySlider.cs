using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SensitivitySlider : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI SliderText;
    public TMP_InputField inputField;
    public Slider slider;
    void Start()
    {
        SliderText.text = Settings.sensitivitySettings.ToString("0.00");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void inputfieldvalueRecieved()
    {


    }

    public void changesens(float sensInput)
    {
        Settings.changeSensitivity(sensInput);
        SliderText.text = sensInput.ToString("0.00");
    }
}
