using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace LibraryManagmentWS
{
	public static class ImageUtils
	{
		public static System.IO.MemoryStream GetPhoto
			(string pathName, System.Drawing.Imaging.ImageFormat format , int? width = null, int? height = null)
		{
			System.IO.MemoryStream oMemoryStream =
				new System.IO.MemoryStream();

			if (!File.Exists(pathName))
			{
				pathName = "c:\\LibraryManagmentWebServiceData\\Default\\NoPhoto.jpg";
			}

			System.Drawing.Image oImage =
				System.Drawing.Image.FromFile(pathName);

			if ((width.HasValue == false) && (height.HasValue == false))
			{
				oImage.Save
					(oMemoryStream, oImage.RawFormat);

				return (oMemoryStream);
			}

			System.Drawing.Image.GetThumbnailImageAbort oThumbnailCallback =
				new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback);

			if ((width.HasValue) && (height.HasValue))
			{
				System.Drawing.Image SqureImage = GetSqureImageFromImage(oImage);
				System.Drawing.Image oThumbnailImage =
					SqureImage.GetThumbnailImage
						(width.Value, height.Value, oThumbnailCallback, System.IntPtr.Zero);

				oThumbnailImage.Save
					(oMemoryStream, oImage.RawFormat);

				return (oMemoryStream);
			}

			if ((width.HasValue) && (height.HasValue == false))
			{
				if (oImage.Width <= width.Value)
				{
					oImage.Save
						(oMemoryStream, oImage.RawFormat);

					return (oMemoryStream);
				}
				else
				{
					double dblCurrentPhotoWidth = (double)oImage.Width;
					double dblCurrentPhotoHeight = (double)oImage.Height;

					double dblFavoritePhotoWidth = (double)width.Value;

					double dblRatio =
						dblFavoritePhotoWidth / dblCurrentPhotoWidth;

					double dblFavoritePhotoHeight = dblCurrentPhotoHeight * dblRatio;

					height = (int)dblFavoritePhotoHeight;

					System.Drawing.Image oThumbnailImage =
						oImage.GetThumbnailImage
							(width.Value, height.Value, oThumbnailCallback, System.IntPtr.Zero);

					oThumbnailImage.Save
						(oMemoryStream, oImage.RawFormat);

					return (oMemoryStream);
				}
			}

			if ((width.HasValue == false) && (height.HasValue))
			{
				if (oImage.Height <= height.Value)
				{
					oImage.Save
						(oMemoryStream, oImage.RawFormat);

					return (oMemoryStream);
				}
				else
				{
					double dblCurrentPhotoWidth = (double)oImage.Width;
					double dblCurrentPhotoHeight = (double)oImage.Height;

					double dblFavoritePhotoHeight = (double)height.Value;

					double dblRatio =
						dblFavoritePhotoHeight / dblCurrentPhotoHeight;

					double dblFavoritePhotoWidth = dblCurrentPhotoWidth * dblRatio;

					width = (int)dblFavoritePhotoWidth;

					System.Drawing.Image oThumbnailImage =
						oImage.GetThumbnailImage
							(width.Value, height.Value, oThumbnailCallback, System.IntPtr.Zero);

					oThumbnailImage.Save
						(oMemoryStream, oImage.RawFormat);

					return (oMemoryStream);
				}
			}

			return (oMemoryStream);
		}

		public static System.IO.MemoryStream GetPhoto
			(System.Drawing.Image oImage, System.Drawing.Imaging.ImageFormat format, int? width = null, int? height = null)
		{
			System.IO.MemoryStream oMemoryStream =
				new System.IO.MemoryStream();

			if (oImage == null)
			{
				oImage =
				   System.Drawing.Image.FromFile("c:\\LibraryManagmentWebServiceData\\Default\\NoPhoto.jpg");	
			}

			if ((width.HasValue == false) && (height.HasValue == false))
			{
				oImage.Save
					(oMemoryStream, format);

				return (oMemoryStream);
			}

			System.Drawing.Image.GetThumbnailImageAbort oThumbnailCallback =
				new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback);

			if ((width.HasValue) && (height.HasValue))
			{
				System.Drawing.Image SqureImage = GetSqureImageFromImage(oImage);
				System.Drawing.Image oThumbnailImage =
					SqureImage.GetThumbnailImage
						(width.Value, height.Value, oThumbnailCallback, System.IntPtr.Zero);

				oThumbnailImage.Save
					(oMemoryStream, format);

				return (oMemoryStream);
			}

			if ((width.HasValue) && (height.HasValue == false))
			{
				if (oImage.Width <= width.Value)
				{
					oImage.Save
						(oMemoryStream, format);

					return (oMemoryStream);
				}
				else
				{
					double dblCurrentPhotoWidth = (double)oImage.Width;
					double dblCurrentPhotoHeight = (double)oImage.Height;

					double dblFavoritePhotoWidth = (double)width.Value;

					double dblRatio =
						dblFavoritePhotoWidth / dblCurrentPhotoWidth;

					double dblFavoritePhotoHeight = dblCurrentPhotoHeight * dblRatio;

					height = (int)dblFavoritePhotoHeight;

					System.Drawing.Image oThumbnailImage =
						oImage.GetThumbnailImage
							(width.Value, height.Value, oThumbnailCallback, System.IntPtr.Zero);

					oThumbnailImage.Save
						(oMemoryStream, format);

					return (oMemoryStream);
				}
			}

			if ((width.HasValue == false) && (height.HasValue))
			{
				if (oImage.Height <= height.Value)
				{
					oImage.Save
						(oMemoryStream, format);

					return (oMemoryStream);
				}
				else
				{
					double dblCurrentPhotoWidth = (double)oImage.Width;
					double dblCurrentPhotoHeight = (double)oImage.Height;

					double dblFavoritePhotoHeight = (double)height.Value;

					double dblRatio =
						dblFavoritePhotoHeight / dblCurrentPhotoHeight;

					double dblFavoritePhotoWidth = dblCurrentPhotoWidth * dblRatio;

					width = (int)dblFavoritePhotoWidth;

					System.Drawing.Image oThumbnailImage =
						oImage.GetThumbnailImage
							(width.Value, height.Value, oThumbnailCallback, System.IntPtr.Zero);

					oThumbnailImage.Save
						(oMemoryStream, format);

					return (oMemoryStream);
				}
			}

			return (oMemoryStream);
		}

		public static System.Drawing.Image GetThumbnailByImageObject
		(System.Drawing.Image oImage, int? width = null, int? height = null)
		{
			if (oImage == null)
			{
				oImage =
				   System.Drawing.Image.FromFile("c:\\LibraryManagmentWebServiceData\\Default\\NoPhoto.jpg");
			}

			System.Drawing.Image.GetThumbnailImageAbort oThumbnailCallback =
				new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback);

			if ((width.HasValue) && (height.HasValue))
			{
				System.Drawing.Image SqureImage = GetSqureImageFromImage(oImage);
				System.Drawing.Image oThumbnailImage =
					SqureImage.GetThumbnailImage
						(width.Value, height.Value, oThumbnailCallback, System.IntPtr.Zero);

				return (oThumbnailImage);
			}

			if ((width.HasValue) && (height.HasValue == false))
			{
				if (oImage.Width <= width.Value)
				{
					return (oImage);
				}
				else
				{
					double dblCurrentPhotoWidth = (double)oImage.Width;
					double dblCurrentPhotoHeight = (double)oImage.Height;

					double dblFavoritePhotoWidth = (double)width.Value;

					double dblRatio =
						dblFavoritePhotoWidth / dblCurrentPhotoWidth;

					double dblFavoritePhotoHeight = dblCurrentPhotoHeight * dblRatio;

					height = (int)dblFavoritePhotoHeight;

					System.Drawing.Image oThumbnailImage =
						oImage.GetThumbnailImage
							(width.Value, height.Value, oThumbnailCallback, System.IntPtr.Zero);


					return (oThumbnailImage);
				}
			}

			if ((width.HasValue == false) && (height.HasValue))
			{
				if (oImage.Height <= height.Value)
				{

					return (oImage);
				}
				else
				{
					double dblCurrentPhotoWidth = (double)oImage.Width;
					double dblCurrentPhotoHeight = (double)oImage.Height;

					double dblFavoritePhotoHeight = (double)height.Value;

					double dblRatio =
						dblFavoritePhotoHeight / dblCurrentPhotoHeight;

					double dblFavoritePhotoWidth = dblCurrentPhotoWidth * dblRatio;

					width = (int)dblFavoritePhotoWidth;

					System.Drawing.Image oThumbnailImage =
						oImage.GetThumbnailImage
							(width.Value, height.Value, oThumbnailCallback, System.IntPtr.Zero);

					return (oThumbnailImage);
				}
			}

			return (oImage);
		}

		public static System.Drawing.Image GetThumbnailByImagePath
		(string pathName, int? width = null, int? height = null)
		{
			if (!File.Exists(pathName))
			{
				pathName = "c:\\LibraryManagmentWebServiceData\\Default\\NoPhoto.jpg";
			}

			System.Drawing.Image oImage =
				System.Drawing.Image.FromFile(pathName);

			System.Drawing.Image.GetThumbnailImageAbort oThumbnailCallback =
				new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback);

			if ((width.HasValue) && (height.HasValue))
			{
				System.Drawing.Image SqureImage = GetSqureImageFromImage(oImage);
				System.Drawing.Image oThumbnailImage =
					SqureImage.GetThumbnailImage
						(width.Value, height.Value, oThumbnailCallback, System.IntPtr.Zero);

				return (oThumbnailImage);
			}

			if ((width.HasValue) && (height.HasValue == false))
			{
				if (oImage.Width <= width.Value)
				{
					return (oImage);
				}
				else
				{
					double dblCurrentPhotoWidth = (double)oImage.Width;
					double dblCurrentPhotoHeight = (double)oImage.Height;

					double dblFavoritePhotoWidth = (double)width.Value;

					double dblRatio =
						dblFavoritePhotoWidth / dblCurrentPhotoWidth;

					double dblFavoritePhotoHeight = dblCurrentPhotoHeight * dblRatio;

					height = (int)dblFavoritePhotoHeight;

					System.Drawing.Image oThumbnailImage =
						oImage.GetThumbnailImage
							(width.Value, height.Value, oThumbnailCallback, System.IntPtr.Zero);


					return (oThumbnailImage);
				}
			}

			if ((width.HasValue == false) && (height.HasValue))
			{
				if (oImage.Height <= height.Value)
				{

					return (oImage);
				}
				else
				{
					double dblCurrentPhotoWidth = (double)oImage.Width;
					double dblCurrentPhotoHeight = (double)oImage.Height;

					double dblFavoritePhotoHeight = (double)height.Value;

					double dblRatio =
						dblFavoritePhotoHeight / dblCurrentPhotoHeight;

					double dblFavoritePhotoWidth = dblCurrentPhotoWidth * dblRatio;

					width = (int)dblFavoritePhotoWidth;

					System.Drawing.Image oThumbnailImage =
						oImage.GetThumbnailImage
							(width.Value, height.Value, oThumbnailCallback, System.IntPtr.Zero);

					return (oThumbnailImage);
				}
			}

			return (oImage);
		}

		public static System.Drawing.Image GetSqureImageFromImage(System.Drawing.Image oImage)
		{
			int dblCurrentPhotoWidth = oImage.Width;
			int dblCurrentPhotoHeight = oImage.Height;

			if (dblCurrentPhotoWidth > dblCurrentPhotoHeight)
			{
				int dblDifferenceBetweenWidthHeight = dblCurrentPhotoWidth - dblCurrentPhotoHeight;
				int intTargetHalf = (int)(dblDifferenceBetweenWidthHeight / 2);
				System.Drawing.Rectangle oTargetRect = new System.Drawing.Rectangle()
				{
					Size = new System.Drawing.Size() { Width = dblCurrentPhotoHeight, Height = dblCurrentPhotoHeight },
					Location = new System.Drawing.Point() { X = intTargetHalf, Y = 0 },
				};

				var newBitMap = new System.Drawing.Bitmap(dblCurrentPhotoHeight, dblCurrentPhotoHeight);
				var graphic = System.Drawing.Graphics.FromImage(newBitMap);
				graphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
				graphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
				graphic.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
				graphic.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
				graphic.DrawImage(oImage, 0, 0, oTargetRect, System.Drawing.GraphicsUnit.Pixel);
				graphic.Save();

				return (newBitMap);
			}
			else if (dblCurrentPhotoWidth == dblCurrentPhotoHeight)
			{
				return (oImage);
			}
			else if (dblCurrentPhotoWidth < dblCurrentPhotoHeight)
			{
				int dblDifferenceBetweenWidthHeight = dblCurrentPhotoHeight - dblCurrentPhotoWidth;
				int intTargetHalf = (int)(dblDifferenceBetweenWidthHeight / 2);
				System.Drawing.Rectangle oTargetRect = new System.Drawing.Rectangle()
				{
					Size = new System.Drawing.Size() { Width = dblCurrentPhotoWidth, Height = dblCurrentPhotoWidth },
					Location = new System.Drawing.Point() { X = 0, Y = intTargetHalf },
				};

				var newBitMap = new System.Drawing.Bitmap(dblCurrentPhotoWidth, dblCurrentPhotoWidth);
				var graphic = System.Drawing.Graphics.FromImage(newBitMap);
				graphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
				graphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
				graphic.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
				graphic.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
				graphic.DrawImage(oImage, 0, 0, oTargetRect, System.Drawing.GraphicsUnit.Pixel);
				graphic.Save();

				return (newBitMap);
			}

			return (oImage);
		}

		public static bool ThumbnailCallback()
		{
			return (false);
		}
	}
}
