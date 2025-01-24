#include <Wire.h>
//#include <Adafruit_VL53L0X.h>
#include "LED.h"


#define NUM_EYELEDS 12
#define EYELED_PIN 3

#define ToUCH_PIN_L 7

#define ToUCH_PIN_L 8

//Adafruit_VL53L0X sensorL = Adafruit_VL53L0X();

LED eyeLed(3, 12); 
  

void setup() {

  Serial.begin(115200);
  Serial.println("Arduino Setup");

  pinMode(ToUCH_PIN_L, INPUT);

  /*
  //init Tof 
  Wire.begin(); // Default I2C pins (SDA, SCL)
  if (!sensorL.begin()) {
    Serial.println("Failed to boot VL53L0X sensor 1!");
    while (1);
  }
  */

  //init Cap 
  //init LEDs
  eyeLed.setColor(255, 255, 0);
  eyeLed.on();
  delay(100);
  Serial.flush(); 
  // put your setup code here, to run once:

}


void loop() {
  String data; 
  if (Serial.available()> 0){
    data = Serial.readStringUntil('\n');
   
  }
 //Serial.println("Data: " + data);
  delay(500);
  // put your main code here, to run repeatedly:

  // get input enable flag from Unity

  /*
  // read tof data 
  VL53L0X_RangingMeasurementData_t measure;

  sensorL.rangingTest(&measure, false);
    if (measure.RangeStatus != 4) {
      Serial.print("HAND_L "); Serial.println(measure.RangeMilliMeter);
    } else {
      Serial.print("HAND_L "); Serial.println(-1);
    }
  */

  // read cap data 

  int buttonState = digitalRead(ToUCH_PIN_L);
  if (buttonState == LOW) {
   Serial.println("TOUCH_L 1");
  } 
  else{
   Serial.println("TOUCH_L 0");
  }


 int buttonState = digitalRead(ToUCH_PIN_R);
  if (buttonState == LOW) {
   Serial.println("TOUCH_R 1");
  } 
  else{
   Serial.println("TOUCH_R 0");
  }



  // sendLED data 
  // eyes 
  
  int r, g, b;
  if(parseRGBA(data, r, g, b).equals("EYE")){
    eyeLed.setColor(r, g, b);
    eyeLed.on();
  } 
    


  // Back 
  // torsp
}


String parseRGBA(String input, int &r, int &g, int &b) {
    int i = input.indexOf(' ');
    if (i == -1) return "-1"; // No space found, invalid input

    // Extract the prefix
    String prefix = input.substring(0, i);

    // Extract the remaining data after the prefix
    String data = input.substring(i + 1);

    // Find spaces within the `data` portion
    int firstSpace = data.indexOf(' ');
    int secondSpace = data.indexOf(' ', firstSpace + 1);

    // Check if enough spaces exist in the data
    if (firstSpace == -1 || secondSpace == -1) return "-1"; // Invalid input

    // Parse RGB values from `data`
    r = data.substring(0, firstSpace).toInt();
    g = data.substring(firstSpace + 1, secondSpace).toInt();
    b = data.substring(secondSpace + 1).toInt();

    // Debug output for the parsed color values
    Serial.println("color " + String(r) + " " + String(g) + " " + String(b) + " " + prefix);

    return prefix;
}
