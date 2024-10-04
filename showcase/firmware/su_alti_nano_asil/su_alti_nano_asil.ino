#include <Servo.h>



Servo servo_1;  //sağ üst
Servo servo_2;  //sol üst
Servo servo_3;  //sağ alt
Servo servo_4;  //sol alt
Servo servo_5;  //orta sağ üst
Servo servo_6;  //orta sol üst
Servo servo_7;  //orta sağ alt
Servo servo_8;  //orta sol alt

Servo firlatici_1;
Servo firlatici_2;

Servo servo_kamera;
Servo servo_silecek;


float orta_motor_deger = 1500;

float servo_1_deger = 1500;
float servo_2_deger = 1500;
float servo_3_deger = 1500;
float servo_4_deger = 1500;

float servo_1_kalan = 0;
float servo_2_kalan = 0;
float servo_3_kalan = 0;
float servo_4_kalan = 0;



int kumanda_max_deger = 200;
int kumanda_min_deger = 0;

int motor_max_deger = 2000;
int motor_min_deger = 1000;


float kamera_derece = 60;
float silecek_derece = 10;


float throttle_speed = 1; //max 1
float pitch_speed = 1; //max 1
float yaw_speed = 1; //max 1
float rol_speed = 1; //max 1

float kamera_derece_speed = 0.050;
float silecek_derece_speed = 0.100;


bool baslangic = true;
bool cam_silindi = false;

byte araci_veri[20] = {0, 0, 100, 100, 100, 100, 0, 0, 0, 0, 0, 0, 0, 100, 5, 4, 10, 0, 4, 55};
double zaman = 0;
double zaman_3 = 0;
double zaman_4 = 0;
byte throttle;
byte yaw;
byte rol;
byte pitch;

byte btn_sd;
byte btn_sc;
byte btn_sb;
byte btn_sg;
byte btn_sh;
byte btn_se;




float gyro_pitch = 0;
float gyro_rol = 0;
float gyro_yaw = 0;


int hangi_deger_yazildi = 0;


struct Orientation {
  double YawAngle;
  double PitchAngle;
  double RollAngle;
};

struct IMU_Values {
  bool Error;
  bool NewDataAvailable;
  double DeltaTimeInSeconds;
  struct Orientation CurrentOrientation;
  struct Orientation PreviousOrientation;
};


/*#define KP_pitch 10//10.00
#define KI_pitch 0.5
#define KD_pitch 0.4

#define KP_roll 10//10.00
#define KI_roll 0.5
#define KD_roll 0.4*/

double KP_pitch = 10;
double KI_pitch = 0.5;
double KD_pitch = 0.4;

double KP_roll = 10;
double KI_roll = 0.5;
double KD_roll = 0.4;

double KP_throttle = 5;
double KI_throttle = 0.0;
double KD_throttle = 0.0;

struct MotorPowers 
{
  int Motor_5_Power;
  int Motor_6_Power;
  int Motor_7_Power;
  int Motor_8_Power;
};



