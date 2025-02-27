#include <Wire.h>
//#include <Adafruit_VL53L0X.h>
#include "LED.h"


#define NUM_EYELEDS 12
#define EYELED_PIN 3
#define CHEST_PIN 4

#define TOUCH_PIN_L 9

#define TOUCH_PIN_R 8

unsigned long previousTime = 0; 

//Adafruit_VL53L0X sensorL = Adafruit_VL53L0X();

LED eyeLed(EYELED_PIN, 8, 3000); 
LED chestLed(CHEST_PIN, 10, 3000); 

void setup() {

  Serial.begin(9600);
  Serial.println("Arduino Setup");

  pinMode(TOUCH_PIN_L, INPUT);

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
  //EYE
  eyeLed.setColor(255, 255, 255);
  eyeLed.on();

  //CHEST 
  chestLed.setColor(255, 255, 255);
  chestLed.on(); 

  delay(100);
  Serial.flush(); 
  // put your setup code here, to run once:
  eyeLed.SetState("BLINK");
}


void loop() {

  //Time Handling
  unsigned long currentTime = millis();
  unsigned long deltaTime = currentTime - previousTime;

  // get data from unity
  // TODO: make this faster
  String data; 
  if (Serial.available()> 0){
    data = Serial.readStringUntil('\n');
   
  }
  // read tof data 
  //readTofData();

  // read cap data 
  ReadButtonState();
  

  // LED DATA 
  // EYES 

  int r, g, b;
  String state;
  if(parseRGBA(data, r, g, b).equals("EYE")){
    eyeLed.setColor(r, g, b);
    eyeLed.on();
    Serial.println("Set Eye color to " + r);
  } 
  else if(parseStateData(data, state).equals("EYE")){ //no vieryfing if state is acutally valid because im tired
    eyeLed.SetState(state);
  }
  
  
  

  // BACK 
  // TORSO 
  if(parseRGBA(data, r, g, b).equals("CHEST")){
    chestLed.setColor(r, g, b);
    chestLed.on(); 
    Serial.println("Set Chest color to " + r);
  }
  else if(parseStateData(data, state).equals("CHEST")){ //no vieryfing if state is acutally valid because im tired
    eyeLed.SetState(state);
  }

  // TIME HANDLING
  previousTime = currentTime; 
  eyeLed.Update(deltaTime);
  delay(10);
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

String parseStateData(String input, String &state){
  int i = input.indexOf(' ');
  if (i == -1) return "-1"; // No space found, invalid input

  String prefix = input.substring(0, i);
  if(!prefix.equals("STATE")) return "-1"; 

  input = input.substring(i+1); 

  i = input.indexOf(' '); 
  prefix = input.substring(0, i);

  state = input.substring(i+1);

  Serial.println("State: " + prefix + " " + state);
  return prefix; 
}

void ReadButtonState(){
  int buttonState = digitalRead(TOUCH_PIN_L);
  if (buttonState == LOW) {
   Serial.println("TOUCH_L 1");
  } 
  else{
   Serial.println("TOUCH_L 0");
  }


  buttonState = digitalRead(TOUCH_PIN_R);
  if (buttonState == LOW) {
   Serial.println("TOUCH_R 1");
  } 
  else{
   Serial.println("TOUCH_R 0");
  }
}

void readTofData(){
  
//VL53L0X_RangingMeasurementData_t measure;

  /*sensorL.rangingTest(&measure, false);
    if (measure.RangeStatus != 4) {
      Serial.print("HAND_L "); Serial.println(measure.RangeMilliMeter);
    } else {
      Serial.print("HAND_L "); Serial.println(-1);
    }
  */
}
