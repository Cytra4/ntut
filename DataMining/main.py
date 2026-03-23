import apriori as AP
import FP
import time

data_name = str(input("輸入資料csv檔名稱(ex: test1.csv): "))
min_support = float(input("輸入min_support(ex: 0.6): "))
min_confidence = float(input("輸入min_confidence(ex: 0.8): "))

print("==========APRIORI==========")
start_time = time.time()
AP.Apriori_Main(data_name, min_support, min_confidence)
end_time = time.time()
apriori_time = end_time - start_time
print("===========================")

print("")

print("=========FP-GROWTH=========")
start_time = time.time()
FP.FPGrowth(data_name, min_support)
end_time = time.time()
fpgrowth_time = end_time - start_time
print("===========================")

print(" ")

print(f"Apriori所花的時間: {apriori_time}")
print(f"FP-Growth所花的時間: {fpgrowth_time}")