/*
 * Title       LxWebcam
 * by          Lung-Kai Cheng (lkcheng89)
 *
 * Copyright (C) 2020 Lung-Kai Cheng
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * Description:
 *   ASCOM Driver for Webcam with LX Modification (Firmware Part)
 *
 * Author: Lung-Kai Cheng
 *   https://github.com/lkcheng89
 *   lkcheng89@gmail.com
 *
 * Revision history, and newer versions:
 *   See GitHub: https://github.com/lkcheng89/lxwebcam
 *
 * Documentation:
 *   https://github.com/lkcheng89/lxwebcam
 *
 * Discussion, Questions, ...etc
 *   https://github.com/lkcheng89/lxwebcam/issues
 */

#define LX                       2
#define LX_INIT                  LOW
#define LX_ON                    HIGH
#define LX_OFF                   LOW

#define SHUTTER                  3
#define SHUTTER_INIT             LOW
#define SHUTTER_OPEN             HIGH
#define SHUTTER_CLOSE            LOW

#define AMP                      4
#define AMP_INIT                 LOW
#define AMP_ON                   LOW
#define AMP_OFF                  HIGH

#define EXPOSURE_DURATION_MIN    33
#define EXPOSURE_DURATION_MAX    60000

#define PULSEGUIDE_RA_P          5
#define PULSEGUIDE_DEC_P         6
#define PULSEGUIDE_DEC_N         7
#define PULSEGUIDE_RA_N          8
#define PULSEGUIDE_INIT          LOW
#define PULSEGUIDE_ON            HIGH
#define PULSEGUIDE_OFF           LOW
#define PULSEGUIDE_DURATION_MIN  1
#define PULSEGUIDE_DURATION_MAX  10000

#define FIRMWARE_DATE            __DATE__
#define FIRMWARE_TIME            __TIME__
#define FIRMWARE_NAME            "LxWebcam"
#define FIRMWARE_VERSION_MAJOR   0
#define FIRMWARE_VERSION_MINOR   1

#define SERIAL_BAUDRATE          9600
#define SERIAL_ECHOMODE          0

class Command
{
public:
  bool parse(char c)
  {
    bool parsed = false;
    
    switch (state)
    {
      case 0: // INIT
      {
        pcmnd = pbuff; *pbuff++ = c; state = 1;
        break;
      }
      case 1: // CMND
      {
        if (c == ' ')
        {
          *pbuff++ = '\0'; pparm = pbuff; state = 2;
        }
        else if (c == '\n' || c == '\r')
        {
          *pbuff++ = '\0'; parsed = true;
        }
        else
        {
          *pbuff++ = c;
        }
        break;
      }
      case 2: // PARM
      {
        if (c == ' ')
        {
          *pbuff++ = '\0'; state = 3;
        }
        else if (c == '\n' || c == '\r')
        {
          *pbuff++ = '\0'; parsed = true;
        }
        else
        {
          *pbuff++ = c;
        }
        break;
      }
      case 3: // DONT CARE
      default:
      {
        if (c == '\n' || c == '\r')
        {
          parsed = true;
        }
        break;
      }
    }

    return parsed;
  }

  String getCommand()
  {
    return String(pcmnd);
  }

  bool hasParameter()
  {
    return (pparm != NULL);
  }

  String getParameter()
  {
    return String(pparm);
  }

  void reset()
  {
    pbuff = buff;
    pcmnd = NULL;
    pparm = NULL;
    state = 0;
  }
  
private:
  char buff[64];
  char *pbuff = buff;
  char *pcmnd = NULL;
  char *pparm = NULL;
  int state = 0;
};

int lx = LX_INIT;
int lxTarget = LX_INIT;
int shutter = SHUTTER_INIT;
int shutterTarget = SHUTTER_INIT;
int amp = AMP_INIT;
int ampTarget = AMP_INIT;

