# Meadow LED Hoodie

This project runs an APA102 RGB LED strip using a [Wilderness Labs Meadow microcontroller](https://store.wildernesslabs.co/).

For DEF CON 32 (2024), I adapted my regular Project Lab badge to also drive an RGB LED strip sewn into the edge of the hood on a hoodie (hooded sweatshirt). This was fun, but also implemented in a way that was kind of goofy to the rest of the badge code. I extracted and refactored that code into this stand-alone project where I plan to continue to add some features.

## Current Setup

* Code run on a Meadow Project Lab IoT prototyping platform
* Lights are 51 APA102 (sized to fit this exact hoodie's drawstring channel length)
* APA102 lights are driven from the SPI bus on one of the Project Lab platform's mikroBUS connectors
* Project Lab and LED strip powered via USB C power breakout port, split via Wago connectors to the Project Lab power and ground screw terminals and the LED strip's power and ground wires

    > NOTE: You can power several LEDs directly from the Project Lab, but this setup avoids the risk of drawing too much current from the Project Lab itself. Check your own current requirements and adjust accordingly.

## Future Plans

* Create version that runs from a Meadow F7 Feather board, in this case crammed carefully into an Altoids tin
* Create system for lighting by algorithm, so each LED is controlled by a time-based calculation rather than direct LED-control logic (probably not as amazing as ElectroMage's Pixelblaze, but hopefully offering some tiny subset of those features.)
