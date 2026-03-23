# data_loader.py
import pandas as pd
import numpy as np
from config import DATA_PATH

def load_and_clean_data(filepath=DATA_PATH):
    """
    讀取並清理資料
    1. 讀取 CSV
    2. 轉換數值型別
    3. 處理缺失值 (根據提案建議去除異常值或空值)
    """
    try:
        df = pd.read_csv(filepath)
        print(f"成功讀取資料，原始筆數: {len(df)}")
        
        
        
        # 刪除成績欄位為空的列
        essential_cols = ['w2math_unified', 'w2nright_unified', 'mat_hw_freq_num', 'chi_hw_freq_num']
        
        df_clean = df.dropna(subset=[c for c in essential_cols if c in df.columns])
        
        print(f"資料清理完成，剩餘筆數: {len(df_clean)}")
        return df_clean
        
    except FileNotFoundError:
        print(f"錯誤：找不到檔案 {filepath}")
        return None
    except Exception as e:
        print(f"發生未預期錯誤: {e}")
        return None

if __name__ == "__main__":

    load_and_clean_data()