int pulseGuide1 = -1;
int pulseGuide1Target = -1;
int pulseGuide2 = PULSEGUIDE_INIT;
int pulseGuide2Target = PULSEGUIDE_INIT;

unsigned long exposureLoopBegin = 0;
unsigned long exposureLoopEnd = 0;
int pulseGuideLoop = -1;
unsigned long pulseGuideLoopBegin = 0;
unsigned long pulseGuideLoopEnd = 0;

Command command;

void setup()
{
  Serial.begin(SERIAL_BAUDRATE);

  pinMode(LX, OUTPUT);
  pinMode(SHUTTER, OUTPUT);
  pinMode(AMP, OUTPUT);
  pinMode(PULSEGUIDE_RA_P, OUTPUT);
  pinMode(PULSEGUIDE_DEC_P, OUTPUT);
  pinMode(PULSEGUIDE_DEC_N, OUTPUT);
  pinMode(PULSEGUIDE_RA_N, OUTPUT);

  digitalWrite(LX, LX_INIT);
  digitalWrite(SHUTTER, SHUTTER_INIT);
  digitalWrite(AMP, AMP_INIT);
  digitalWrite(PULSEGUIDE_RA_P, PULSEGUIDE_INIT);
  digitalWrite(PULSEGUIDE_DEC_P, PULSEGUIDE_INIT);
  digitalWrite(PULSEGUIDE_DEC_N, PULSEGUIDE_INIT);
  digitalWrite(PULSEGUIDE_RA_N, PULSEGUIDE_INIT);
}

