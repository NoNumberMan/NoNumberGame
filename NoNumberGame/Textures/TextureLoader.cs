using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace NoNumberGame
{
	public static class TextureLoader
	{
		public static Texture LoadTexture( string textureLocation ) {
			Bitmap bmp = new Bitmap( textureLocation );
			bmp.RotateFlip( RotateFlipType.RotateNoneFlipY ); //y=0 must be at the top

			int textureId = GL.GenTexture();
			GL.BindTexture( TextureTarget.Texture2D, textureId );

			//These are temporary. Will set the tex params properly when needed
			GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureWrapS, ( int ) TextureWrapMode.Repeat );
			GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureWrapT, ( int ) TextureWrapMode.Repeat );
			GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, ( int ) TextureMinFilter.Linear );
			GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, ( int ) TextureMagFilter.Linear );
			
			BitmapData bmpData = bmp.LockBits( new Rectangle( 0, 0, bmp.Width, bmp.Height ), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb );
			GL.TexImage2D( TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp.Width, bmp.Height,
				0, PixelFormat.Bgra, PixelType.UnsignedByte, bmpData.Scan0 );
			bmp.UnlockBits( bmpData );

			GL.GenerateMipmap( GenerateMipmapTarget.Texture2D );
			GL.BindTexture( TextureTarget.Texture2D, 0 );

			return new Texture( textureId );
		}
	}
}