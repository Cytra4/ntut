#!/usr/bin/env python3
# controll.py - for running under Node.js child_process
# 使用 Jetson.GPIO 與 MCP3008 SPI 讀取 LDR，並以讀到的值控制兩顆 LED 閃爍頻率
# 每次會把 "ADC=<adc_value>,FREQ=<freq>" 輸出到 stdout（立即 flush）

import Jetson.GPIO as GPIO
import time
import sys
import signal

# --- SPI / GPIO config (BCM 編號) ---
SPICLK = 11
SPIMISO = 9
SPIMOSI = 10
SPICS = 8
output_pin1 = 4
output_pin2 = 17
photo_ch = 0

# --- 頻率映射參數 (可調) ---
MIN_FREQ = 0.5   # Hz (最慢)
MAX_FREQ = 6.0   # Hz (最快)

# 讀取間隔（讀 ADC 的頻率，單位秒）
READ_INTERVAL = 0.05  # 50ms 讀一次 ADC，實際閃爍仍以 freq 控制

# GPIO 初始化旗標
_initialized = False
_running = True

def readadc(adcnum, clockpin, mosipin, misopin, cspin):
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
    # read in one bit, one null bit and 10 ADC bits.
    for i in range(12):
        GPIO.output(clockpin, True)
        GPIO.output(clockpin, False)
        adcout <<= 1
        if (GPIO.input(misopin)):
            adcout |= 0x1

    GPIO.output(cspin, True)
    adcout >>= 1
    return adcout

def adc_to_freq(adc_value):
    # clamp adc_value to [0,1023]
    v = max(0, min(1023, int(adc_value)))
    # invert mapping so adc small -> freq large
    freq = MAX_FREQ - (v / 1023.0) * (MAX_FREQ - MIN_FREQ)
    return float(freq)

def gpio_init():
    global _initialized
    if _initialized:
        return
    GPIO.setwarnings(False)
    GPIO.setmode(GPIO.BCM)
    # SPI pins
    GPIO.setup(SPIMOSI, GPIO.OUT)
    GPIO.setup(SPIMISO, GPIO.IN)
    GPIO.setup(SPICLK, GPIO.OUT)
    GPIO.setup(SPICS, GPIO.OUT)
    # LED pins
    GPIO.setup(output_pin1, GPIO.OUT, initial=GPIO.LOW)
    GPIO.setup(output_pin2, GPIO.OUT, initial=GPIO.LOW)
    _initialized = True

def cleanup_and_exit(signum=None, frame=None):
    global _running
    _running = False
    try:
        GPIO.output(output_pin1, GPIO.LOW)
        GPIO.output(output_pin2, GPIO.LOW)
        GPIO.cleanup()
    except Exception:
        pass
    # Print a final message so Node.js can see exit (optional)
    print("EXIT")
    try:
        sys.stdout.flush()
    except Exception:
        pass
    # exit after a short delay to let stdout flush
    time.sleep(0.05)
    sys.exit(0)

# catch signals so Node.js can kill gracefully
signal.signal(signal.SIGINT, cleanup_and_exit)
signal.signal(signal.SIGTERM, cleanup_and_exit)

def main():
    gpio_init()
    print("START")
    sys.stdout.flush()

    led_state = False
    last_toggle = time.time()
    current_freq = MIN_FREQ
    half_period = 1.0 / (2.0 * current_freq) if current_freq > 0 else 0.5
    last_report = 0.0
    REPORT_INTERVAL = 0.1  # 每 100ms 輸出 ADC/FREQ 給 Node.js

    try:
        while _running:
            # 1) 讀 ADC
            adc_value = readadc(photo_ch, SPICLK, SPIMOSI, SPIMISO, SPICS)
            if adc_value < 0:
                adc_value = 0

            # 2) 計算頻率
            freq = adc_to_freq(adc_value)
            if freq <= 0:
                half_period = 0.5
            else:
                half_period = 1.0 / (2.0 * freq)

            # 3) 控制 LED 切換（非阻塞）
            now = time.time()
            if now - last_toggle >= half_period:
                led_state = not led_state
                GPIO.output(output_pin1, GPIO.HIGH if led_state else GPIO.LOW)
                GPIO.output(output_pin2, GPIO.HIGH if led_state else GPIO.LOW)
                last_toggle = now

            # 4) 定期把 ADC 與 FREQ 印到 stdout（讓 Node.js 立刻收到）
            if now - last_report >= REPORT_INTERVAL:
                # 格式化輸出，方便 Node.js 解析
                out = f"ADC={int(adc_value)},FREQ={freq:.3f}"
                print(out)
                try:
                    sys.stdout.flush()
                except Exception:
                    pass
                last_report = now

            # 5) 小睡避免 100% CPU
            time.sleep(READ_INTERVAL)
    except SystemExit:
        pass
    except Exception as e:
        # 若發生例外，印錯誤給 stderr，並嘗試清理
        print(f"ERROR: {e}", file=sys.stderr)
        try:
            sys.stderr.flush()
        except Exception:
            pass
    finally:
        cleanup_and_exit()

if __name__ == "__main__":
    main()
