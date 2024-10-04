double pitch_pid_i, pitch_last_error;
double pitch_control_signal;

double roll_pid_i, roll_last_error;
double roll_control_signal;

double throttle_pid_i, throttle_last_error;
double throttle_control_signal;

bool arac_durdu = false;

float asil_throttle = 0;

int ilk_derinlik = 0;
float hedef_derinlik = 0;

float dead_throttle;
float throttle_degisim_speed = 0.055; //0.01 de saniyede 10 santim
float throttle_dead_line = 10;
float zaman_2 = 0;

float Motor_5_Power_deger = 1500;
float Motor_6_Power_deger = 1500;
float Motor_7_Power_deger = 1500;
float Motor_8_Power_deger = 1500;

void ilk_derilik_belirle()
{
  ilk_derinlik = derinlik();
}
void hedef_derinlik_sifirla()
{
  hedef_derinlik = derinlik() - ilk_derinlik;
}
void throttle_degisim_ayarla(double degisim_speed_veri)
{
  throttle_degisim_speed = degisim_speed_veri;
}

void resetPidVariables() 
{
  pitch_pid_i = 0;
  pitch_last_error = 0;

  roll_pid_i = 0;
  roll_last_error = 0;

  throttle_pid_i = 0;
  throttle_last_error = 0;
}

struct MotorPowers calculateMotorPowers(byte throttle, struct IMU_Values imu_values) 
{
  
  
  double pitchError = 0 - imu_values.CurrentOrientation.PitchAngle; //su altını tam ortala
  double rollError = 0 - imu_values.CurrentOrientation.RollAngle;
  

  pitch_control_signal = getControlSignal(pitchError, KP_pitch, KI_pitch, KD_pitch, pitch_pid_i, pitch_last_error, imu_values.DeltaTimeInSeconds);
  roll_control_signal = getControlSignal(rollError, KP_roll, KI_roll, KD_roll, roll_pid_i, roll_last_error, imu_values.DeltaTimeInSeconds);
  
  
  /*if(throttle_control_signal > 400)
  {
    throttle_control_signal = 400;
  }
  else if(throttle_control_signal < -400)
  {
    throttle_control_signal
  }*/
  
  struct MotorPowers motorPowers;
  

  asil_throttle = map(throttle, kumanda_min_deger, kumanda_max_deger, motor_min_deger, motor_max_deger);

  if(btn_sc == 0)
  {
    Motor_5_Power_deger = asil_throttle;
    Motor_6_Power_deger = asil_throttle;//map(throttle, kumanda_min_deger, kumanda_max_deger, motor_max_deger, motor_min_deger);
    Motor_7_Power_deger = asil_throttle;
    Motor_8_Power_deger = asil_throttle;
    hedef_derinlik_sifirla();
    resetPidVariables();
  }
  else if(btn_sc == 1)
  {
    Motor_5_Power_deger = asil_throttle - pitch_control_signal - roll_control_signal;
    Motor_6_Power_deger = asil_throttle + pitch_control_signal - roll_control_signal;//map(throttle, kumanda_min_deger, kumanda_max_deger, motor_max_deger, motor_min_deger)
    Motor_7_Power_deger = asil_throttle - pitch_control_signal + roll_control_signal;
    Motor_8_Power_deger = asil_throttle + pitch_control_signal + roll_control_signal;
    hedef_derinlik_sifirla();
  }
  else if(btn_sc == 2)
  {
    if(millis() - zaman_2 > 1)
    {
      dead_throttle = throttle;
      if(dead_throttle < (100 + throttle_dead_line) && dead_throttle > (100 - throttle_dead_line))
      {
        dead_throttle = 100;
        /*if(arac_durdu == false)
        {
          hedef_derinlik_sifirla();
          arac_durdu = true;
        }*/
      }
      else
      {
        //arac_durdu = false;
      }
      hedef_derinlik += (dead_throttle - 100) * throttle_degisim_speed;
      zaman_2 = millis();
    }
    
    double throttleError = ((int)(hedef_derinlik)) - (derinlik() - ilk_derinlik);//hedef derinlik vs yeri

    throttle_control_signal = getControlSignal(throttleError, KP_throttle, KI_throttle, KD_throttle, throttle_pid_i, throttle_last_error, imu_values.DeltaTimeInSeconds);

    
  
    Motor_5_Power_deger = 1500 + throttle_control_signal ;//- pitch_control_signal - roll_control_signal;
    Motor_6_Power_deger = 1500 + throttle_control_signal ;//+ pitch_control_signal - roll_control_signal;//map(throttle, kumanda_min_deger, kumanda_max_deger, motor_max_deger, motor_min_deger)
    Motor_7_Power_deger = 1500 + throttle_control_signal ;//- pitch_control_signal + roll_control_signal;
    Motor_8_Power_deger = 1500 + throttle_control_signal ;//+ pitch_control_signal + roll_control_signal;

    Motor_5_Power_deger = constrain(Motor_5_Power_deger, 1100, 1900);
    Motor_6_Power_deger = constrain(Motor_6_Power_deger, 1100, 1900);
    Motor_7_Power_deger = constrain(Motor_7_Power_deger, 1100, 1900);
    Motor_8_Power_deger = constrain(Motor_8_Power_deger, 1100, 1900);

    Motor_5_Power_deger = Motor_5_Power_deger - pitch_control_signal - roll_control_signal;
    Motor_6_Power_deger = Motor_6_Power_deger + pitch_control_signal - roll_control_signal;
    Motor_7_Power_deger = Motor_7_Power_deger - pitch_control_signal + roll_control_signal;
    Motor_8_Power_deger = Motor_8_Power_deger + pitch_control_signal + roll_control_signal;
  }
  

  Motor_5_Power_deger = constrain(Motor_5_Power_deger, motor_min_deger, motor_max_deger);
  Motor_6_Power_deger = constrain(Motor_6_Power_deger, motor_min_deger, motor_max_deger);
  Motor_7_Power_deger = constrain(Motor_7_Power_deger, motor_min_deger, motor_max_deger);
  Motor_8_Power_deger = constrain(Motor_8_Power_deger, motor_min_deger, motor_max_deger);


  
  motorPowers.Motor_5_Power = Motor_5_Power_deger;
  motorPowers.Motor_6_Power = Motor_6_Power_deger;
  motorPowers.Motor_7_Power = Motor_7_Power_deger;
  motorPowers.Motor_8_Power = Motor_8_Power_deger;

  return motorPowers;
}
