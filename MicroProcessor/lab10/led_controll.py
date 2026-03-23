#!/usr/bin/env python3
# led_control.py
# usage: python3 led_control.py <pin> <ON|OFF>
# 控制單一 BCM 腳位 ON/OFF (使用 Jetson.GPIO)

import sys
import time
import Jetson.GPIO as GPIO

def usage():
    print("Usage: led_control.py <pin> <ON|OFF>", file=sys.stderr)

def main():
    if len(sys.argv) < 3:
        usage()
        return 2
    try:
        pin = int(sys.argv[1])
        action = sys.argv[2].upper()
        if action not in ("ON", "OFF"):
            usage()
            return 2
    except Exception as e:
        print("Invalid args", e, file=sys.stderr)
        return 2

    try:
        GPIO.setwarnings(False)
        GPIO.setmode(GPIO.BCM)
        GPIO.setup(pin, GPIO.OUT, initial=GPIO.LOW)
        if action == "ON":
            GPIO.output(pin, GPIO.HIGH)
        else:
            GPIO.output(pin, GPIO.LOW)
        # optional: short delay to ensure hardware set
        time.sleep(0.02)
        print(f"OK {pin} {action}")
        sys.stdout.flush()
        # do NOT cleanup entire GPIO if you still need other pins (but safe to cleanup)
        GPIO.cleanup()
        return 0
    except Exception as e:
        print("GPIO error: " + str(e), file=sys.stderr)
        try:
            GPIO.cleanup()
        except:
            pass
        return 3

if __name__ == "__main__":
    sys.exit(main())
