using System;
using System.Diagnostics;
using System.IO;
using System.Security.Authentication.ExtendedProtection;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace NoNumberGame
{
	public static class Program
	{
		public static void Main( string[] args ) {
			Console.WriteLine( "Hello World!" );

			GameWindowSettings   gws = GameWindowSettings.Default;
			NativeWindowSettings nws = NativeWindowSettings.Default;
			gws.IsMultiThreaded = false;
			gws.RenderFrequency = 60;
			gws.UpdateFrequency = 60;

			nws.APIVersion = Version.Parse( "4.1.0" );
			nws.Size       = new Vector2i( 1280, 720 );
			nws.Title      = "NoNumberGame";

			GameWindow window = new GameWindow( gws, nws );
			Camera     cam    = new Camera();

			int           uniformProj   = -1;
			ShaderProgram shaderProgram = new ShaderProgram() { id = 0 };
			window.Load += () => {
				shaderProgram = LoadShaderProgram( "../../../vertex_shader.glsl", "../../../fragment_shader.glsl" );
				uniformProj   = GL.GetUniformLocation( shaderProgram.id, "proj" );
			};

			window.KeyDown += ( args ) => {
				switch ( args.Key ) {
					case Keys.W:
						cam.MoveForwards( 0.2f );
						break;
					case Keys.S:
						cam.MoveForwards( -0.2f );
						break;
					case Keys.A:
						cam.MoveSideways( 0.2f );
						break;
					case Keys.D:
						cam.MoveSideways( -0.2f );
						break;
					case Keys.Space:
						cam.MoveUpwards( -0.2f );
						break;
					case Keys.LeftShift:
						cam.MoveUpwards( 0.2f );
						break;
					default: break;
				}
			};

			window.Resize += ( args ) => {
				GL.Viewport( 0, 0, args.Width, args.Height );
			};

			window.MouseMove += ( args ) => {
				if ( window.IsMouseButtonDown( MouseButton.Middle ) ) {
					cam.Rotate( args.DeltaY / 100.0f, -args.DeltaX / 100.0f, 0.0f );
				}
			};



			float[] heightmap = NoiseGenerator.GenerateNoise2D( 0, 0, 256, 256, 4,
				new[] { 2, 8, 32, 128 }, new[] { 0.04f, 0.1f, 1.0f, 0.4f } );


			float[][] colorMap = new float[][] {
				new float[] { 0.0f, 1.0f, 0.0f },    //grass
				new float[] { 0.5f, 0.5f, 0.1f },    //sand
				new float[] { 0.4f, 0.4f, 0.4f },    //rock
				new float[] { 0.95f, 0.95f, 0.95f }, //snow
			};


			float[] vertexArray = new float[256 * 256 * 3];
			float[] colorArray  = new float[256 * 256 * 3];
			int[]   indexArray  = new int[255 * 255   * 6];

			Random rand = new Random();
			for ( int z = 0; z < 256; ++z )
			for ( int x = 0; x < 256; ++x ) {
				float y = heightmap[x + 256 * z];

				vertexArray[3 * ( x + 256 * z ) + 0] = x - 0.0f;
				vertexArray[3 * ( x + 256 * z ) + 1] = -y * 64.0f;
				vertexArray[3 * ( x + 256 * z ) + 2] = z - 0.0f;

				int colorIndex = 0;
				float yc = y + 0.1f * ( float ) rand.NextDouble();
				
				if ( yc      < 0.3f ) colorIndex = 1;
				else if ( yc < 0.5f ) colorIndex = 0;
				else if ( yc < 0.7f ) colorIndex = 2;
				else colorIndex                 = 3;
				colorArray[3 * ( x + 256 * z ) + 0] = colorMap[colorIndex][0];
				colorArray[3 * ( x + 256 * z ) + 1] = colorMap[colorIndex][1];
				colorArray[3 * ( x + 256 * z ) + 2] = colorMap[colorIndex][2];

				if ( x < 255 && z < 255 ) {
					indexArray[6 * ( x + 255 * z ) + 0] = x         + 256 * z;
					indexArray[6 * ( x + 255 * z ) + 1] = x         + 256 * ( z + 1 );
					indexArray[6 * ( x + 255 * z ) + 2] = ( x + 1 ) + 256 * z;
					indexArray[6 * ( x + 255 * z ) + 3] = ( x + 1 ) + 256 * z;
					indexArray[6 * ( x + 255 * z ) + 4] = x         + 256 * ( z + 1 );
					indexArray[6 * ( x + 255 * z ) + 5] = ( x + 1 ) + 256 * ( z + 1 );
				}
			}
			
			for ( int i = 0; i < 255 * 255 * 2; ++i ) {
				float dc = 0.90f + 0.2f * ( float ) rand.NextDouble();
				colorArray[3 * indexArray[3 * i + 0] + 0] *= dc;
				colorArray[3 * indexArray[3 * i + 0] + 1] *= dc;
				colorArray[3 * indexArray[3 * i + 0] + 2] *= dc;
				colorArray[3 * indexArray[3 * i + 1] + 0] *= dc;
				colorArray[3 * indexArray[3 * i + 1] + 1] *= dc;
				colorArray[3 * indexArray[3 * i + 1] + 2] *= dc;
				colorArray[3 * indexArray[3 * i + 2] + 0] *= dc;
				colorArray[3 * indexArray[3 * i + 2] + 1] *= dc;
				colorArray[3 * indexArray[3 * i + 2] + 2] *= dc;
			}


			window.RenderFrame += ( FrameEventArgs args ) => {
				GL.UseProgram( shaderProgram.id );
				GL.Enable( EnableCap.DepthTest );

				GL.ClearColor( 0.2f, 0.5f, 0.8f, 0 );
				GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );

				Matrix4 projMatrix = cam.GetMatrix() * Matrix4.CreatePerspectiveFieldOfView( ( float ) Math.PI / 3.0f, ( float ) window.Size.X / ( float ) window.Size.Y, 0.1f, 2000.0f );
				GL.UniformMatrix4( uniformProj, false, ref projMatrix );



				int vao      = GL.GenVertexArray();
				int vertices = GL.GenBuffer();
				int colors   = GL.GenBuffer();
				int indices  = GL.GenBuffer();

				GL.BindVertexArray( vao );
				GL.BindBuffer( BufferTarget.ArrayBuffer, vertices );
				GL.BufferData( BufferTarget.ArrayBuffer, vertexArray.Length * sizeof ( float ), vertexArray, BufferUsageHint.StaticCopy );
				GL.EnableVertexAttribArray( 0 );
				GL.VertexAttribPointer( 0, 3, VertexAttribPointerType.Float, false, 0, 0 );

				GL.BindBuffer( BufferTarget.ArrayBuffer, colors );
				GL.BufferData( BufferTarget.ArrayBuffer, colorArray.Length * sizeof ( float ), colorArray, BufferUsageHint.StaticCopy );
				GL.EnableVertexAttribArray( 1 );
				GL.VertexAttribPointer( 1, 3, VertexAttribPointerType.Float, false, 0, 0 );

				GL.BindBuffer( BufferTarget.ElementArrayBuffer, indices );
				GL.BufferData( BufferTarget.ElementArrayBuffer, indexArray.Length * sizeof ( int ), indexArray, BufferUsageHint.StaticCopy );


				GL.DrawArrays( PrimitiveType.Triangles, 0, 3 );
				GL.DrawElements( PrimitiveType.Triangles, indexArray.Length - 1, DrawElementsType.UnsignedInt, indices );

				GL.BindBuffer( BufferTarget.ArrayBuffer, 0 );
				GL.BindVertexArray( 0 );
				GL.DeleteVertexArray( vao );
				GL.DeleteBuffer( vertices );
				GL.DeleteBuffer( colors );
				GL.DeleteBuffer( indices );



				window.SwapBuffers();
			};

			window.Run();
		}



		private static Shader LoadShader( string shaderLocation, ShaderType type ) {
			int shaderId = GL.CreateShader( type );
			GL.ShaderSource( shaderId, File.ReadAllText( shaderLocation ) );
			GL.CompileShader( shaderId );
			string infoLog = GL.GetShaderInfoLog( shaderId );
			if ( !string.IsNullOrEmpty( infoLog ) ) {
				throw new Exception( infoLog );
			}

			return new Shader() { id = shaderId };
		}

		private static ShaderProgram LoadShaderProgram( string vertexShaderLocation, string fragmentShaderLocation ) {
			int shaderProgramId = GL.CreateProgram();

			Shader vertexShader   = LoadShader( vertexShaderLocation, ShaderType.VertexShader );
			Shader fragmentShader = LoadShader( fragmentShaderLocation, ShaderType.FragmentShader );

			GL.AttachShader( shaderProgramId, vertexShader.id );
			GL.AttachShader( shaderProgramId, fragmentShader.id );
			GL.LinkProgram( shaderProgramId );
			GL.DetachShader( shaderProgramId, vertexShader.id );
			GL.DetachShader( shaderProgramId, fragmentShader.id );
			GL.DeleteShader( vertexShader.id );
			GL.DeleteShader( fragmentShader.id );

			string infoLog = GL.GetProgramInfoLog( shaderProgramId );
			if ( !string.IsNullOrEmpty( infoLog ) ) {
				throw new Exception( infoLog );
			}

			return new ShaderProgram() { id = shaderProgramId };
		}



		public struct Shader
		{
			public int id;
		}

		public struct ShaderProgram
		{
			public int id;
		}
	}
}