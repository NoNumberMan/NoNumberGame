using System;
using OpenTK.Graphics.OpenGL4;

namespace NoNumberGame
{
	public readonly struct Vao : IDisposable
	{
		public readonly int vaoId;
		public readonly int indexId;
		public readonly int size;

		public Vao( int vaoId, int indexId, int size ) {
			this.vaoId = vaoId;
			this.indexId = indexId;
			this.size = size;
		}

		public void Dispose() {
			GL.DeleteBuffer( indexId );
			GL.DeleteVertexArray( vaoId );
		}
	}
}