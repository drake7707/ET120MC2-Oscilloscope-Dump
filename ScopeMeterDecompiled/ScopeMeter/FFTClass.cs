using System;

// Token: 0x02000003 RID: 3
internal static class FFTClass
{
	// Token: 0x06000004 RID: 4 RVA: 0x00002898 File Offset: 0x00000A98
	public static void FFT(COMPLEX[] TD, COMPLEX[] FD, int power)
	{
		int count = 1 << power;
		COMPLEX[] W = new COMPLEX[count / 2];
		COMPLEX[] X = new COMPLEX[count];
		COMPLEX[] X2 = new COMPLEX[count];
		for (int i = 0; i < count / 2; i++)
		{
			double angle = (double)(-(double)i) * 3.1415926535897931 * 2.0 / (double)count;
			W[i].re = Math.Cos(angle);
			W[i].im = Math.Sin(angle);
		}
		TD.CopyTo(X, 0);
		for (int j = 0; j < power; j++)
		{
			for (int k = 0; k < 1 << j; k++)
			{
				int bfsize = 1 << power - j;
				for (int i = 0; i < bfsize / 2; i++)
				{
					int p = k * bfsize;
					X2[i + p] = COMPLEX.Add(X[i + p], X[i + p + bfsize / 2]);
					X2[i + p + bfsize / 2] = COMPLEX.Mul(COMPLEX.Sub(X[i + p], X[i + p + bfsize / 2]), W[i * (1 << j)]);
				}
			}
			COMPLEX[] array = X;
			X = X2;
			X2 = array;
		}
		for (int k = 0; k < count; k++)
		{
			int p = 0;
			for (int i = 0; i < power; i++)
			{
				if ((k & 1 << i) != 0)
				{
					p += 1 << power - i - 1;
				}
			}
			FD[k] = X[p];
		}
	}
}
