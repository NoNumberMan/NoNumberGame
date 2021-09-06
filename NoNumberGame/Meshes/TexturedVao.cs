using System;

namespace NoNumberGame
{
	public struct TexturedVao : IDisposable
	{
		private readonly Vao _vao;
		private Texture? _texture;

		public TexturedVao( Vao vao ) {
			_vao = vao;
			_texture = null;
		}

		public void Dispose() {
			_vao.Dispose();
		}

		public void SetTexture( Texture texture ) {
			_texture = texture;
		}

		public void RemoveTexture() {
			_texture = null;
		}

		public void Draw( ShaderProgram shaderProgram ) {
			if ( _texture.HasValue ) _vao.Draw( shaderProgram, _texture.Value );
			else _vao.Draw( shaderProgram );
		}

		public void Draw() {
			if ( _texture.HasValue ) _vao.Draw( _texture.Value );
			else _vao.Draw();
		}
	}
}