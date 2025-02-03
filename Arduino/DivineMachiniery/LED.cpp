#include "HardwareSerial.h"
#include "LED.h"

LED::LED(int pinNumber, int numLeds,  int blinkThreshold): leds(numLeds, pinNumber, NEO_GRB + NEO_KHZ800) { // Initialize WS2812FX correctly
  this->pin = pinNumber; 
  pinMode(pin, OUTPUT); 
  this->numLeds = numLeds;
  this->blinkThreshold = blinkThreshold; 
  this->currentBlinkTresh = blinkThreshold; 
  this->state = 0;
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

void LED::Update(int deltaTime){
  //Serial.println(currentColor);
  this->deltaTime += deltaTime; 
  switch(this->state){
    case 0: 
      if(currentColor =! leds.Color(0, 0, 0)){ off(); }
      currentColor = leds.Color(0, 0, 0);
      break; 
    case 1:  
      if(currentColor == leds.Color(0, 0, 0)){ on(); }
      break; 
    case 2: 
      blink();
      break; 
    case 3:
      flash(); 
      break; 
    case 4: 
      wave(); 
      break;
    default: 
      off(); 
  }
  leds.show(); 
}


void LED::blink(){
    if(currentColor == leds.Color(0,0,0) || deltaTime < blinkThreshold){
      return; 
    }

    Serial.println("Blink");
    this->off(); 
    delay(200);
    this->on();
    deltaTime = 0; 
    currentBlinkTresh = blinkThreshold + 1000 * (rand() / (float)RAND_MAX) * 2.0 - 1.0;
}

void LED::SetState(String state){
  if(state.equals("ON")){
    this->state = 1; 
  }
  else if (state.equals("OFF")){
    this->state = 0; 
  }
  else if (state.equals("BLINK")){
    this->state = 2; 
  }
   else if (state.equals("FLASH")){
    this->state = 3; 
  }
   else if (state.equals("WAVE")){
    this->state = 4; 
  }
}

void LED::flash(){

}

void LED::wave(){

}

