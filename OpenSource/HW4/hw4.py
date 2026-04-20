from simple_cron import SimpleCRON
from machine import RTC

rtc = RTC()
cron = SimpleCRON(rtc)

def send_letter():
    print("Send a letter to kiki")

cron.add('0 0 12 4 * 2026', send_letter)