#!/usr/bin/env python3
# worker_task.py
import threading
import time
import sys

# Lab 9 要求：拆分運算範圍 (Data Parallelism)
def count_sum(start, end, result_list, index):
    total = 0
    for i in range(start, end + 1):
        total += i
    result_list[index] = total

def main():
    start_time = time.time()
    results = [0, 0]
    
    # 建立兩個執行緒 T1, T2 (比照 Lab 9)
    t1 = threading.Thread(target=count_sum, args=(1, 25000000, results, 0))
    t2 = threading.Thread(target=count_sum, args=(25000001, 50000000, results, 1))
    
    t1.start()
    t2.start()
    t1.join()
    t2.join()
    
    final_sum = sum(results)
    end_time = time.time()
    
    # 輸出結果給 Node.js
    print(f"RESULT={final_sum}")
    print(f"TIME={end_time - start_time:.4f}")
    sys.stdout.flush()

if __name__ == "__main__":
    main()
