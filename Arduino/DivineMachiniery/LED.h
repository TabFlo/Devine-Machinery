#ifndef LED_H
#define LED_H

#include <WS2812FX.h>
#include <Arduino.h>
#include <Adafruit_NeoPixel.h>

class LED {
  private: 
    int pin; 
    uint32_t  currentColor; 
    int numLeds;
    int deltaTime; 
    int blinkThreshold; 
    int currentBlinkTresh; 
    int state; 
    WS2812FX leds;

  public: 
    LED(int pinNumber, int numLeds, int blinkThreshold);

    void setColor(int r, int g, int b);
    void on();
    void off(); 
    void blink(); 
    void Update(int deltaTime);
    void SetState(String state); 

  private: 
    void flash(); 
    void wave(); 
};

#endif