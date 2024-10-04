using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class su_alti_camera : MonoBehaviour
{
    [SerializeField] arduino_Control arduino_control;

    [SerializeField] GameObject ust_renk_sinir;
    [SerializeField] GameObject alt_renk_sinir;
    [SerializeField] GameObject sag_renk_sinir;
    [SerializeField] GameObject sol_renk_sinir;
    [Space(25)]

    [SerializeField] float x_sinir_uzunluk;
    [SerializeField] float y_sinir_uzunluk;
    [Space(5)]
    [SerializeField] float obje_asil_x;
    [SerializeField] float obje_asil_y;
    [Space(25)]

    [SerializeField] byte otonom_kamera_aci = 125;

    [Space(25)]
    [SerializeField] int cameraIndex;
    [SerializeField] RawImage rawImage;

    [SerializeField] GameObject kirmizi_renk_orta;


    private WebCamTexture webcamTexture;


    Color32[] pixels;
    Color pixelColor;

    [SerializeField] Color targetColor = Color.red; // Aranan renk
    [SerializeField] float colorTolerance = 0.1f;
    [Space(25)]

    public bool genel_otonom = false;
    public bool renk_bulundu;

    [SerializeField] byte[] otonom_kumanda_veri = new byte[10];

    public float gecen_sure;
    [SerializeField] int gonderilen_veri_sayisi;


    void Start()
    {
        WebCamDevice[] webcams = WebCamTexture.devices;

        if (webcams.Length > cameraIndex)
        {
            webcamTexture = new WebCamTexture(webcams[cameraIndex].name);
            if (webcamTexture != null && rawImage != null)
            {
                rawImage.texture = webcamTexture;
                webcamTexture.Play();
            }
        }
        else
        {
            Debug.LogError("Belirtilen indekse sahip bir kamera bulunamadı.");
        }

        ust_renk_sinir.SetActive(false);
        alt_renk_sinir.SetActive(false);
        sag_renk_sinir.SetActive(false);
        sol_renk_sinir.SetActive(false);
    }

    public void otonom_ac()
    {
        genel_otonom = true;
        otonom_kamera_aci = 125;
        Debug.LogWarning("Otonom acildi");
        CancelInvoke("AnalyzeColorIntensityAndCenter");
        InvokeRepeating("AnalyzeColorIntensityAndCenter", 1.0f, 0.25f);
    }
    public void otonom_kapat()
    {
        genel_otonom = false;
        Debug.LogWarning("Otonom kapatıldı");
        CancelInvoke("AnalyzeColorIntensityAndCenter");
    }

    /*void AnalyzeColorIntensityAndCenter()
    {
        if (webcamTexture == null)
        {
            Debug.LogError("Webcam texture is not available.");
            return;
        }

        int totalX = 0;
        int totalY = 0;
        int count = 0;

        pixels = webcamTexture.GetPixels32();

        for (int i = 0; i < pixels.Length; i++)
        {
            pixelColor = pixels[i];

            // Aranan renk ile piksel rengi arasındaki fark kontrol ediliyor
            if (ColorDistance(pixelColor, targetColor) <= colorTolerance)
            {
                totalX += i % webcamTexture.width;
                totalY += i / webcamTexture.width;
                count++;
            }
        }

        if (count > 0)
        {
            float averageX = (float)totalX / count;
            float averageY = (float)totalY / count;

            Debug.Log("Center - X: " + averageX + " Y: " + averageY);

            // Şimdi ise bir objeyi bu koordinata ışınlama işlemi
            renk_bulundu = true;
            TeleportObjectToCenter(kirmizi_renk_orta, new Vector2(averageX, averageY));
        }
        else
        {
            Debug.Log("Target color not found.");
            renk_bulundu = false;
            TeleportObjectToCenter(kirmizi_renk_orta, new Vector2(5000, 5000));
        }
        otonom_veri_gonder();
    }

    float ColorDistance(Color a, Color b)
    {
        // Renkler arası mesafe hesaplama
        return Mathf.Pow(a.r - b.r, 2) + Mathf.Pow(a.g - b.g, 2) + Mathf.Pow(a.b - b.b, 2);
    }*/




    void AnalyzeColorIntensityAndCenter()
    {
        if (webcamTexture == null)
        {
            Debug.LogError("Webcam texture is not available.");
            return;
        }

        int totalX = 0;
        int totalY = 0;
        int count = 0;

        pixels = webcamTexture.GetPixels32();

        float colorTolerance = 0.3f; // Varsayılan değer, 0 ile 1 arasında ayarlanabilir

        for (int y = 0; y < webcamTexture.height; y++)
        {
            for (int x = 0; x < webcamTexture.width; x++)
            {
                int index = y * webcamTexture.width + x;
                Color pixelColor = pixels[index];

                // Pikselin rengini HSV renk uzayına dönüştürme
                float H, S, V;
                Color.RGBToHSV(pixelColor, out H, out S, out V);
                // Kırmızı tonunu daha dar bir aralıkta algılama (ton, doygunluk ve değer değerlerini ayarlama)
                if ((H >= 0.98f || H <= 0.02f) && S > 0.5f && V > 0.3f && ColorDistance(pixelColor, targetColor) <= colorTolerance)
                {
                    totalX += x;
                    totalY += y;
                    count++;
                }
            }
        }

        if (count > 0)
        {
            float averageX = (float)totalX / count;
            float averageY = (float)totalY / count;

            Debug.Log("Center - X: " + averageX + " Y: " + averageY);

            // Şimdi ise bir objeyi bu koordinata ışınlama işlemi
            renk_bulundu = true;
            TeleportObjectToCenter(kirmizi_renk_orta, new Vector2(averageX, averageY));
        }
        else
        {
            Debug.Log("Target color not found.");
            renk_bulundu = false;
            TeleportObjectToCenter(kirmizi_renk_orta, new Vector2(5000, 5000));

        }
        otonom_veri_gonder();
    }

    float ColorDistance(Color a, Color b)
    {
        // Renkler arası mesafe hesaplama
        return Mathf.Pow(a.r - b.r, 2) + Mathf.Pow(a.g - b.g, 2) + Mathf.Pow(a.b - b.b, 2);
    }






    float ColorIntensity(Color a, Color b)
    {
        // Renk yoğunluğu hesaplama
        return Mathf.Abs(a.r - b.r) + Mathf.Abs(a.g - b.g) + Mathf.Abs(a.b - b.b);
    }

    void TeleportObjectToCenter(GameObject obj, Vector2 center)
    {
        // Objeyi koordinata ışınlama işlemi

        //x in max ı 1280     asil max 743
        //y nin max ı 720     asil max 418
        float obj_max_x = 1920;
        float obj_max_y = 1080;

        obje_asil_x = ((center.x * obj_max_x) / 1280) - (obj_max_x / 2);
        obje_asil_y = ((center.y * obj_max_y) / 720) - (obj_max_y / 2);
        Vector3 new_pos = new Vector3(obje_asil_x, obje_asil_y, 0);

        if (obj != null)
        {
            RectTransform obt_rect_transform;
            if (obj.TryGetComponent<RectTransform>(out obt_rect_transform))
            {
                obt_rect_transform.anchoredPosition = new_pos;
            }
            else
            {
                obj.transform.position = new_pos;
            }
        }
        
    }


    void otonom_veri_gonder()
    {
        ust_renk_sinir.SetActive(true);
        alt_renk_sinir.SetActive(true);
        sag_renk_sinir.SetActive(true);
        sol_renk_sinir.SetActive(true);

        RectTransform ust_rect_sinir = ust_renk_sinir.GetComponent<RectTransform>();
        RectTransform alt_rect_sinir = alt_renk_sinir.GetComponent<RectTransform>();
        RectTransform sag_rect_sinir = sag_renk_sinir.GetComponent<RectTransform>();
        RectTransform sol_rect_sinir = sol_renk_sinir.GetComponent<RectTransform>();

        ust_rect_sinir.anchoredPosition = new Vector2(0, y_sinir_uzunluk);
        ust_rect_sinir.sizeDelta = new Vector2(x_sinir_uzunluk * 2, 10);

        alt_rect_sinir.anchoredPosition = new Vector2(0, -y_sinir_uzunluk);
        alt_rect_sinir.sizeDelta = new Vector2(x_sinir_uzunluk * 2, 10);

        sag_rect_sinir.anchoredPosition = new Vector2(x_sinir_uzunluk, 0);
        sag_rect_sinir.sizeDelta = new Vector2(10, y_sinir_uzunluk * 2);

        sol_rect_sinir.anchoredPosition = new Vector2(-x_sinir_uzunluk, 0);
        sol_rect_sinir.sizeDelta = new Vector2(10, y_sinir_uzunluk * 2);



        if (renk_bulundu == true)
        {
            if (obje_asil_x > x_sinir_uzunluk) //saga don
            {
                ust_renk_sinir.GetComponent<Image>().color = Color.white;
                alt_renk_sinir.GetComponent<Image>().color = Color.white;
                sag_renk_sinir.GetComponent<Image>().color = Color.red;
                sol_renk_sinir.GetComponent<Image>().color = Color.white;

                otonom_kumanda_veri[0] = 100; //throttle
                otonom_kumanda_veri[2] = 120; //rol
                otonom_kumanda_veri[3] = 100; //pitch
            }
            else if (obje_asil_x < -x_sinir_uzunluk) //sola don
            {
                ust_renk_sinir.GetComponent<Image>().color = Color.white;
                alt_renk_sinir.GetComponent<Image>().color = Color.white;
                sag_renk_sinir.GetComponent<Image>().color = Color.white;
                sol_renk_sinir.GetComponent<Image>().color = Color.red;

                otonom_kumanda_veri[0] = 100; //throttle
                otonom_kumanda_veri[2] = 80; //rol
                otonom_kumanda_veri[3] = 100; //pitch
            }
            else
            {
                sag_renk_sinir.GetComponent<Image>().color = Color.green;
                sol_renk_sinir.GetComponent<Image>().color = Color.green;

                if (obje_asil_y > y_sinir_uzunluk) //hızlı ileri git
                {
                    ust_renk_sinir.GetComponent<Image>().color = Color.red;
                    alt_renk_sinir.GetComponent<Image>().color = Color.white;

                    otonom_kumanda_veri[0] = 100; //throttle
                    otonom_kumanda_veri[2] = 100; //rol
                    otonom_kumanda_veri[3] = 150; //pitch
                }
                else if (obje_asil_y < -y_sinir_uzunluk) //kamera aci azalt en az ise in
                {
                    ust_renk_sinir.GetComponent<Image>().color = Color.white;
                    alt_renk_sinir.GetComponent<Image>().color = Color.red;

                    
                    if (otonom_kamera_aci <= 5)
                    {
                        otonom_kamera_aci = 0;

                        otonom_kumanda_veri[0] = 50; //throttle
                        otonom_kumanda_veri[2] = 100; //rol
                        otonom_kumanda_veri[3] = 100; //pitch
                    }
                    else
                    {
                        otonom_kamera_aci -= 5;
                        otonom_kumanda_veri[0] = 100; //throttle
                        otonom_kumanda_veri[2] = 100; //rol
                        otonom_kumanda_veri[3] = 110; //pitch
                    }
                }
                else //ileri git
                {
                    ust_renk_sinir.GetComponent<Image>().color = Color.green;
                    alt_renk_sinir.GetComponent<Image>().color = Color.green;

                    otonom_kumanda_veri[0] = 100; //throttle
                    otonom_kumanda_veri[2] = 100; //rol
                    otonom_kumanda_veri[3] = 120; //pitch
                }
            }




            byte toplam = 0;

            //otonom_kumanda_veri[0] = 100; //throttle
            otonom_kumanda_veri[1] = 100; //yaw
            //otonom_kumanda_veri[2] = 100; //rol
            //otonom_kumanda_veri[3] = 100; //pitch

            otonom_kumanda_veri[4] = 2; //sd
            otonom_kumanda_veri[5] = 0; //sc
            otonom_kumanda_veri[6] = 0; //sb
            otonom_kumanda_veri[7] = 0; //sg
            otonom_kumanda_veri[8] = 0; //sh

            toplam = (byte)((otonom_kumanda_veri[0] / 4) + (otonom_kumanda_veri[1] / 4) + (otonom_kumanda_veri[2] / 4) + (otonom_kumanda_veri[3] / 4));
            otonom_kumanda_veri[9] = toplam; //toplam

            otonom_kumanda_veri[10] = otonom_kamera_aci;
        }
        else //kırmızı renk ara
        {
            ust_renk_sinir.GetComponent<Image>().color = Color.red;
            alt_renk_sinir.GetComponent<Image>().color = Color.red;
            sag_renk_sinir.GetComponent<Image>().color = Color.red;
            sol_renk_sinir.GetComponent<Image>().color = Color.red;


            byte toplam = 0;

            otonom_kumanda_veri[0] = 100; //throttle
            otonom_kumanda_veri[1] = 100; //yaw
            otonom_kumanda_veri[2] = 100; //rol
            otonom_kumanda_veri[3] = 100; //pitch

            otonom_kumanda_veri[4] = 2; //sd
            otonom_kumanda_veri[5] = 0; //sc
            otonom_kumanda_veri[6] = 0; //sb
            otonom_kumanda_veri[7] = 0; //sg
            otonom_kumanda_veri[8] = 0; //sh

            toplam = (byte)((otonom_kumanda_veri[0] / 4) + (otonom_kumanda_veri[1] / 4) + (otonom_kumanda_veri[2] / 4) + (otonom_kumanda_veri[3] / 4));
            otonom_kumanda_veri[9] = toplam; //toplam

            otonom_kamera_aci = 125; //bu sıkıntı yaratırsa eşitleme
            otonom_kumanda_veri[10] = otonom_kamera_aci; //aciyi sifirla
        }

        arduino_control.serialPort.Write(otonom_kumanda_veri, 0, otonom_kumanda_veri.Length);
        gonderilen_veri_sayisi++;



    }
}
