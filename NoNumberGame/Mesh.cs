using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL4;

namespace NoNumberGame
{
	public readonly struct Mesh
	{
		private readonly float[] vertexArray;
		private readonly float[] colorArray;
		private readonly float[] normalArray; //TODO
		private readonly int[]   indexArray;

		public Mesh( int vertices, int indices ) {
			vertexArray = new float[3 * vertices];
			colorArray  = new float[3 * vertices];
			normalArray = new float[3 * vertices];
			indexArray  = new int[indices];
		}

		public Mesh( float[] vertexArray, float[] colorArray, float[] normalArray, int[] indexArray ) {
			this.vertexArray = vertexArray;
			this.colorArray  = colorArray;
			this.normalArray = normalArray;
			this.indexArray  = indexArray;
		}


		public Vao GenerateVao() {
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

			GL.BindBuffer( BufferTarget.ArrayBuffer, 0 );
			GL.BindVertexArray( 0 );
			GL.DeleteBuffer( vertices );
			GL.DeleteBuffer( colors );

			return new Vao( vao, indices, indexArray.Length );
		}

		public void SetVertex( int index, float x, float y, float z ) {
			vertexArray[3 * index + 0] = x;
			vertexArray[3 * index + 1] = y;
			vertexArray[3 * index + 2] = z;
		}

		public void SetColor( int index, float r, float g, float b ) {
			colorArray[3 * index + 0] = r;
			colorArray[3 * index + 1] = g;
			colorArray[3 * index + 2] = b;
		}

		public void SetNormal( int index, float nx, float ny, float nz ) {
			normalArray[3 * index + 0] = nx;
			normalArray[3 * index + 1] = ny;
			normalArray[3 * index + 2] = nz;
		}

		public void SetVertexPoint( int index, float x, float y, float z, float r, float g, float b, float nx, float ny, float nz ) {
			vertexArray[3 * index + 0] = x;
			vertexArray[3 * index + 1] = y;
			vertexArray[3 * index + 2] = z;
			colorArray[3  * index + 0] = r;
			colorArray[3  * index + 1] = g;
			colorArray[3  * index + 2] = b;
			normalArray[3 * index + 0] = nx;
			normalArray[3 * index + 1] = ny;
			normalArray[3 * index + 2] = nz;
		}

		public void SetIndex( int index, int value ) {
			indexArray[index] = value;
		}
	}
}