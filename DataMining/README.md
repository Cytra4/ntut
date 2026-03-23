# HW2 Programming
## Install
Python: https://www.python.org/downloads/
<br>
All the imported packages used in code are already included when you install Python.

## How to Run
Simply runs the main.py to start, enters the .csv file you want to data mine (ex: test1.csv), then enters the min_support value (ex: 0.6), lastly enter the min_confidence value (ex: 0.8). 
<br><br>
Example Input:
<br>
輸入資料csv檔名稱(ex: test1.csv): test1.csv
<br>
輸入min_support(ex: 0.6): 0.6
<br>
輸入min_confidence(ex: 0.8): 0.8

<br>
The output will show both algorithms' results:
<br>
Apriori will show the Largest Frequent Itemsets and it's support value, then shows all the Association rules with their Confidence value.
<br>
FP-Growth will show the F-list, all the Transactions in order, and the Frequent Patterns with their support value.
<br>
Lastly the output will show both algorithms' perform time to compare. 