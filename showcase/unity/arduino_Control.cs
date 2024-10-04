using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.Text;
using System;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class arduino_Control : MonoBehaviour
{
    [SerializeField] GamepadManager gamepad_manager;
    [SerializeField] Canvas_manager canvas_manager;
    [SerializeField] su_alti_camera su_alti_camera;
    [SerializeField] GameObject sanal_su_alti;
    [SerializeField] Image gyro_z_image;

    [SerializeField] float gyro_x;
    [SerializeField] float gyro_y;
    [SerializeField] float gyro_z;

    [SerializeField] float barometre_deger;


    [SerializeField] int gelen_veri_sayisi;

    public SerialPort serialPort;
    string com;

    string gelen_deger;

    [SerializeField] byte[] gonderilcek_veri = new byte[18]; //new byte[10];
    [SerializeField] byte toplam = 0;

    [SerializeField] byte throttle;
    [SerializeField] byte yaw;
    [SerializeField] byte rol;
    [SerializeField] byte pitch;

    [SerializeField] byte se;
    [SerializeField] byte sd;
    [SerializeField] byte sc;
    [SerializeField] byte sb;
    [SerializeField] byte sg;
    [SerializeField] byte sh;

    [SerializeField] float pitch_roll_p_deger, pitch_roll_i_deger, pitch_roll_d_deger;
    [SerializeField] float throttle_p_deger, throttle_i_deger, throttle_d_deger, throttle_stick_speed_deger;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        com = canvas_manager.com_field.text;

        serialPort = new SerialPort("COM" + com, 9600); // Arduino'nun bağlı olduğu port ve baud hızı
        Debug.Log("COM" + com);
        serialPort.Open();
        Debug.Log("Seri port acildi.");


        if (PlayerPrefs.GetFloat("pitch_roll_pid_p") != 0)
        {
            canvas_manager.pitch_roll_p_field.text = PlayerPrefs.GetFloat("pitch_roll_pid_p").ToString();
            pitch_roll_p_deger = PlayerPrefs.GetFloat("pitch_roll_pid_p");
        }
        if (PlayerPrefs.GetFloat("pitch_roll_pid_i") != 0)
        {
            canvas_manager.pitch_roll_i_field.text = PlayerPrefs.GetFloat("pitch_roll_pid_i").ToString();
            pitch_roll_i_deger = PlayerPrefs.GetFloat("pitch_roll_pid_i");
        }
        if (PlayerPrefs.GetFloat("pitch_roll_pid_d") != 0)
        {
            canvas_manager.pitch_roll_d_field.text = PlayerPrefs.GetFloat("pitch_roll_pid_d").ToString();
            pitch_roll_d_deger = PlayerPrefs.GetFloat("pitch_roll_pid_d");
        }


        if (PlayerPrefs.GetFloat("throttle_pid_p") != 0)
        {
            canvas_manager.throttle_p_field.text = PlayerPrefs.GetFloat("throttle_pid_p").ToString();
            throttle_p_deger = PlayerPrefs.GetFloat("throttle_pid_p");
        }
        if (PlayerPrefs.GetFloat("throttle_pid_i") != 0)
        {
            canvas_manager.throttle_i_field.text = PlayerPrefs.GetFloat("throttle_pid_i").ToString();
            throttle_i_deger = PlayerPrefs.GetFloat("throttle_pid_i");
        }
        if (PlayerPrefs.GetFloat("throttle_pid_d") != 0)
        {
            canvas_manager.throttle_d_field.text = PlayerPrefs.GetFloat("throttle_pid_d").ToString();
            throttle_d_deger = PlayerPrefs.GetFloat("throttle_pid_d");
        }


        if (PlayerPrefs.GetFloat("throttle_stick_speed") != 0)
        {
            canvas_manager.throttle_stick_speed_field.text = PlayerPrefs.GetFloat("throttle_stick_speed").ToString();
            throttle_stick_speed_deger = PlayerPrefs.GetFloat("throttle_stick_speed");
        }




        if (su_alti_camera.genel_otonom == false)
        {
            StartCoroutine(kumanda_verisi_baslat());
        }

    }
    private void Update()
    {
        su_alti_camera.gecen_sure += Time.deltaTime;

        canvas_manager.barometre_text.text = (/*(int)*/(barometre_deger)).ToString() + "CM";
    }

    IEnumerator kumanda_verisi_baslat()
    {
        while (true)
        {
            throttle = (byte)((Mathf.Round((gamepad_manager.throttle + 1) * 100) / 100) * 100);
            yaw = (byte)((Mathf.Round((gamepad_manager.yaw + 1) * 100) / 100) * 100);
            rol = (byte)((Mathf.Round((gamepad_manager.rol + 1) * 100) / 100) * 100);
            pitch = (byte)((Mathf.Round((gamepad_manager.pitch + 1) * 100) / 100) * 100);

            sd = (byte)(gamepad_manager.sd + 1);
            sc = (byte)(gamepad_manager.sc + 1);
            sb = (byte)(gamepad_manager.sb + 1);
            sg = (byte)(gamepad_manager.sg + 1);
            sh = (byte)(gamepad_manager.sh + 1);
            se = (byte)(gamepad_manager.se + 1);

            toplam = (byte)((throttle / 4) + (yaw / 4) + (rol / 4) + (pitch / 4));


            gonderilcek_veri[0] = throttle;
            gonderilcek_veri[1] = yaw;
            gonderilcek_veri[2] = rol;
            gonderilcek_veri[3] = pitch;

            gonderilcek_veri[4] = sd;
            gonderilcek_veri[5] = sc;
            gonderilcek_veri[6] = sb;
            gonderilcek_veri[7] = sg;
            gonderilcek_veri[8] = sh;
            gonderilcek_veri[9] = se;
            gonderilcek_veri[10] = toplam; //pid eri de yolla

            if (float.TryParse(canvas_manager.pitch_roll_p_field.text, out pitch_roll_p_deger))
            {
                gonderilcek_veri[11] = (byte)pitch_roll_p_deger;
                PlayerPrefs.SetFloat("pitch_roll_pid_p", pitch_roll_p_deger);

            }
            else
            {
                Debug.LogError("pitch roll pid 'p' deger hatalı: " + pitch_roll_p_deger);
                canvas_manager.pitch_roll_p_field.text = 0.ToString();
                gonderilcek_veri[11] = 0;
            }

            if (float.TryParse(canvas_manager.pitch_roll_i_field.text, out pitch_roll_i_deger))
            {
                gonderilcek_veri[12] = (byte)pitch_roll_i_deger;
                PlayerPrefs.SetFloat("pitch_roll_pid_i", pitch_roll_i_deger);
            }
            else
            {
                Debug.LogError("pitch roll pid 'i' deger hatalı: " + pitch_roll_i_deger);
                canvas_manager.pitch_roll_i_field.text = 0.ToString();
                gonderilcek_veri[12] = 0;
            }

            if (float.TryParse(canvas_manager.pitch_roll_d_field.text, out pitch_roll_d_deger))
            {
                gonderilcek_veri[13] = (byte)pitch_roll_d_deger;
                PlayerPrefs.SetFloat("pitch_roll_pid_d", pitch_roll_d_deger);
            }
            else
            {
                Debug.LogError("pitch roll pid 'd' deger hatalı: " + pitch_roll_d_deger);
                canvas_manager.pitch_roll_d_field.text = 0.ToString();
                gonderilcek_veri[13] = 0;
            }





            if (float.TryParse(canvas_manager.throttle_p_field.text, out throttle_p_deger))
            {
                gonderilcek_veri[14] = (byte)throttle_p_deger;
                PlayerPrefs.SetFloat("throttle_pid_p", throttle_p_deger);
            }
            else
            {
                Debug.LogError("throttle pid 'p' deger hatalı: " + throttle_p_deger);
                canvas_manager.throttle_p_field.text = 0.ToString();
                gonderilcek_veri[14] = 0;
            }

            if (float.TryParse(canvas_manager.throttle_i_field.text, out throttle_i_deger))
            {
                gonderilcek_veri[15] = (byte)throttle_i_deger;
                PlayerPrefs.SetFloat("throttle_pid_i", throttle_i_deger);
            }
            else
            {
                Debug.LogError("throttle pid 'i' deger hatalı: " + throttle_i_deger);
                canvas_manager.throttle_i_field.text = 0.ToString();
                gonderilcek_veri[15] = 0;
            }

            if (float.TryParse(canvas_manager.throttle_d_field.text, out throttle_d_deger))
            {
                gonderilcek_veri[16] = (byte)throttle_d_deger;
                PlayerPrefs.SetFloat("throttle_pid_d", throttle_d_deger);
            }
            else
            {
                Debug.LogError("throttle pid 'd' deger hatalı: " + throttle_d_deger);
                canvas_manager.throttle_d_field.text = 0.ToString();
                gonderilcek_veri[16] = 0;
            }



            if (float.TryParse(canvas_manager.throttle_stick_speed_field.text, out throttle_stick_speed_deger))
            {
                gonderilcek_veri[17] = (byte)throttle_stick_speed_deger;
                PlayerPrefs.SetFloat("throttle_stick_speed", throttle_stick_speed_deger);
            }
            else
            {
                Debug.LogError("throttle stick speed deger hatalı: " + throttle_stick_speed_deger);
                canvas_manager.throttle_stick_speed_field.text = 0.ToString();
                gonderilcek_veri[17] = 0;
            }






            serialPort.WriteLine("256");
            serialPort.Write(gonderilcek_veri, 0, gonderilcek_veri.Length);



            gelen_deger = serialPort.ReadExisting(); //gelen veri okuma
            gyro_veri_oku(gelen_deger);




            #region sanal su alti

            sanal_su_alti.transform.localRotation = Quaternion.Euler(gyro_x, 0, -gyro_y);

            if (gyro_z > 0)
            {
                gyro_z_image.fillClockwise = true;
                gyro_z_image.fillAmount = (gyro_z * 0.5f) / 180;
            }
            else
            {
                gyro_z_image.fillClockwise = false;
                gyro_z_image.fillAmount = (-gyro_z * 0.5f) / 180;
            }

            #endregion

            yield return new WaitForSeconds(0.1f);
        }
    }
    public void gyro_veri_oku(string veri)
    {
        if (!string.IsNullOrEmpty(veri))
        {
            string[] veriParcalari = veri.Replace(".", ",").Split('|');

            if (veriParcalari.Length == 4 &&
                float.TryParse(veriParcalari[0], out gyro_x) &&
                float.TryParse(veriParcalari[1], out gyro_y) &&
                float.TryParse(veriParcalari[1], out gyro_z) &&
                float.TryParse(veriParcalari[3], out barometre_deger))
            {

                Debug.Log(veri);
            }
            else
            {
                Debug.LogError("Geçersiz veri formatı: " + veri);
            }
        }
    }

    void OnApplicationQuit()
    {
        serialPort.Close();
    }
    private void OnDestroy()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
            Debug.Log("Seri port kapatildi.");
        }
    }
}