using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL4;

namespace NoNumberGame
{
	public readonly struct Mesh
	{
		private readonly float[] _vertexArray;
		private readonly float[] _colorArray;
		private readonly float[] _normalArray; //TODO
		private readonly int[]   _indexArray;

		public Mesh( int vertices, int indices ) {
			_vertexArray = new float[3 * vertices];
			_colorArray  = new float[3 * vertices];
			_normalArray = new float[3 * vertices];
			_indexArray  = new int[indices];
		}

		public Mesh( float[] vertexArray, float[] colorArray, float[] normalArray, int[] indexArray ) {
			_vertexArray = vertexArray;
			_colorArray  = colorArray;
			_normalArray = normalArray;
			_indexArray  = indexArray;
		}


		public Vao GenerateVao() {
			int vao      = GL.GenVertexArray();
			int vertices = GL.GenBuffer();
			int colors   = GL.GenBuffer();
			int indices  = GL.GenBuffer();

			GL.BindVertexArray( vao );
			GL.BindBuffer( BufferTarget.ArrayBuffer, vertices );
			GL.BufferData( BufferTarget.ArrayBuffer, _vertexArray.Length * sizeof ( float ), _vertexArray, BufferUsageHint.StaticCopy );
			GL.EnableVertexAttribArray( 0 );
			GL.VertexAttribPointer( 0, 3, VertexAttribPointerType.Float, false, 0, 0 );

			GL.BindBuffer( BufferTarget.ArrayBuffer, colors );
			GL.BufferData( BufferTarget.ArrayBuffer, _colorArray.Length * sizeof ( float ), _colorArray, BufferUsageHint.StaticCopy );
			GL.EnableVertexAttribArray( 1 );
			GL.VertexAttribPointer( 1, 3, VertexAttribPointerType.Float, false, 0, 0 );

			GL.BindBuffer( BufferTarget.ElementArrayBuffer, indices );
			GL.BufferData( BufferTarget.ElementArrayBuffer, _indexArray.Length * sizeof ( int ), _indexArray, BufferUsageHint.StaticCopy );

			GL.BindBuffer( BufferTarget.ArrayBuffer, 0 );
			GL.BindBuffer( BufferTarget.ElementArrayBuffer, 0 );
			GL.BindVertexArray( 0 );
			GL.DeleteBuffer( vertices );
			GL.DeleteBuffer( colors );

			return new Vao( vao, indices, _indexArray.Length );
		}

		public void SetVertex( int index, float x, float y, float z ) {
			_vertexArray[3 * index + 0] = x;
			_vertexArray[3 * index + 1] = y;
			_vertexArray[3 * index + 2] = z;
		}

		public void SetColor( int index, float r, float g, float b ) {
			_colorArray[3 * index + 0] = r;
			_colorArray[3 * index + 1] = g;
			_colorArray[3 * index + 2] = b;
		}

		public void SetNormal( int index, float nx, float ny, float nz ) {
			_normalArray[3 * index + 0] = nx;
			_normalArray[3 * index + 1] = ny;
			_normalArray[3 * index + 2] = nz;
		}

		public void SetVertexPoint( int index, float x, float y, float z, float r, float g, float b, float nx, float ny, float nz ) {
			_vertexArray[3 * index + 0] = x;
			_vertexArray[3 * index + 1] = y;
			_vertexArray[3 * index + 2] = z;
			_colorArray[3  * index + 0] = r;
			_colorArray[3  * index + 1] = g;
			_colorArray[3  * index + 2] = b;
			_normalArray[3 * index + 0] = nx;
			_normalArray[3 * index + 1] = ny;
			_normalArray[3 * index + 2] = nz;
		}

		public void SetIndex( int index, int value ) {
			_indexArray[index] = value;
		}
	}
}