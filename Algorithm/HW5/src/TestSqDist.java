public class TestSqDist {
	static void test(double[] a, double[] b, double expected) {
        double result = KDTree.sqDist(a, b);

        if (result != expected) {
            System.out.println("FAIL");
            System.out.println("Expected: " + expected + ", Got: " + result);
        } else {
            System.out.println("PASS");
        }
    }

    public static void main(String[] args) {
        test(new double[]{-1, 1}, new double[]{1, -1}, 8);
        test(new double[]{2, 3}, new double[]{2, 3}, 0);
        test(new double[]{5}, new double[]{2}, 9);
        test(new double[]{1, 2, 3}, new double[]{4, 6, 3}, 25);
        // (3^2 + 4^2 + 0^2 = 9 + 16 = 25)
        test(new double[]{}, new double[]{}, 0);
		test(
            new double[]{1,2,3,4,5},
            new double[]{5,4,3,2,1},
            40
        );
        // (4^2 + 2^2 + 0 + 2^2 + 4^2 = 16+4+0+4+16 = 40)
        double[] a = new double[1000];
        double[] b = new double[1000];
        for (int i = 0; i < 1000; i++) {
            a[i] = i;
            b[i] = i + 1;
        }
        // each diff = 1 → sum = 1000
        test(a, b, 1000);
    }

}
