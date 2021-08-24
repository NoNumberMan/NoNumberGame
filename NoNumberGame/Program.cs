using System;
using System.Diagnostics;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace NoNumberGame
{
	public static class Program
	{
		private static GameWindow? _window;
		private static Camera?     _camera;
		private static Vao?        _terrainVao;


		private static void Init() {
			Debug.WriteLine( "Hello World!" );

			GameWindowSettings   gws = GameWindowSettings.Default;
			NativeWindowSettings nws = NativeWindowSettings.Default;
			gws.IsMultiThreaded = false;
			gws.RenderFrequency = 60;
			gws.UpdateFrequency = 60;

			nws.APIVersion = Version.Parse( "4.1.0" );
			nws.Size       = new Vector2i( 1280, 720 );
			nws.Title      = "NoNumberGame";

			_window = new GameWindow( gws, nws );
			_camera = new Camera();


			_window.Load        += OnWindowLoad;
			_window.KeyDown     += OnWindowKeyDown;
			_window.Resize      += OnWindowResize;
			_window.MouseMove   += OnWindowMouseMove;
			_window.RenderFrame += Run;
		}

		private static void Run( FrameEventArgs args ) {
			ShaderProgram shaderProgram = ShaderLoader.LoadShaderProgram( "../../../vertex_shader.glsl", "../../../fragment_shader.glsl" );
			int           uniformProj   = GL.GetUniformLocation( shaderProgram.id, "proj" );


			GL.UseProgram( shaderProgram.id );
			GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );

			Matrix4 projMatrix = _camera!.GetMatrix() * Matrix4.CreatePerspectiveFieldOfView( ( float ) Math.PI / 3.0f, ( float ) _window!.Size.X / ( float ) _window!.Size.Y, 0.1f, 2000.0f );
			GL.UniformMatrix4( uniformProj, false, ref projMatrix );


			GL.BindVertexArray( _terrainVao!.Value.vaoId );
			GL.BindBuffer( BufferTarget.ElementArrayBuffer, _terrainVao!.Value.indexId );
			GL.DrawElements( PrimitiveType.Triangles, _terrainVao!.Value.size, DrawElementsType.UnsignedInt, 0 );


			_window!.SwapBuffers();
			GL.DeleteProgram(shaderProgram.id);
		}

		private static void Terminate() {
			_window!.Dispose();
		}

		private static void OnWindowLoad() {
			GL.Enable( EnableCap.DepthTest );
			GL.ClearColor( 0.2f, 0.5f, 0.8f, 0 );

			Mesh terrain = TerrainGenerator.GenerateTerrain();
			_terrainVao = terrain.GenerateVao();
		}

		private static void OnWindowKeyDown( KeyboardKeyEventArgs args ) {
			switch ( args.Key ) {
				case Keys.W:
					_camera!.MoveForwards( 0.2f );
					break;
				case Keys.S:
					_camera!.MoveForwards( -0.2f );
					break;
				case Keys.A:
					_camera!.MoveSideways( 0.2f );
					break;
				case Keys.D:
					_camera!.MoveSideways( -0.2f );
					break;
				case Keys.Space:
					_camera!.MoveUpwards( -0.2f );
					break;
				case Keys.LeftShift:
					_camera!.MoveUpwards( 0.2f );
					break;
				default: break;
			}
		}

		private static void OnWindowResize( ResizeEventArgs args ) {
			GL.Viewport( 0, 0, args.Width, args.Height );
		}

		private static void OnWindowMouseMove( MouseMoveEventArgs args ) {
			if ( _window!.IsMouseButtonDown( MouseButton.Middle ) ) {
				_camera!.Rotate( args.DeltaY / 100.0f, -args.DeltaX / 100.0f, 0.0f );
			}
		}



		public static void Main( string[] args ) {
			Init();
			_window!.Run();
			Terminate();
		}
	}
}