# visualizer.py
import matplotlib.pyplot as plt
import seaborn as sns
import pandas as pd
import numpy as np
import os
import config

# 確保輸出資料夾存在
if not os.path.exists(config.OUTPUT_DIR):
    os.makedirs(config.OUTPUT_DIR)
    print(f"已建立輸出資料夾: {config.OUTPUT_DIR}")

def set_chinese_font():
    """設定中文字型"""
    import platform
    system_name = platform.system()
    if system_name == "Windows":
        plt.rcParams['font.sans-serif'] = ['Microsoft JhengHei']
    elif system_name == "Darwin":
        plt.rcParams['font.sans-serif'] = ['Arial Unicode MS']
    else:
        plt.rcParams['font.sans-serif'] = ['SimHei']
    plt.rcParams['axes.unicode_minus'] = False

def save_current_plot(filename):
    """將目前的圖表存檔"""
    file_path = os.path.join(config.OUTPUT_DIR, filename)
    plt.savefig(file_path, dpi=300, bbox_inches='tight')
    plt.close() # 關閉圖表釋放記憶體
    print(f"    -> 圖表已儲存: {filename}")

def plot_homework_quantity_vs_score(results, subject_name):
    """繪製作業數量與成績的折線圖"""
    set_chinese_font()
    
    if not results or 'grouped_stats' not in results:
        return

    data = results['grouped_stats']
    correlation = results.get('correlation', 0)

    plt.figure(figsize=(10, 6))
    plt.plot(data.index, data.values, marker='o', linestyle='-', linewidth=2, color='#2c3e50')
    plt.title(f'{subject_name}：作業頻率與平均成績關係圖 (相關係數: {correlation:.2f})', fontsize=14)
    plt.xlabel('作業頻率 (標準化分數，越高代表越常有作業)', fontsize=12)
    plt.ylabel('平均成績', fontsize=12)
    plt.grid(True, linestyle='--', alpha=0.7)
    plt.tight_layout()
    
    # 存檔
    save_current_plot(f"{subject_name}_1_作業頻率vs成績.png")

def plot_homework_type_impact(results, subject_name):
    """繪製不同作業類型對成績影響的長條圖"""
    set_chinese_font()
    
    if not results:
        return

    types = list(results.keys())
    labels = [config.TYPE_DISPLAY_NAMES.get(t, t) for t in types]
    diffs = [results[t]['diff'] for t in types]
    colors = ['#27ae60' if x >= 0 else '#c0392b' for x in diffs]

    plt.figure(figsize=(12, 6))
    bars = plt.bar(labels, diffs, color=colors, alpha=0.8)
    
    for bar in bars:
        height = bar.get_height()
        plt.text(bar.get_x() + bar.get_width()/2., height,
                 f'{height:.2f}',
                 ha='center', va='bottom' if height > 0 else 'top',
                 fontsize=10, fontweight='bold')

    plt.axhline(0, color='black', linewidth=0.8)
    plt.title(f'{subject_name}：各類作業「有做 vs 沒做」的成績差異', fontsize=14)
    plt.ylabel('成績差異 (正值代表有助益)', fontsize=12)
    plt.xticks(rotation=0)
    plt.tight_layout()
    
    # 存檔
    save_current_plot(f"{subject_name}_2_作業類型影響.png")

def plot_performance_correlation(results, subject_name):
    """繪製作業數量與課堂表現的相關性熱圖"""
    set_chinese_font()
    
    if not results:
        return

    # 清理標籤名稱
    clean_labels = []
    for key in results.keys():
        if "跟得上" in key: clean_labels.append("跟得上進度")
        elif "用功" in key: clean_labels.append("用功程度")
        elif "作業" in key: clean_labels.append("作業表現")
        else: clean_labels.append(key)

    values = list(results.values())

    plt.figure(figsize=(8, 5))
    bars = plt.barh(clean_labels, values, color='#3498db', alpha=0.7)
    plt.axvline(0, color='black', linewidth=0.8)
    plt.title(f'{subject_name}：作業頻率與課堂表現相關性', fontsize=14)
    plt.xlabel('相關係數', fontsize=12)
    plt.xlim(-0.5, 0.5)
    
    for bar in bars:
        width = bar.get_width()
        plt.text(width, bar.get_y() + bar.get_height()/2,
                 f'{width:.2f}',
                 ha='left' if width > 0 else 'right', va='center',
                 fontsize=10)

    plt.tight_layout()
    
    # 存檔
    save_current_plot(f"{subject_name}_3_課堂表現相關性.png")