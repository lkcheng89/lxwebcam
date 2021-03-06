ASCOM Driver for Webcam with LX Modification
============================================

This repository provides the ASCOM driver for Webcam with LX Modification. The following features are included:

* ASCOM driver controls LX-Modded webcam via builtin USB-To-Serial Port of Arduino Nano.
* Philips SPC900NC webcam is used to build and pass the conformance test.
* Pulse guide (via ST4 interface) is also supported for guided astrophotoraphy.

The firmware (saved as *.ino) is also included in this project and is easily found in the root directory.

* For the Schematics and PCB, please refer to https://easyeda.com/lkcheng89/lxwebcam
* For the Mechanical Enclosure, please refer to https://www.thingiverse.com/thing:4601845

Prerequisites
--------------

In order to use this driver you need to install:

* .NET Framework 4 or higher version (https://dotnet.microsoft.com/download)

    Please ensure this is installed on system or ASCOM platform may not work correctly.

* ASCOM Platform 6.2 or higher version (https://ascom-standards.org/)

  **↓ Users do not have to install items below. These are required only for developers ↓**

* ASCOM Platform Developer Components 6.2 or higher version (https://ascom-standards.org/Developer/DriverImpl.htm)

    Please install this to build and register driver.

* Microsoft Visual Studio 2017 or higher version (https://visualstudio.microsoft.com/)

    Please install this to build project in C# 7.0.

* Inno Setup (https://jrsoftware.org/isinfo.php)

    Please install this to create setup program that registers driver for both 32- and 64-bits platforms.

Installing
----------

Execute **LxWebcamDriver-v<MajorVersion>.<MinorVersion>.exe**, follow the instruction of setup program and everything will be done automatically. The name of camera driver will be called **LxWebcam** in chooser dialog of ASCOM platform.

Building
--------

Compile source code
^^^^^^^^^^^^^^^^^^^
Launch Microsoft Visual Studio and open **LxWebcam.sln** located at the root directory, build solution from the menu "Build > Build Solution" or accelerator key "F6".
::
    Suggest "Run as administrator" because "Register for COM interop" requires administrator's privileges in Debug mode.

Register ASCOM driver
^^^^^^^^^^^^^^^^^^^^^
$(FrameworkDir)\\regasm.exe /codebase bin\\Release\\ASCOM.LxWebcam.Camera.dll
::
    * $(FrameworkDir) is located at C:\Windows\Microsoft.NET\Framework\v4.0.30319 for 32-bits platform;
    * $(FrameworkDir) is located at C:\Windows\Microsoft.NET\Framework64\v4.0.30319 for 64-bits platform.

*This step is ommitted when build in Debug mode, this is required only when build in Release mode.*

Unregister ASCOM driver
^^^^^^^^^^^^^^^^^^^^^^^
$(FrameworkDir)\\regasm.exe -u bin\\Release\\ASCOM.LxWebcam.Camera.dll

Create setup program
^^^^^^^^^^^^^^^^^^^^
Launch Inno Setup and open **LxWebcamSetup.iss** located at the root directory, click **Run** button or accelerator key "F9" to create the setup program named **LxWebcamDriver-v<MajorVersion>.<MinorVersion>.exe**.

Usage
-----

Report me an issue if you find any problem during your usage. Stay up a night with clear sky.
