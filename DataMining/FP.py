# started at 10/21 21:31
# finished at 10/24 12:11
# total taken time : around 15 hrs

from collections import Counter
import csv

first_appearance = {}

def load_data(filename):
    transactions = []
    with open(filename, 'r') as f:
        reader = csv.DictReader(f)
        for row in reader:
            items = row['Items'].split(',')
            transactions.append(set(items))
    return transactions

class Node:
    def __init__(self, name, count, parent):
        self.name = name
        self.count = count
        self.parent = parent
        self.children = {}
        self.link = None

    def display(self, ind=1):
        print('  ' * ind, f'{self.name}:{self.count}')
        for child in self.children.values():
            child.display(ind + 1)

def build_fp_tree(transactions, min_support):
    header = Counter()
    for t in transactions:
        for i in t:
            header[i] += 1
    header = {k: v for k, v in header.items() if v >= min_support}
    if not header:
        return None, None

    ordered_header = sorted(header, key=lambda i: (-header[i], first_appearance[i]))
    header_table = {i: [header[i], None] for i in ordered_header}

    root = Node('Null', 1, None)
    for t in transactions:
        ordered = [i for i in ordered_header if i in t]
        current = root
        for i in ordered:
            if i in current.children:
                current.children[i].count += 1
            else:
                new_node = Node(i, 1, current)
                current.children[i] = new_node
               
                if header_table[i][1] is None: # Link nodes in header table
                    header_table[i][1] = new_node
                else:
                    link_node = header_table[i][1]
                    while link_node.link is not None:
                        link_node = link_node.link
                    link_node.link = new_node
            current = current.children[i]
    return root, header_table

def find_prefix_paths(base_pat, header_table):
    cond_pats = {}
    node = header_table[base_pat][1]
    while node is not None:
        prefix_path = []
        parent = node.parent
        while parent is not None and parent.name != 'Null':
            prefix_path.append(parent.name)
            parent = parent.parent
        prefix_path.reverse()
        if prefix_path:
            cond_pats[frozenset(prefix_path)] = node.count
        node = node.link
    return cond_pats

def mine_tree(header_table, min_support, prefix, freq_item_list):
    sorted_items = [v[0] for v in sorted(header_table.items(), key=lambda x: (x[1][0], first_appearance[x[0]]))]
    for base_pat in sorted_items:
        new_freq_set = prefix.copy()
        new_freq_set.add(base_pat)
        freq_item_list.append((new_freq_set, header_table[base_pat][0]))
        cond_pats = find_prefix_paths(base_pat, header_table)
        cond_trans = []
        for path, count in cond_pats.items():
            trans = list(path)
            for _ in range(count):
                cond_trans.append(trans)
        cond_tree, cond_header = build_fp_tree(cond_trans, min_support)
        if cond_header is not None:
            mine_tree(cond_header, min_support, new_freq_set, freq_item_list)

def FPGrowth(filename, min_supp):
    path = filename
    transactions = load_data(path)

    min_support = min_supp

    item_counts = Counter()
    for items in transactions:
        for item in items:
            item_counts[item] += 1


    frequent_items = {item: count for item, count in item_counts.items() if count/len(transactions) >= min_support} # Keep only frequent items

    for items in transactions:
        for item in items:
            if item not in first_appearance:
                first_appearance[item] = len(first_appearance)

    F_list = sorted(frequent_items, key=lambda i: (-frequent_items[i], first_appearance[i]))
    print("\nF-list:", F_list)

    ordered_transactions = []
    for items in transactions:
        ordered = [i for i in F_list if i in items]
        if ordered:
            ordered_transactions.append(ordered)

    print("\nOrdered Transactions:") 
    for i, t in enumerate(ordered_transactions, 1):
        print(f"T{i}: {t}")

    root, header_table = build_fp_tree(ordered_transactions, min_support)

    # print("\nFP-tree Structure:")
    # root.display()

    freq_item_list = []
    mine_tree(header_table, min_support, set(), freq_item_list)

    print("\nFrequent Patterns (itemset : support):")
    for pattern, support in sorted(freq_item_list, key=lambda x: (len(x[0]), sorted(x[0]))):
        print(f"{set(pattern)} : {support}")