using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IOSRateMenuSupportManager : MonoBehaviour
{
    public Text txt_Loading,txt_Ok;
    private float timer = 1;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0;
        txt_Loading.gameObject.SetActive(true);
        txt_Ok.gameObject.SetActive(false);


    }

    void Update()
    {
        Time.timeScale = 0;
        if (timer >= 0)
            timer -= Time.unscaledDeltaTime * 1.2f;
        if (timer < 0)
        {
            txt_Loading.gameObject.SetActive(false);
            txt_Ok.gameObject.SetActive(true);
        }
    }

    public void okButtonCLicked()
    {
        Time.timeScale = 1;
        
        Destroy(this.gameObject);
    }
}
