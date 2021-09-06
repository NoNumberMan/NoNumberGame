using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace NoNumberGame
{
	public class VaoModel : IDisposable
	{
		private struct Transformation
		{
			public Matrix4 mat4;
			public int uniform;
		}


		private readonly Dictionary<string, Vao> _model = new Dictionary<string, Vao>();
		private readonly Dictionary<string, Transformation?> _transformations = new Dictionary<string, Transformation?>();
		private readonly Texture                 _texture;

		public VaoModel( Texture texture ) {
			_texture = texture;
		}

		public void Dispose() {
			foreach ( Vao vao in _model.Values ) vao.Dispose();
			//GL.DeleteTexture( _texture.id ); //TODO _texture.Dispose();
		}



		public void AddVao( string name, Vao vao ) {
			_model.Add( name, vao );
			_transformations.Add( name, null );
		}

		public void SetTransformation( string name, int uniform, ref Matrix4 mat4 ) {
			_transformations[name] = new Transformation() { mat4 = mat4, uniform = uniform };
		}

		public void Draw( ShaderProgram shaderProgram ) {
			foreach ( string name in _model.Keys ) {
				if ( _transformations[name].HasValue ) {
					Matrix4 mat4 = _transformations[name]!.Value.mat4;
					GL.UniformMatrix4( _transformations[name]!.Value.uniform, false, ref mat4 );
				}
				
				_model[name].Draw( shaderProgram, _texture );
			}
		}

		public void Draw() {
			foreach ( string name in _model.Keys ) {
				if ( _transformations[name].HasValue ) {
					Matrix4 mat4 = _transformations[name]!.Value.mat4;
					GL.UniformMatrix4( _transformations[name]!.Value.uniform, false, ref mat4 );
				}

				_model[name].Draw( _texture );
			}
		}
	}
}