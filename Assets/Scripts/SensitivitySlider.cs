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
    private Slider slider;
    void Start()
    {
        slider = GetComponent<Slider>();
        SliderText.text = Settings.sensitivitySettings.ToString("0.00");
        if (inputField != null)
        {
            inputField.text = Settings.sensitivitySettings.ToString("0.000");
        }
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
        SliderText.text = sensInput.ToString("0.000");
        inputField.text = sensInput.ToString("0.000");

    }

    public void OnSensitivityTextEdit(string text)
    {
        float sens;
        if(float.TryParse(text, out sens))
        {
            Settings.changeSensitivity(sens);
            slider.value = sens;
        }
    }
}