void setup() 
{
  
  Serial.begin(9600);

  //initializeIMU();
  //ilk_derilik_belirle();
  
  
  servo_1.attach(17);//
  servo_2.attach(12);//
  servo_3.attach(3);//
  servo_4.attach(4);//
  servo_5.attach(15);//
  servo_6.attach(13);//
  servo_7.attach(16);//
  servo_8.attach(14);//

  firlatici_1.attach(16);

  //servo_kamera.attach(1);
  //servo_silecek.attach(2);

  delay(1000);
  pinMode(9, OUTPUT);
  pinMode(11, OUTPUT);
  analogWrite(11, 0);
  delay(1000);
  analogWrite(11, 155);
  delay(1000);
  analogWrite(11, 0);
  delay(1000);
  analogWrite(11, 155);
  delay(1000);
  analogWrite(11, 0);
  
  digitalWrite(9,LOW);//low yap

  servo_1.writeMicroseconds(1500);
  servo_2.writeMicroseconds(1500);
  servo_3.writeMicroseconds(1500);
  servo_4.writeMicroseconds(1500);
  servo_5.writeMicroseconds(1500);
  servo_6.writeMicroseconds(1500);
  servo_7.writeMicroseconds(1500); 
  servo_8.writeMicroseconds(1500);

  firlatici_1.writeMicroseconds(1500);
  delay(5);

  //servo_kamera.write(60);//60
  //servo_silecek.write(60); //10 // silecek acilarini ogrenince ac

  //delay(5000); //motor kalibre için bekleme
  
  /*servo_5.writeMicroseconds(1600);
  delay(2000);
  servo_5.writeMicroseconds(1500);*/
  //Serial.println("motorlar ayarlandi");


  
  /*servo_1.writeMicroseconds(1600);
  delay(2000);
  servo_1.writeMicroseconds(1500);
  servo_2.writeMicroseconds(1600);
  delay(2000);
  servo_2.writeMicroseconds(1500);
  servo_3.writeMicroseconds(1600);
  delay(2000);
  servo_3.writeMicroseconds(1500);
  servo_4.writeMicroseconds(1600);
  delay(2000);
  servo_4.writeMicroseconds(1500);
  servo_5.writeMicroseconds(1600);
  delay(2000);
  servo_5.writeMicroseconds(1500);
  servo_6.writeMicroseconds(1600);
  delay(2000);
  servo_6.writeMicroseconds(1500);
  servo_7.writeMicroseconds(1600);
  delay(2000);
  servo_7.writeMicroseconds(1500);
  servo_8.writeMicroseconds(1600);
  delay(2000);
  servo_8.writeMicroseconds(1500);*/
}

void loop() 
{
  //firlatici_1.writeMicroseconds(1600);
  serialEvent();
}

void serialEvent()
{
  if(Serial.parseInt() == 256)
  {
    Serial.readBytes(araci_veri, 20);
    if(araci_veri[12] == (araci_veri[2] / 4) + (araci_veri[3] / 4) + (araci_veri[4] / 4) + (araci_veri[5] / 4))
    {
      throttle = (araci_veri[2] * throttle_speed) + (100 - (throttle_speed * 100));
      yaw = (araci_veri[3] * yaw_speed) + (100 - (yaw_speed * 100));
      rol = (araci_veri[4] * rol_speed) + (100 - (rol_speed * 100));
      pitch = (araci_veri[5] * pitch_speed) + (100 - (pitch_speed * 100));

      
      btn_sd = araci_veri[6];
      btn_sc = araci_veri[7]; 
      btn_sb = araci_veri[8]; //kamera buton
      btn_sg = araci_veri[9];
      btn_sh = araci_veri[10];
      btn_se = araci_veri[11];


      KP_pitch = double(araci_veri[13] / 10);
      KP_roll = double(araci_veri[13] / 10);
      
      KI_pitch = double(araci_veri[14] / 10);
      KI_roll = double(araci_veri[14] / 10);

      KD_pitch = double(araci_veri[15] / 10);
      KD_roll = double(araci_veri[15] / 10);



      KP_throttle = double(araci_veri[16] / 10);
      
      KI_throttle = double(araci_veri[17] / 10);

      KD_throttle = double(araci_veri[18] / 10);

      throttle_degisim_ayarla(araci_veri[19] / 1000.0);
      
    }
  }
  
  if(btn_sh == 2)
  {
    if(btn_sd == 0) //yüzde 100 güçte çalıştır
    {
      throttle_speed = 1.0;
      pitch_speed = 1.0;
      yaw_speed = 1.0;
      rol_speed = 1.0;
    }
    else if(btn_sd == 1) //yüzde 90 güçte çalıştır
    {
      throttle_speed = 0.9;
      pitch_speed = 0.9;
      yaw_speed = 0.9;
      rol_speed = 0.9;
    }
    else if(btn_sd == 2) //yüzde 80 güçte çalıştır
    {
      throttle_speed = 0.8;
      pitch_speed = 0.8;
      yaw_speed = 0.8;
      rol_speed = 0.8;
    }

    if(btn_sg == 2)
    {
      if(btn_sc == 1)
      {
        resetPidVariables();
      }
      else if(btn_sc == 2)
      {
        hedef_derinlik_sifirla();
        resetPidVariables();
      }
    }
    
    motor_calistir();
  }
  else
  {
    motor_durdur();
    hedef_derinlik_sifirla();
    resetPidVariables();
  }
  
  /*if(millis() - zaman  > 500) //veri yollama kısmı
  {
    struct IMU_Values imuValues = GetIMU_Values();
    digitalWrite(9, HIGH);
    delay(5);//5
    gyro_veri_gonder();
    barometre_deger_gonder(); //ayarla
    delay(30);//30
    digitalWrite(9, LOW);
    //delay(50);
    zaman = millis();
  }*/

  /*   //kamera kısmı servoları taktığında burayı aç

  if(btn_sb == 0) //kamera kismi
  {
    if(millis() - zaman_3  > 1) //kamera hız ayarlama kismi
    {
      kamera_derece -= kamera_derece_speed;
      zaman_3 = millis();
    }
  }
  else if(btn_sb == 2)
  {
    if(millis() - zaman_3  > 1) //kamera hız ayarlama kismi
    {
      kamera_derece += kamera_derece_speed;
      zaman_3 = millis();
    }
  }
  //servo_kamera.write(kamera_derece);

  if(btn_se > 0) //silecek kismi
  {
    //kamerayı indir ardından sileceği aşa yukarı yapıp kamerayı eski konumuna çek
  }

  
  
  */
}

