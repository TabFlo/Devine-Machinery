#include "HardwareSerial.h"
#include "LED.h"

LED::LED(int pinNumber, int numLeds,  int blinkThreshold): leds(numLeds, pinNumber, NEO_GRB + NEO_KHZ800) { // Initialize WS2812FX correctly
  this->pin = pinNumber; 
  pinMode(pin, OUTPUT); 
  this->numLeds = numLeds;
  this->blinkThreshold = blinkThreshold; 
  leds.init();  // Initialize the LED strip
}

// Setters
void LED::setColor(int r, int g, int b){
  currentColor = leds.Color(r, g, b);
}

// Led Methods
void LED::on(){
  leds.fill(currentColor, 0, numLeds);
  leds.show();
}

void LED::off(){
    leds.fill(leds.Color(0, 0, 0), 0, numLeds); //TODO: Make this a parameter
    leds.show();
}

void LED::blink(int delayTime){
    if(currentColor == leds.Color(0,0,0) || deltaTime < blinkThreshold){
      return; 
    }

    Serial.println("Blink");
    this->off(); 
    delay(delayTime);
    this->on();
    deltaTime = 0; 
}

void LED::UpdateTime(int deltaTime){
  this->deltaTime += deltaTime; 
}

