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
    WS2812FX leds;

  public: 
    LED(int pinNumber, int numLeds);

    void setColor(int r, int g, int b);
    void on();
    void off(); 
};

#endif