void motor_calistir()
{
  struct IMU_Values imuValues = GetIMU_Values();

  struct MotorPowers motorPowers = calculateMotorPowers(throttle, imuValues);
  
  servo_5.writeMicroseconds(motorPowers.Motor_5_Power);                            //PID'li orta motor kontrol asil kontrol
  servo_6.writeMicroseconds(motorPowers.Motor_6_Power);
  servo_7.writeMicroseconds(motorPowers.Motor_7_Power);
  servo_8.writeMicroseconds(motorPowers.Motor_8_Power);

  
  servo_1_deger = -1500 + (map(pitch, kumanda_min_deger, kumanda_max_deger, motor_min_deger, motor_max_deger)) + 
                          (map(rol, kumanda_min_deger, kumanda_max_deger, motor_max_deger, motor_min_deger));
  servo_1_deger = constrain(servo_1_deger, 1000, 2000);


  
  servo_2_deger = -1500 + (map(pitch, kumanda_min_deger, kumanda_max_deger, motor_min_deger, motor_max_deger)) + 
                          (map(rol, kumanda_min_deger, kumanda_max_deger, motor_max_deger, motor_min_deger));
  servo_2_deger = constrain(servo_2_deger, 1000, 2000);


  
  servo_3_deger = -1500 + (map(pitch, kumanda_min_deger, kumanda_max_deger, motor_min_deger, motor_max_deger)) + 
                          (map(rol, kumanda_min_deger, kumanda_max_deger, motor_min_deger, motor_max_deger));
  servo_3_deger = constrain(servo_3_deger, 1000, 2000);


  
  servo_4_deger = -1500 + (map(pitch, kumanda_min_deger, kumanda_max_deger, motor_min_deger, motor_max_deger)) + 
                          (map(rol, kumanda_min_deger, kumanda_max_deger, motor_min_deger, motor_max_deger));
  servo_4_deger = constrain(servo_4_deger, 1000, 2000);
  


  servo_1.writeMicroseconds(servo_1_deger);
  servo_2.writeMicroseconds(servo_2_deger);
  servo_3.writeMicroseconds(servo_3_deger);
  servo_4.writeMicroseconds(servo_4_deger);

}

void motor_durdur()
{
  servo_1.writeMicroseconds(1500);
  servo_2.writeMicroseconds(1500);
  servo_3.writeMicroseconds(1500);
  servo_4.writeMicroseconds(1500);
  servo_5.writeMicroseconds(1500);
  servo_6.writeMicroseconds(1500);
  servo_7.writeMicroseconds(1500);
  servo_8.writeMicroseconds(1500);
}
