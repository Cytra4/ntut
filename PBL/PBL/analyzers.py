# analyzers.py
import pandas as pd
import numpy as np

# === 設定：最小樣本數門檻 ===
# 如果某一群組的人數少於這個數字，代表樣本偏差太大，不予計算
MIN_SAMPLE_SIZE = 10

def analyze_homework_quantity_vs_score(df, config):
    """
    分析作業頻率(數值)與成績的關係
    """
    freq_col = config['freq']
    score_col = config['score']
    
    # 去除空值
    clean_df = df[[freq_col, score_col]].dropna()
    
    if len(clean_df) < MIN_SAMPLE_SIZE:
        return None

    # 計算相關係數
    corr = clean_df[freq_col].corr(clean_df[score_col])
    
    # 為了畫圖，我們將頻率分組取平均成績
    grouped = clean_df.groupby(freq_col)[score_col].mean()
    
    return {
        'correlation': corr,
        'grouped_stats': grouped
    }

def analyze_homework_type_impact(df, config):
    """
    分析各類作業(有做vs沒做)對成績的影響差異
    """
    results = {}
    score_col = config['score']
    
    for type_col in config['types']:
        if type_col not in df.columns:
            continue
            
        # 1. 分成「有做(>0)」與「沒做(<=0 或 NaN)」兩組
        # 注意：這裡假設標準化後，原本為0的數值可能變成某個負數或0，視Scaler而定
        # 但通常 raw data 轉過來，我們可以用「是否大於最小值」或「是否大於平均」來切
        # 簡單起見，我們假設資料中較大的值代表「有做」，較小的值代表「沒做」
        # 更好的方式是看原始資料，但因為只有標準化資料，我們用「中位數」或簡單二分法
        
        subset = df[[type_col, score_col]].dropna()
        if len(subset) < MIN_SAMPLE_SIZE:
            continue

        # 這裡我們嘗試用一種通用邏輯：
        # 該欄位數值 > 該欄位平均值 => 視為「有做/頻率高」
        # 該欄位數值 < 該欄位平均值 => 視為「沒做/頻率低」
        threshold = subset[type_col].mean()
        
        group_high = subset[subset[type_col] > threshold][score_col]
        group_low = subset[subset[type_col] <= threshold][score_col]
        
        # === 關鍵修正：檢查樣本數 ===
        # 如果某一組人數太少（例如大家都做習題，只有3個人沒做），則不計算，避免偏差
        if len(group_high) < MIN_SAMPLE_SIZE or len(group_low) < MIN_SAMPLE_SIZE:
            # 樣本不足，跳過此類型
            continue
            
        avg_high = group_high.mean()
        avg_low = group_low.mean()
        
        diff = avg_high - avg_low
        
        results[type_col] = {
            'diff': diff,
            'avg_high': avg_high,
            'avg_low': avg_low,
            'count_high': len(group_high),
            'count_low': len(group_low)
        }
        
    return results

def analyze_performance_correlation(df, config):
    """
    分析作業頻率與課堂表現(跟上進度/用功程度)的相關性
    """
    results = {}
    freq_col = config['freq']
    
    for p_col in config['performance']:
        if p_col not in df.columns:
            continue
            
        clean_df = df[[freq_col, p_col]].dropna()
        if len(clean_df) < MIN_SAMPLE_SIZE:
            continue
            
        corr = clean_df[freq_col].corr(clean_df[p_col])
        results[p_col] = corr
        
    return results
# analyzers.py (請將此函式加到檔案最下方)

def analyze_optimal_frequency(df, config):
    """
    分析作業頻率的「甜蜜點」與「極端值」
    將作業頻率分為：低 (Low)、中 (Mid)、高 (High) 三組
    回傳：各組的 平均成績(Mean) 與 人數(Count)
    """
    freq_col = config['freq']
    score_col = config['score']
    
    # 建立乾淨的資料子集
    data = df[[freq_col, score_col]].dropna().copy()
    
    # 如果總人數太少，無法分析
    if len(data) < 30: 
        return None

    # 使用 qcut 將資料依據「人數」切成三等份
    # 這樣可以確保每一組的人數差不多，比較公平
    try:
        data['group'] = pd.qcut(data[freq_col], q=3, labels=['Low (少)', 'Mid (中)', 'High (多)'])
    except ValueError:
        # 如果數據太集中無法切分 (例如大家頻率都一樣)，改用 cut (依數值切分)
        data['group'] = pd.cut(data[freq_col], bins=3, labels=['Low (少)', 'Mid (中)', 'High (多)'])

    # 計算各組統計量
    # agg 函式可以同時計算 平均值(mean) 和 人數(count)
    result = data.groupby('group')[score_col].agg(['mean', 'count'])
    
    return result