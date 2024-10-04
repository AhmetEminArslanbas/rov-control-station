#include <Wire.h>
#include "MS5837.h"

MS5837 sensor;


void barometre_deger_gonder()
{
  sensor.read();

  Serial.print("|");
  Serial.print((sensor.depth() * 100));
}

int derinlik()
{
  sensor.read();

  return (int)(sensor.depth() * 100);
}
