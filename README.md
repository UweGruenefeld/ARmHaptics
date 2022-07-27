# ARm Haptics
**3D-Printed Wearable Haptics for Mobile Augmented Reality**

Uwe Gruenefeld, Alexander Geilen, Jonathan Liebers, Nick Wittig, Marion Koelle, and Stefan Schneegass

![Overview of the ARm Haptics system.](/Teaser.png?raw=true "Overview of the ARm Haptics system.")

**Figure 1: We present the ARm Haptics system, which builds upon 3D-printed wearable input modules to provide haptics for Augmented Reality. a) 3D-printed mount for input modules, b) three different input modules, c) ARm Haptics system with two mounted input modules, and d) button in Augmented Reality linked to worn input module (full blue bar indicates successful linking).**

ABSTRACT Augmented Reality (AR) technology enables users to superpose virtual content onto their environments. However, interacting with virtual content while mobile often requires users to perform interactions in mid-air, resulting in a lack of haptic feedback. Hence, in this work, we present the ARm Haptics system, which is worn on the user's forearm and provides 3D-printed input modules, each representing well-known interaction components such as buttons, sliders, and rotary knobs. These modules can be changed quickly, thus allowing users to adapt them to their current use case. After an iterative development of our system, which involved a focus group with HCI researchers, we conducted a user study to compare the ARm Haptics system to hand-tracking-based interaction in mid-air (baseline). Our findings show that using our system results in significantly lower error rates for slider and rotary input. Moreover, use of the ARm Haptics system results in significantly higher pragmatic quality and lower effort, frustration, and physical demand. Following our findings, we discuss opportunities for haptics worn on the forearm.

Links: [Paper (probably not available yet)](https://doi.org/10.1145/3546728)

## Overview
* **3D Printing** contains all files required to 3D print the mount and the three different input modules
* **Architecture** provides an overview of the system architecture
* **Arduino Firmware** is the firmware used for the NodeMCU (for the different input modules)
* **Unity** contains the script used to listen to the MQTT broker

## System Architecture
![The system architecture of the ARm Haptics system.](/Architecture/Architecture.png?raw=true "The system architecture of the ARm Haptics system.")

**Figure 2: The system architecture of the ARm Haptics system**