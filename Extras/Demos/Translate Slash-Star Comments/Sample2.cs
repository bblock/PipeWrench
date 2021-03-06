
/* TODO: Remember to blah, blah... */

public static Bitmap MakeGrayscale(Bitmap original)
{
	/******************************************************
	This minimally-altered csharp example code was borrowed 
	from the nice folks at http://www.switchonthecode.com 
	******************************************************/

	/* make an empty bitmap the 
	same size as original */

	Bitmap newBitmap = new Bitmap(original.Width, original.Height); /* the bitmap */

	for (int i = 0; i < original.Width; i++)
	{
		for (int j = 0; j < original.Height; j++)
		{
			/* get the pixel from the original image */

			Color originalColor = original.GetPixel(i, j);

			/* create the grayscale 
			version of the pixel */

			int grayScale = (int)((originalColor.R * .3) + 
			(originalColor.G * .59) + (originalColor.B * .11));

			/* create the color object */

			Color newColor = Color.FromArgb(grayScale, grayScale, grayScale);

			/* set the new image's pixel 
			to the grayscale version */

			newBitmap.SetPixel(i, j, newColor);
		}
	}

	return newBitmap;
}

