import csv
from itertools import combinations, chain
import time

def load_data(filename):
    transactions = []
    with open(filename, 'r') as f:
        reader = csv.DictReader(f)
        for row in reader:
            items = row['Items'].split(',')
            transactions.append(set(items))
    return transactions

def generate_candidates(frequent_itemsets, k):
    candidates = set()
    items = list(frequent_itemsets)
    n = len(items)
    for i in range(n):
        for j in range(i+1, n):
            union_set = items[i].union(items[j])
            if len(union_set) == k:
                candidates.add(frozenset(union_set))
    return candidates

def calculate_support(transactions, candidates, min_support):
    item_count = {}
    for candidate in candidates:
        for transaction in transactions:
            if candidate.issubset(transaction):
                item_count[candidate] = item_count.get(candidate, 0) + 1

    frequent_itemsets = {}
    for itemset, count in item_count.items():
        support = count / len(transactions)
        if support >= min_support:
            frequent_itemsets[itemset] = support
    return frequent_itemsets

def apriori(transactions, min_support=0.5):
    all_items = set(chain.from_iterable(transactions))
    candidates_1 = [frozenset([item]) for item in all_items]
    frequent_itemsets = calculate_support(transactions, candidates_1, min_support)
    
    all_frequent_itemsets = dict(frequent_itemsets)
    k = 2
    
    while frequent_itemsets:
        candidates_k = generate_candidates(frequent_itemsets.keys(), k)
        frequent_itemsets = calculate_support(transactions, candidates_k, min_support)
        all_frequent_itemsets.update(frequent_itemsets)
        k += 1
    
    return all_frequent_itemsets

def generate_rules_largest_k(frequent_itemsets, min_confidence=0.7):
    rules = []
    if not frequent_itemsets:
        return rules

    max_size = max(len(itemset) for itemset in frequent_itemsets)

    for itemset in frequent_itemsets:
        if len(itemset) < 2 or len(itemset) != max_size:
            continue
        for i in range(1, len(itemset)):
            for antecedent in combinations(itemset, i):
                antecedent = frozenset(antecedent)
                consequent = itemset - antecedent
                confidence = frequent_itemsets[itemset] / frequent_itemsets[antecedent]
                if confidence >= min_confidence:
                    rules.append((set(antecedent), set(consequent), confidence))
    return rules

def get_largest_frequent_itemsets(frequent_itemsets):
    if not frequent_itemsets:
        return {}
    max_size = max(len(itemset) for itemset in frequent_itemsets)
    largest_itemsets = {itemset: support for itemset, support in frequent_itemsets.items() if len(itemset) == max_size}
    return largest_itemsets

def Apriori_Main(data_name="test1.csv",min_sup=0.6,min_conf=0.7):
    transactions = load_data(data_name)
    min_support = min_sup
    min_confidence = min_conf

    frequent_itemsets = apriori(transactions, min_support)

    largest_frequent_itemsets = get_largest_frequent_itemsets(frequent_itemsets)
    print("Largest Frequent Itemsets | Support:")
    if (len(largest_frequent_itemsets) > 0):
        for itemset, support in largest_frequent_itemsets.items():
            print(f"{set(itemset)}: {support:.2f}")
    else:
        print("None")

    rules_largest_k = generate_rules_largest_k(frequent_itemsets, min_confidence)
    print("Association Rules | Confidence:")
    if (len(rules_largest_k) > 0):
        for antecedent, consequent, confidence in rules_largest_k:
            print(f"{antecedent} -> {consequent}: {confidence:.2f}")
    else:
        print("None")