import Jetson.GPIO as GPIO
import time

# --- 變數設定 ---
# 使用 BCM 編號。
# 注意：如果你原本接的是 Board Pin 7，在 BCM 模式下對應的可能是 GPIO 4。
# 請確認你的 LED 接腳。這裡假設 output_pin 設為 BCM 號碼。
SPICLK = 11
SPIMISO = 9
SPIMOSI = 10
SPICS = 8
output_pin1 = 4  # 假設這是 BCM 7，請依實際接線調整
output_pin2 = 17
photo_ch = 0

# --- 設定門檻值 (新增功能) ---
# 這是用來決定開關燈的數值，你可以根據現場光線調整
# MCP3008 範圍是 0-1023
THRESHOLD = 500 

# --- 讀取 ADC 函數 (保持不變) ---
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

# --- 初始化函數 (已修正) ---
def init():
    # 修正重點：統一使用 BCM，並移除中間多餘的 cleanup
    GPIO.setwarnings(False)
    GPIO.setmode(GPIO.BCM) 

    # 設定 SPI 接腳
    GPIO.setup(SPIMOSI, GPIO.OUT)
    GPIO.setup(SPIMISO, GPIO.IN)
    GPIO.setup(SPICLK, GPIO.OUT)
    GPIO.setup(SPICS, GPIO.OUT)
    
    # 設定 LED 接腳
    GPIO.setup(output_pin1, GPIO.OUT, initial=GPIO.LOW)
    GPIO.setup(output_pin2, GPIO.OUT, initial=GPIO.LOW)

# --- 主程式 ---
def main():
    init()
    print("程式開始執行，按 Ctrl+C 停止...")
    print(f"目前設定門檻值為: {THRESHOLD}")

    while True:
        # 1. 讀取數值
        adc_value = readadc(photo_ch, SPICLK, SPIMOSI, SPIMISO, SPICS)
        
        # 2. 判斷邏輯 (圖片要求：依據讀取門檻值控制 LED)
        # 假設數值越小代表越暗 (視電路接法而定，若相反請把 < 改成 >)
        # 這裡示範：數值小於 500 (變暗) -> 開燈
        if adc_value < THRESHOLD: 
            GPIO.output(output_pin1, GPIO.HIGH) # LED 亮
            GPIO.output(output_pin2, GPIO.HIGH) # LED 亮
            status = "LED ON (亮)"
        else:
            GPIO.output(output_pin1, GPIO.LOW)  # LED 滅
            GPIO.output(output_pin2, GPIO.LOW)  # LED 滅
            status = "LED OFF (滅)"

        # 3. 印出狀態 (圖片要求：在 terminal 上印出)
        print(f"ADC Value: {adc_value} | Status: {status}")
        
        time.sleep(1)

if __name__ == '__main__':
    try:
        main()
    except KeyboardInterrupt:
        print("\n程式停止，清理 GPIO 設定...")
        GPIO.cleanup()