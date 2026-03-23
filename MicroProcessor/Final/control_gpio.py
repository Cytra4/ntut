#!/usr/bin/env python3
import sys
import time
import random

# Pin mapping: change to match your Jetson TX2 wiring
PIN_MAP = {
    'door1': 11,
    'door2': 13,
    'win1' : 15,
    'win2' : 16,
}

USE_GPIO = False
GPIO = None
HAVE_SPIDEV = False
spi = None
try:
    import spidev
    HAVE_SPIDEV = True
    spi = spidev.SpiDev()
    spi.open(0, 0)
    spi.max_speed_hz = 1350000
except Exception:
    HAVE_SPIDEV = False

try:
    import Jetson.GPIO as GPIO
    USE_GPIO = True
    # do not setmode globally here; we'll set appropriate mode in init if needed
except Exception:
    USE_GPIO = False

def set_pin(pin, state):
    if USE_GPIO:
        try:
            GPIO.setup(pin, GPIO.OUT, initial=GPIO.LOW)
            GPIO.output(pin, GPIO.HIGH if state else GPIO.LOW)
        except Exception as e:
            print('[GPIO ERR]', e)
    else:
        print('[SIM] set_pin', pin, state)

def blink_pin(pin, interval_ms, duration_s=10):
    if USE_GPIO:
        try:
            GPIO.setup(pin, GPIO.OUT, initial=GPIO.LOW)
            end = time.time() + duration_s
            state = False
            while time.time() < end:
                state = not state
                GPIO.output(pin, GPIO.HIGH if state else GPIO.LOW)
                time.sleep(interval_ms/1000.0)
        except Exception as e:
            print('[GPIO ERR]', e)
    else:
        # simulate blinking
        end = time.time() + duration_s
        state = False
        while time.time() < end:
            state = not state
            print(f'[SIM BLINK] pin={pin} state={state}')
            time.sleep(interval_ms/1000.0)

# --- MCP3008 SPI read helpers (hardware SPI via spidev, or bit-banged via Jetson.GPIO) ---
SPICLK = 11
SPIMISO = 9
SPIMOSI = 10
SPICS = 8
PHOTO_CH = 0
_ldr_inited = False

def _init_ldr_bitbang():
    global _ldr_inited
    if not USE_GPIO:
        return
    try:
        GPIO.setwarnings(False)
        # user's code used BCM numbering for SPI pins
        GPIO.setmode(GPIO.BCM)
        GPIO.setup(SPIMOSI, GPIO.OUT)
        GPIO.setup(SPIMISO, GPIO.IN)
        GPIO.setup(SPICLK, GPIO.OUT)
        GPIO.setup(SPICS, GPIO.OUT)
        _ldr_inited = True
    except Exception as e:
        print('[ADC INIT ERR]', e)
        _ldr_inited = False

def _readadc_bitbang(adcnum, clockpin, mosipin, misopin, cspin):
    if ((adcnum > 7) or (adcnum < 0)):
        return -1
    GPIO.output(cspin, True)
    GPIO.output(clockpin, False) # start clock low
    GPIO.output(cspin, False)    # bring CS low

    commandout = adcnum
    commandout |= 0x18 # start bit + single-ended bit
    commandout <<= 3   # we only need to send 5 bits here
    for i in range(5):
        if (commandout & 0x80):
            GPIO.output(mosipin, True)
        else:
            GPIO.output(mosipin, False)
        commandout <<= 1
        GPIO.output(clockpin, True)
        GPIO.output(clockpin, False)

    adcout = 0
    for i in range(12):
        GPIO.output(clockpin, True)
        GPIO.output(clockpin, False)
        adcout <<= 1
        if (GPIO.input(misopin)):
            adcout |= 0x1
    GPIO.output(cspin, True)
    adcout >>= 1
    return adcout

def read_ldr_raw():
    # Prefer hardware SPI via spidev if available
    try:
        if HAVE_SPIDEV and spi is not None:
            # MCP3008 read: send [1, (8+ch)<<4, 0]
            resp = spi.xfer2([1, (8 + PHOTO_CH) << 4, 0])
            val = ((resp[1] & 3) << 8) | resp[2]
            return val
        # fallback to bit-banged SPI using Jetson.GPIO
        if USE_GPIO:
            if not _ldr_inited:
                _init_ldr_bitbang()
            if _ldr_inited:
                return _readadc_bitbang(PHOTO_CH, SPICLK, SPIMOSI, SPIMISO, SPICS)
        # final fallback: simulated random
        return random.randint(0, 1023)
    except Exception as e:
        print('[ADC ERR]', e)
        return random.randint(0, 1023)

def main():
    if len(sys.argv) < 2:
        print('no action')
        return
    action = sys.argv[1]
    if action == 'set_detection':
        dev = sys.argv[2] if len(sys.argv) > 2 else 'unknown'
        state = sys.argv[3] if len(sys.argv) > 3 else 'off'
        pin = PIN_MAP.get(dev)
        if pin:
            set_pin(pin, state == 'on')
            print(f'[GPIO] set_detection {dev} -> {state}')
        else:
            print('[GPIO] unknown device', dev)
    elif action == 'trigger_alarm':
        dev = sys.argv[2] if len(sys.argv) > 2 else 'unknown'
        interval = int(sys.argv[3]) if len(sys.argv) > 3 else 500
        pin = PIN_MAP.get(dev)
        if pin:
            print(f'[GPIO] trigger_alarm {dev} interval={interval}ms')
            blink_pin(pin, interval, duration_s=10)
            print(f'[GPIO] trigger_alarm_done {dev}')
        else:
            print('[GPIO] unknown device', dev)
    elif action == 'stop_alarm':
        dev = sys.argv[2] if len(sys.argv) > 2 else 'unknown'
        pin = PIN_MAP.get(dev)
        if pin:
            set_pin(pin, True)
            print(f'[GPIO] stop_alarm {dev}')
        else:
            print('[GPIO] unknown device', dev)
    elif action == 'read_ldr':
        val = read_ldr_raw()
        print(val)
    else:
        print('unknown action', action)

if __name__ == '__main__':
    main()
