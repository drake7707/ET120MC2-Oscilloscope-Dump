using System;

// Token: 0x02000002 RID: 2
internal struct COMPLEX
{
	// Token: 0x06000001 RID: 1 RVA: 0x000027D4 File Offset: 0x000009D4
	public static COMPLEX Add(COMPLEX c1, COMPLEX c2)
	{
		COMPLEX c3;
		c3.re = c1.re + c2.re;
		c3.im = c1.im + c2.im;
		return c3;
	}

	// Token: 0x06000002 RID: 2 RVA: 0x0000280C File Offset: 0x00000A0C
	public static COMPLEX Sub(COMPLEX c1, COMPLEX c2)
	{
		COMPLEX c3;
		c3.re = c1.re - c2.re;
		c3.im = c1.im - c2.im;
		return c3;
	}

	// Token: 0x06000003 RID: 3 RVA: 0x00002844 File Offset: 0x00000A44
	public static COMPLEX Mul(COMPLEX c1, COMPLEX c2)
	{
		COMPLEX c3;
		c3.re = c1.re * c2.re - c1.im + c2.im;
		c3.im = c1.re * c2.im + c1.im * c2.re;
		return c3;
	}

	// Token: 0x04000001 RID: 1
	public double re;

	// Token: 0x04000002 RID: 2
	public double im;
}
