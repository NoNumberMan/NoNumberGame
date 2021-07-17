using System;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

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

			int uniformSaturation = -1;
			int uniformProj = -1;
			ShaderProgram shaderProgram = new ShaderProgram(){id=0};
			window.Load += () => {
				shaderProgram = LoadShaderProgram("../../../vertex_shader.glsl", "../../../fragment_shader.glsl" );
				uniformSaturation = GL.GetUniformLocation( shaderProgram.id, "saturation" );
				uniformProj = GL.GetUniformLocation( shaderProgram.id, "proj" );
			};

			int counter = 0;
			window.RenderFrame += ( FrameEventArgs args ) => {
				GL.UseProgram( shaderProgram.id );

				GL.ClearColor( 1.0f, 0, 0, 0 );
				GL.Clear( ClearBufferMask.ColorBufferBit );

				GL.Uniform1( uniformSaturation, (float)Math.Sin(counter / 100.0f) + 1.0f);
				Matrix4 projMatrix = Matrix4.CreatePerspectiveFieldOfView( (float)Math.PI / 3.0f, (float)window.Size.X / (float)window.Size.Y, 0.1f, 2000.0f );
				GL.UniformMatrix4( uniformProj, false, ref projMatrix );

				float[] vertexData = {
					-250f, -250f, -450.0f * ( (float)Math.Sin( counter / 100.0f ) + 2.0f ), 1.0f, 0.0f, 0.0f, 
					250f, -250f, -450.0f * ( ( float ) Math.Sin( counter / 100.0f ) + 2.0f ), 0.0f, 1.0f, 0.0f, 
					0.0f, 250f, -450.0f * ( ( float ) Math.Sin( counter / 100.0f ) + 2.0f ), 0.0f, 0.0f, 1.0f
				}; 

				int vao = GL.GenVertexArray();
				int vertices = GL.GenBuffer();
				
				GL.BindVertexArray( vao );
				GL.BindBuffer( BufferTarget.ArrayBuffer, vertices );
				GL.BufferData( BufferTarget.ArrayBuffer, vertexData.Length * sizeof ( float ), vertexData, BufferUsageHint.StaticCopy );
				GL.EnableVertexAttribArray( 0 );
				GL.VertexAttribPointer( 0, 3, VertexAttribPointerType.Float, false, 24, 0 );
				GL.EnableVertexAttribArray( 1 );
				GL.VertexAttribPointer( 1, 3, VertexAttribPointerType.Float, false, 24, 12 );

				GL.DrawArrays( PrimitiveType.Triangles, 0, 3 );

				GL.BindBuffer( BufferTarget.ArrayBuffer, 0 );
				GL.BindVertexArray( 0 );
				GL.DeleteVertexArray( vao );
				GL.DeleteBuffer( vertices );



				window.SwapBuffers();

				++counter;
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