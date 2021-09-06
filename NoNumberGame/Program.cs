using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
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

		private static Camera? _camera;

		//private static VaoModel?   _terrainModel;
		private static VaoModel? _planeModel;
		private static Plane?    _plane;
		private static World?    _world;
		private static Texture? _terrainTexture;


		private static void Init() {
			Debug.WriteLine( "Hello World!" );

			CultureInfo.CurrentCulture = new CultureInfo( "en-US" );

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
			_plane  = new Plane();
			_world  = World.Generate( 0, 0 );

			_window.Load        += OnWindowLoad;
			_window.KeyDown     += OnWindowKeyDown;
			_window.Resize      += OnWindowResize;
			_window.MouseMove   += OnWindowMouseMove;
			_window.Closing     += OnWindowClose;
			_window.RenderFrame += Run;
		}

		private static void Run( FrameEventArgs args ) {
			ShaderProgram shaderProgram = ShaderLoader.LoadShaderProgram( "../../../vertex_shader.glsl", "../../../fragment_shader.glsl" );
			int           uniformProj   = GL.GetUniformLocation( shaderProgram.id, "proj" );
			int           uniformCam    = GL.GetUniformLocation( shaderProgram.id, "cam" );
			int           uniformObj    = GL.GetUniformLocation( shaderProgram.id, "obj" );
			int           uniformAni    = GL.GetUniformLocation( shaderProgram.id, "ani" );

			if ( _window!.KeyboardState.IsKeyDown( Keys.W ) ) _plane!.AccForwards( 0.2f );
			if ( _window!.KeyboardState.IsKeyDown( Keys.S ) ) _plane!.AccForwards( -0.2f );
			if ( _window!.KeyboardState.IsKeyDown( Keys.A ) ) _plane!.AccAngle( 0.0f, 0.01f, 0.0f );
			if ( _window!.KeyboardState.IsKeyDown( Keys.D ) ) _plane!.AccAngle( 0.0f, -0.01f, 0.0f );
			if ( _window!.KeyboardState.IsKeyDown( Keys.Right ) ) _plane!.AccAngle( 0.0f, 0.0f, 0.01f );
			if ( _window!.KeyboardState.IsKeyDown( Keys.Left ) ) _plane!.AccAngle( 0.0f, 0.0f, -0.01f );
			if ( _window!.KeyboardState.IsKeyDown( Keys.Up ) ) _plane!.AccAngle( 0.01f, 0.0f, 0.0f );
			if ( _window!.KeyboardState.IsKeyDown( Keys.Down ) ) _plane!.AccAngle( -0.01f, 0.0f, 0.0f );

			_plane!.Update();

			GL.UseProgram( shaderProgram.id );
			GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );

			Matrix4 projMatrix = Matrix4.CreatePerspectiveFieldOfView( ( float ) Math.PI / 3.0f, ( float ) _window!.Size.X / ( float ) _window!.Size.Y, 0.1f, 2000.0f );
			Matrix4 camMatrix  = _camera!.GetMatrix();
			//Matrix4 camMatrix     = _plane!.GetCamMatrix();
			Matrix4 planeMatrix   = _plane!.GetMatrix();
			Matrix4 terrainMatrix = Matrix4.Identity;
			Matrix4 propMatrix    = Matrix4.CreateRotationZ( 0.1f * _plane!.GetLifetime() );

			GL.UniformMatrix4( uniformProj, false, ref projMatrix );
			GL.UniformMatrix4( uniformCam, false, ref camMatrix );

			GL.UniformMatrix4( uniformObj, false, ref terrainMatrix );
			MeshModel model = _world!.GenerateMeshModel();
			VaoModel vao = model.ToVaoModel( _terrainTexture!.Value );
			vao.Draw();

			vao.Dispose();

			GL.UniformMatrix4( uniformObj, false, ref planeMatrix );
			//make that propeller go round and round baby! TODO wrap in an animation class later (:
			_planeModel!.SetTransformation( "PropellerShape1", uniformAni, ref propMatrix );
			_planeModel!.SetTransformation( "PropellerShape2", uniformAni, ref propMatrix );
			_planeModel!.SetTransformation( "PropellerShape3", uniformAni, ref propMatrix );
			_planeModel!.Draw();

			_window!.SwapBuffers();
			GL.DeleteProgram( shaderProgram.id );
		}

		private static void Terminate() {
			_window!.Dispose();
		}

		private static void OnWindowLoad() {
			GL.Enable( EnableCap.DepthTest );
			GL.ClearColor( 0.2f, 0.5f, 0.8f, 0 );

			_terrainTexture = TextureLoader.LoadTexture( "../../../white.png" );

			MeshModel plane        = ModelLoader.LoadModel( "../../../WW2-Plane-LowPoly.obj" );
			Texture   planeTexture = TextureLoader.LoadTexture( "../../../plane-diffuse.png" );
			_planeModel = plane.ToVaoModel( planeTexture );
		}

		private static void OnWindowKeyDown( KeyboardKeyEventArgs args ) {
			switch ( args.Key ) {
				case Keys.W:
					_camera!.MoveForwards( 0.8f );
					break;
				case Keys.S:
					_camera!.MoveForwards( -0.8f );
					break;
				case Keys.A:
					_camera!.MoveSideways( 0.8f );
					break;
				case Keys.D:
					_camera!.MoveSideways( -0.8f );
					break;
				case Keys.Space:
					_camera!.MoveUpwards( -0.8f );
					break;
				case Keys.LeftShift:
					_camera!.MoveUpwards( 0.8f );
					break;
				default: break;
			}
		}

		private static void OnWindowResize( ResizeEventArgs args ) {
			GL.Viewport( 0, 0, args.Width, args.Height );
		}

		private static void OnWindowMouseMove( MouseMoveEventArgs args ) {
			if ( _window!.IsMouseButtonDown( MouseButton.Middle ) ) {
				_camera!.Rotate( args.DeltaY / 100.0f, args.DeltaX / 100.0f, 0.0f );
			}
		}

		private static void OnWindowClose( CancelEventArgs args ) {
			_planeModel!.Dispose();
		}



		public static void Main( string[] args ) {
			Init();
			_window!.Run();
			Terminate();
		}
	}
}