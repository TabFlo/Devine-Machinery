#include <FastLED.h>
#include "LED.h"

#define NUM_EYELEDS 12
#define EYELED_PIN 3

LED eyeLed(3, 12); 
  

void setup() {

  Serial.begin(9600); 
  Serial.println("Arduino Setup");

  //init Tof 
  //init Cap 
  //init LEDs
  eyeLed.setColor(255, 0, 0);
  eyeLed.on();

  // put your setup code here, to run once:

}


void loop() {
  String data; 
  if (Serial.available()> 0){
    data = Serial.readStringUntil('\n');
    //Serial.println("Data: " + data);
  }

  // put your main code here, to run repeatedly:

  // get input enable flag from Unity
    // read tof data 
    // read cap data 
      // if Input, send

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
    Serial.println("color " + String(r) + " " + String(g) + " " + String(b));

    return prefix;
}
