using UnityEngine;
using UnityEngine.UI;

public class SetOptionFromUI : MonoBehaviour
{
    public Scrollbar volumeSlider;
    public Scrollbar musicSlider;
    public TMPro.TMP_Dropdown turnDropdown;
    public SetTurnTypeFromPlayerPref turnTypeFromPlayerPref;

    private void Start()
    {
        volumeSlider.onValueChanged.AddListener(SetGlobalVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        turnDropdown.onValueChanged.AddListener(SetTurnPlayerPref);

        if (PlayerPrefs.HasKey("turn"))
            turnDropdown.SetValueWithoutNotify(PlayerPrefs.GetInt("turn"));

        // Khôi phục giá trị đã lưu trữ
        if (PlayerPrefs.HasKey("globalVolume"))
            volumeSlider.value = PlayerPrefs.GetFloat("globalVolume");
        if (PlayerPrefs.HasKey("musicVolume"))
            musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
    }

    public void SetGlobalVolume(float value)
    {
        PlayerPrefs.SetFloat("globalVolume", value); // Lưu giá trị
        AudioManager.instance.SetGlobalVolume(value);
    }

    public void SetMusicVolume(float value)
    {
        PlayerPrefs.SetFloat("musicVolume", value); // Lưu giá trị
        AudioManager.instance.SetVolume("BackgroundMusic", value);
    }

    public void SetTurnPlayerPref(int value)
    {
        PlayerPrefs.SetInt("turn", value);
        turnTypeFromPlayerPref.ApplyPlayerPref();
    }
}
