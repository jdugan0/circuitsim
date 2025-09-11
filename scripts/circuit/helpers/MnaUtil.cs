static class MnaUtil
{
    public static void Add(ref double[,] M, int r, int c, double v)
    {
        if (r >= 0 && c >= 0) M[r, c] += v;
    }
    public static void Set(ref double[] v, int i, double val)
    {
        if (i >= 0) v[i] = val;
    }
}