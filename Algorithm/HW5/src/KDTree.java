import java.util.Vector;

public class KDTree {
	int depth;
	double[] point;
	KDTree left;
	KDTree right;

	KDTree(double[] point, int depth) {
		this.point = point;
		this.depth = depth;
	}

	double diff(double[] a) {
		int r = depth % point.length;
		return a[r] - point[r];
	}

	boolean compare(double[] a) {
		return diff(a) >= 0;
	}

	static KDTree insert(KDTree tree, double[] p) {
		if (tree == null) {
			return new KDTree(p, 0);
		}

		if (tree.compare(p)) {
			if (tree.right == null) {
				tree.right = new KDTree(p, tree.depth + 1);
			} else {
				insert(tree.right, p);
			}
		} else {
			if (tree.left == null) {
				tree.left = new KDTree(p, tree.depth + 1);
			} else {
				insert(tree.left, p);
			}
		}
		return tree;
	}

	static double sqDist(double[] a, double[] b) {
		double result = 0;
		for (int i = 0; i < a.length; i++) {
			result += Math.pow(a[i] - b[i], 2);
		}
		return result;
	}

	static double[] closestNaive(KDTree tree, double[] a, double[] champion) {
		if (tree == null) {
			return champion;
		}

		if (champion == null || sqDist(tree.point, a) < sqDist(champion, a)) {
			champion = tree.point;
		}

		champion = closestNaive(tree.left, a, champion);
		champion = closestNaive(tree.right, a, champion);

		return champion;
	}

	static double[] closestNaive(KDTree tree, double[] a) {
		return closestNaive(tree, a, null);
	}

	static double[] closest(KDTree tree, double[] a, double[] champion) {
		if (tree == null)
			return champion;

		InteractiveClosest.trace(tree.point, champion);

		if (champion == null || sqDist(tree.point, a) < sqDist(champion, a)) {
			champion = tree.point;
		}

		KDTree first, second;
		if (tree.compare(a)) {
			first = tree.right;
			second = tree.left;
		} else {
			first = tree.left;
			second = tree.right;
		}

		champion = closest(first, a, champion);

		double planeDist = Math.pow(tree.diff(a), 2);

		if (champion == null || planeDist < sqDist(champion, a)) {
			champion = closest(second, a, champion);
		}

		return champion;
	}

	static double[] closest(KDTree tree, double[] a) {
		if (tree == null)
			return null;
		return closest(tree, a, tree.point);
	}

	static int size(KDTree tree) {
		if (tree == null)
			return 0;
		return 1 + size(tree.left) + size(tree.right);
	}

	static void sum(KDTree tree, double[] acc) {
		if (tree == null)
			return;

		for (int i = 0; i < tree.point.length; i++) {
			acc[i] += tree.point[i];
		}

		sum(tree.left, acc);
		sum(tree.right, acc);
	}

	static double[] average(KDTree tree) {
		if (tree == null)
			return null;

		int n = size(tree);

		double[] acc = new double[tree.point.length];
		sum(tree, acc);

		for (int i = 0; i < acc.length; i++) {
			acc[i] /= n;
		}

		return acc;
	}

	// Soulution 1:
	// static KDTree largest(Vector<KDTree> subs) {
	// KDTree best = null;
	// int bestSize = -1;
	// for (KDTree t : subs) {
	// int s = size(t);
	// if (s > bestSize) {
	// bestSize = s;
	// best = t;
	// }
	// }
	// return best;
	// }

	// static Vector<double[]> palette(KDTree tree, int maxpoints) {
	// Vector<KDTree> subtrees = new Vector<>();
	// subtrees.add(tree);

	// while (subtrees.size() < maxpoints) {

	// KDTree biggest = largest(subtrees);
	// subtrees.remove(biggest);

	// if (biggest == null || (biggest.left == null && biggest.right == null)){
	// subtrees.add(biggest);
	// break;
	// }

	// if (biggest.left != null)
	// subtrees.add(biggest.left);
	// if (biggest.right != null)
	// subtrees.add(biggest.right);
	// }

	// Vector<double[]> result = new Vector<>();
	// for (KDTree t : subtrees) {
	// result.add(average(t));
	// }

	// return result;
	// }

	// Solution 2
	static void varianceRec(KDTree tree, double[] avg, double[] acc) {
		if (tree == null)
			return;

		acc[0] += sqDist(tree.point, avg);

		varianceRec(tree.left, avg, acc);
		varianceRec(tree.right, avg, acc);
	}
	
	static double variance(KDTree tree) {
		if (tree == null)
			return 0;

		double[] avg = average(tree);
		double[] acc = new double[1];

		varianceRec(tree, avg, acc);

		return acc[0] / size(tree);
	}

	static KDTree bestToSplit(Vector<KDTree> subs) {
		KDTree best = null;
		double bestScore = -1;

		for (KDTree t : subs) {
			double score = variance(t) * size(t);
			if (score > bestScore) {
				bestScore = score;
				best = t;
			}
		}
		return best;
	}

	static Vector<double[]> palette(KDTree tree, int maxpoints) {
		Vector<KDTree> subtrees = new Vector<>();
		subtrees.add(tree);

		while (subtrees.size() < maxpoints) {
			KDTree t = bestToSplit(subtrees);
			subtrees.remove(t);

			if (t == null || (t.left == null && t.right == null)) {
				subtrees.add(t);
				break;
			}

			if (t.left != null)
				subtrees.add(t.left);
			if (t.right != null)
				subtrees.add(t.right);
		}

		Vector<double[]> result = new Vector<>();
		for (KDTree t : subtrees)
			result.add(average(t));

		return result;
	}

	public String pointToString() {
		StringBuffer sb = new StringBuffer();
		sb.append("[");
		if (this.point.length > 0)
			sb.append(this.point[0]);
		for (int i = 1; i < this.point.length; i++)
			sb.append("," + this.point[i]);
		sb.append("]");
		return sb.toString();
	}

}