void loop()
{
  // Supported Command Set
  //
  // * LX
  // * LX [ON|OFF]; OFF is default value
  // * SHUTTER
  // * SHUTTER [OPEN|CLOSE]; CLOSE is default value
  // * AMP
  // * AMP [ON|OFF]; ON is default value
  // * START <duration>
  // * STOP
  // * RA+ <duration>
  // * DEC+ <duration>
  // * DEC- <duration>
  // * RA- <duration>
  // * DURATION [EXPOSURE|PULSEGUIDE]
  // * STATE
  // * VERSION
  //
  
  unsigned long time = millis();

  if (Serial.available() > 0)
  {
    char c = Serial.read();

#if SERIAL_ECHOMODE
    Serial.print(c);
#endif
    
    bool parsed = command.parse(toupper(c));
    
    if (parsed)
    {
      String cmnd = command.getCommand();

      if (cmnd == "LX")
      {
        if (command.hasParameter())
        {
          String parm = command.getParameter();

          if (parm == "ON")
          {
            Serial.println("OK"); lxTarget = LX_ON;
          }
          else if (parm == "OFF")
          {
            Serial.println("OK"); lxTarget = LX_OFF;
          }
          else
          {
            Serial.println("ERROR:PARAMETER");
          }
        }
        else if (lx == LX_ON)
        {
          Serial.println("ON");
        }
        else/* if (lx == LX_OFF)*/
        {
          Serial.println("OFF");
        }
      }
      else if (cmnd == "SHUTTER")
      {
        if (command.hasParameter())
        {
          String parm = command.getParameter();

          if (parm == "OPEN")
          {
            Serial.println("OK"); shutterTarget = SHUTTER_OPEN;
          }
          else if (parm == "CLOSE")
          {
            Serial.println("OK"); shutterTarget = SHUTTER_CLOSE;
          }
          else
          {
            Serial.println("ERROR:PARAMETER");
          }
        }
        else if (shutter == SHUTTER_OPEN)
        {
          Serial.println("OPEN");
        }
        else/* if (shutter == SHUTTER_CLOSE)*/
        {
          Serial.println("CLOSE");
        }
      }
      else if (cmnd == "AMP")
      {
        if (command.hasParameter())
        {
          String parm = command.getParameter();

          if (parm == "ON")
          {
            Serial.println("OK"); ampTarget = AMP_ON;
          }
          else if (parm == "OFF")
          {
            Serial.println("OK"); ampTarget = AMP_OFF;
          }
          else
          {
            Serial.println("ERROR:PARAMETER");
          }
        }
        else if (amp == AMP_ON)
        {
          Serial.println("ON");
        }
        else/* if (amp == AMP_OFF)*/
        {
          Serial.println("OFF");
        }
      }
      else if (cmnd == "START")
      {
        if (exposureLoopBegin < exposureLoopEnd)
        {
          Serial.println("ERROR:STATE");
        }
        else
        {
          int duration = command.getParameter().toInt();

          if (EXPOSURE_DURATION_MIN <= duration && duration <= EXPOSURE_DURATION_MAX)
          {
            exposureLoopBegin = time;
            exposureLoopEnd = time + duration;

            Serial.println("OK");
          }
          else
          {
            Serial.println("ERROR:PARAMETER");
          }
        }
      }
      else if (cmnd == "STOP")
      {
        if (exposureLoopBegin < exposureLoopEnd)
        {
          exposureLoopEnd = time;

          Serial.println("OK");
        }
        else
        {
          Serial.println("ERROR:STATE");
        }
      }
      else if (cmnd == "RA+")
      {
        if (pulseGuideLoop != -1 && pulseGuideLoopBegin < pulseGuideLoopEnd)
        {
          Serial.println("ERROR:STATE");
        }
        else
        {
          int duration = command.getParameter().toInt();

          if (PULSEGUIDE_DURATION_MIN <= duration && duration <= PULSEGUIDE_DURATION_MAX)
          {
            pulseGuideLoop = PULSEGUIDE_RA_P;
            pulseGuideLoopBegin = time;
            pulseGuideLoopEnd = time + duration;

            Serial.println("OK");
          }
          else
          {
            Serial.println("ERROR:PARAMETER");
          }
        }
      }
      else if (cmnd == "DEC+")
      {
        if (pulseGuideLoop != -1 && pulseGuideLoopBegin < pulseGuideLoopEnd)
        {
          Serial.println("ERROR:STATE");
        }
        else
        {
          int duration = command.getParameter().toInt();

          if (PULSEGUIDE_DURATION_MIN <= duration && duration <= PULSEGUIDE_DURATION_MAX)
          {
            pulseGuideLoop = PULSEGUIDE_DEC_P;
            pulseGuideLoopBegin = time;
            pulseGuideLoopEnd = time + duration;

            Serial.println("OK");
          }
          else
          {
            Serial.println("ERROR:PARAMETER");
          }
        }
      }
      else if (cmnd == "DEC-")
      {
        if (pulseGuideLoop != -1 && pulseGuideLoopBegin < pulseGuideLoopEnd)
        {
          Serial.println("ERROR:STATE");
        }
        else
        {
          int duration = command.getParameter().toInt();

          if (PULSEGUIDE_DURATION_MIN <= duration && duration <= PULSEGUIDE_DURATION_MAX)
          {
            pulseGuideLoop = PULSEGUIDE_DEC_N;
            pulseGuideLoopBegin = time;
            pulseGuideLoopEnd = time + duration;

            Serial.println("OK");
          }
          else
          {
            Serial.println("ERROR:PARAMETER");
          }
        }
      }
      else if (cmnd == "RA-")
      {
        if (pulseGuideLoop != -1 && pulseGuideLoopBegin < pulseGuideLoopEnd)
        {
          Serial.println("ERROR:STATE");
        }
        else
        {
          int duration = command.getParameter().toInt();

          if (PULSEGUIDE_DURATION_MIN <= duration && duration <= PULSEGUIDE_DURATION_MAX)
          {
            pulseGuideLoop = PULSEGUIDE_RA_N;
            pulseGuideLoopBegin = time;
            pulseGuideLoopEnd = time + duration;

            Serial.println("OK");
          }
          else
          {
            Serial.println("ERROR:PARAMETER");
          }
        }
      }
      else if (cmnd == "DURATION")
      {
        String action = command.getParameter();

        if (action == "EXPOSURE")
        {
          Serial.print(EXPOSURE_DURATION_MIN); Serial.print(" "); Serial.println(EXPOSURE_DURATION_MAX);
        }
        else if (action == "PULSEGUIDE")
        {
          Serial.print(PULSEGUIDE_DURATION_MIN); Serial.print(" "); Serial.println(PULSEGUIDE_DURATION_MAX);
        }
        else
        {
          Serial.println("ERROR:PARAMETER");
        }
      }
      else if (cmnd == "STATE")
      {
        if (exposureLoopBegin < exposureLoopEnd)
        {
          if (pulseGuideLoop != -1 && pulseGuideLoopBegin < pulseGuideLoopEnd)
          {
            Serial.println("EXPOSURE PULSEGUIDE");
          }
          else
          {
            Serial.println("EXPOSURE");
          }
        }
        else if (pulseGuideLoop != -1 && pulseGuideLoopBegin < pulseGuideLoopEnd)
        {
          Serial.println("PULSEGUIDE");
        }
        else
        {
          Serial.println("IDLE");
        }
      }
      else if (cmnd == "VERSION")
      {
        Serial.print(FIRMWARE_VERSION_MAJOR); Serial.print("."); Serial.println(FIRMWARE_VERSION_MINOR);
      }
      else
      {
        Serial.println("ERROR:COMMAND");
      }
      
      command.reset();
    }
  }

  // Perform Exposure Control Loop

  if (exposureLoopBegin < exposureLoopEnd)
  {
    if (time == exposureLoopBegin)
    {
      shutterTarget = SHUTTER_OPEN;
    }
    else if (time >= exposureLoopEnd)
    {
      shutterTarget = SHUTTER_CLOSE;

      exposureLoopBegin = 0;
      exposureLoopEnd = 0;
    }
  }

  // Perform Pulse Guide Control Loop

  if (pulseGuideLoop != -1 && pulseGuideLoopBegin < pulseGuideLoopEnd)
  {
    if (time == pulseGuideLoopBegin)
    {
      pulseGuide1Target = pulseGuideLoop;
      pulseGuide2Target = PULSEGUIDE_ON;
    }
    else if (time >= pulseGuideLoopEnd)
    {
      pulseGuide1Target = pulseGuideLoop;
      pulseGuide2Target = PULSEGUIDE_OFF;

      pulseGuideLoop = -1;
      pulseGuideLoopBegin = 0;
      pulseGuideLoopEnd = 0;
    }
  }

  // Perform Control Pins Setting

  if (lx != lxTarget)
  {
    digitalWrite(LX, lxTarget); lx = lxTarget;
  }

  if (shutter != shutterTarget)
  {
    digitalWrite(SHUTTER, shutterTarget); shutter = shutterTarget;
  }

  if (amp != ampTarget)
  {
    digitalWrite(AMP, ampTarget); amp = ampTarget;
  }

  if (pulseGuide1 != pulseGuide1Target)
  {
    digitalWrite(PULSEGUIDE_RA_P, PULSEGUIDE_OFF);
    digitalWrite(PULSEGUIDE_DEC_P, PULSEGUIDE_OFF);
    digitalWrite(PULSEGUIDE_DEC_N, PULSEGUIDE_OFF);
    digitalWrite(PULSEGUIDE_RA_N, PULSEGUIDE_OFF);
    
    digitalWrite(pulseGuide1Target, pulseGuide2Target); pulseGuide1 = pulseGuide1Target; pulseGuide2 = pulseGuide2Target;
  }
  else if (pulseGuide2 != pulseGuide2Target)
  {
    digitalWrite(pulseGuide1Target, pulseGuide2Target); pulseGuide1 = pulseGuide1Target; pulseGuide2 = pulseGuide2Target;
  }
}
