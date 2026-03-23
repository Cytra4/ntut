
from config import SUBJECTS, COLUMN_MAPPING, PERFORMANCE_LABELS
from data_loader import load_and_clean_data
import analyzers
import visualizer

def main():
    df = load_and_clean_data()
    if df is None:
        return

    for subj_code, subj_name in SUBJECTS.items():
        print(f"\n正在分析科目: {subj_name} ({subj_code})...")
        subj_config = COLUMN_MAPPING[subj_code]
        
 
        if subj_config['score'] not in df.columns:
            print(f"警告: 找不到 {subj_name} 的成績欄位 {subj_config['score']}")
            continue
        qty_result = analyzers.analyze_homework_quantity_vs_score(df, subj_config)
        if qty_result:

            visualizer.plot_homework_quantity_vs_score(qty_result, subj_name)
        
        type_result = analyzers.analyze_homework_type_impact(df, subj_config)
        if type_result:
        
            visualizer.plot_homework_type_impact(type_result, subj_name)
            
            for t, res in type_result.items():
                diff = res['diff']
             
        perf_result = analyzers.analyze_performance_correlation(df, subj_config)
        
        if perf_result:
            visualizer.plot_performance_correlation(perf_result, subj_name)
    
        # 極端值 
        print(f"驗證極端值")
        optimal_result = analyzers.analyze_optimal_frequency(df, subj_config)
        
        if optimal_result is not None:
            print(optimal_result)
            means = optimal_result['mean']
        print("-" * 30) 

    print("\n=== 分析完成，圖表已顯示 ===")

if __name__ == "__main__":
    main()