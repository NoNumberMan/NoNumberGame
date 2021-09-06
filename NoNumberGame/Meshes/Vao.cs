using System;
using OpenTK.Graphics.OpenGL4;

namespace NoNumberGame
{
	public readonly struct Vao : IDisposable
	{
		private readonly int _vaoId;
		private readonly int _indexId;
		private readonly int _size;

		public Vao( int vaoId, int indexId, int size ) {
			_vaoId = vaoId;
			_indexId = indexId;
			_size = size;
		}

		public void Dispose() {
			GL.DeleteBuffer( _indexId );
			GL.DeleteVertexArray( _vaoId );
		}

		public void Draw( ShaderProgram shaderProgram, Texture texture ) {
			GL.BindVertexArray( _vaoId );
			GL.BindBuffer( BufferTarget.ElementArrayBuffer, _indexId );
			GL.BindTexture( TextureTarget.Texture2D, texture.id );
			GL.UseProgram( shaderProgram.id );
			GL.DrawElements( PrimitiveType.Triangles, _size, DrawElementsType.UnsignedInt, 0 );
		}

		public void Draw( ShaderProgram shaderProgram ) {
			GL.BindVertexArray( _vaoId );
			GL.BindBuffer( BufferTarget.ElementArrayBuffer, _indexId );
			GL.UseProgram( shaderProgram.id );
			GL.DrawElements( PrimitiveType.Triangles, _size, DrawElementsType.UnsignedInt, 0 );
		}

		public void Draw( Texture texture ) {
			GL.BindVertexArray( _vaoId );
			GL.BindBuffer( BufferTarget.ElementArrayBuffer, _indexId );
			GL.BindTexture( TextureTarget.Texture2D, texture.id );
			GL.DrawElements( PrimitiveType.Triangles, _size, DrawElementsType.UnsignedInt, 0 );
		}

		public void Draw() {
			GL.BindVertexArray( _vaoId );
			GL.BindBuffer( BufferTarget.ElementArrayBuffer, _indexId );
			GL.DrawElements( PrimitiveType.Triangles, _size, DrawElementsType.UnsignedInt, 0 );
		}
	}
}