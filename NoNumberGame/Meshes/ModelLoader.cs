using System;
using System.Collections.Generic;
using System.IO;
using OpenTK.Mathematics;

namespace NoNumberGame
{
	public static class ModelLoader
	{
		private struct FaceData
		{
			public Vector3i v0;
			public Vector3i v1;
			public Vector3i v2;

			public Vector3i Get( int index ) {
				return index switch {
					0 => v0,
					1 => v1,
					2 => v2,
					_ => new Vector3i( 0 )
				};
			}
		}

		private readonly struct MeshData
		{
			public readonly string         name;
			public readonly List<Vector3>  vertices;
			public readonly List<Vector3>  normals;
			public readonly List<Vector2>  texcoords;
			public readonly List<FaceData> faces;

			public MeshData( string name ) {
				this.name = name;
				vertices  = new List<Vector3>();
				normals   = new List<Vector3>();
				texcoords = new List<Vector2>();
				faces     = new List<FaceData>();
			}
		}

		public static MeshModel LoadModel( string modelLocation ) { //just for obj with negative faces
			string file = File.ReadAllText( modelLocation );

			List<MeshData> meshes = new List<MeshData>();
			LoadFromFile( file, meshes );

			MeshModel model = new MeshModel();
			FillModel( model, meshes );

			return model;
		}

		private static void FillModel( MeshModel model, IReadOnlyList<MeshData> meshes ) {
			for ( int i = 0; i < meshes.Count; ++i ) {
				int     faceCount     = meshes[i].faces.Count;
				float[] vertexArray   = new float[9 * faceCount];
				float[] normalArray   = new float[9 * faceCount];
				float[] colorArray    = new float[9 * faceCount];
				float[] texcoordArray = new float[6 * faceCount];
				int[]   indexArray    = new int[3   * faceCount];

				for ( int j = 0; j < faceCount; ++j ) {
					FaceData faceData = meshes[i].faces[j];

					vertexArray[9 * j + 0] = meshes[i].vertices[meshes[i].vertices.Count + faceData.v0.X].X;
					vertexArray[9 * j + 1] = meshes[i].vertices[meshes[i].vertices.Count + faceData.v0.X].Y;
					vertexArray[9 * j + 2] = meshes[i].vertices[meshes[i].vertices.Count + faceData.v0.X].Z;
					vertexArray[9 * j + 3] = meshes[i].vertices[meshes[i].vertices.Count + faceData.v1.X].X;
					vertexArray[9 * j + 4] = meshes[i].vertices[meshes[i].vertices.Count + faceData.v1.X].Y;
					vertexArray[9 * j + 5] = meshes[i].vertices[meshes[i].vertices.Count + faceData.v1.X].Z;
					vertexArray[9 * j + 6] = meshes[i].vertices[meshes[i].vertices.Count + faceData.v2.X].X;
					vertexArray[9 * j + 7] = meshes[i].vertices[meshes[i].vertices.Count + faceData.v2.X].Y;
					vertexArray[9 * j + 8] = meshes[i].vertices[meshes[i].vertices.Count + faceData.v2.X].Z;

					normalArray[9 * j + 0] = meshes[i].normals[meshes[i].normals.Count + faceData.v0.Y].X;
					normalArray[9 * j + 1] = meshes[i].normals[meshes[i].normals.Count + faceData.v0.Y].Y;
					normalArray[9 * j + 2] = meshes[i].normals[meshes[i].normals.Count + faceData.v0.Y].Z;
					normalArray[9 * j + 3] = meshes[i].normals[meshes[i].normals.Count + faceData.v1.Y].X;
					normalArray[9 * j + 4] = meshes[i].normals[meshes[i].normals.Count + faceData.v1.Y].Y;
					normalArray[9 * j + 5] = meshes[i].normals[meshes[i].normals.Count + faceData.v1.Y].Z;
					normalArray[9 * j + 6] = meshes[i].normals[meshes[i].normals.Count + faceData.v2.Y].X;
					normalArray[9 * j + 7] = meshes[i].normals[meshes[i].normals.Count + faceData.v2.Y].Y;
					normalArray[9 * j + 8] = meshes[i].normals[meshes[i].normals.Count + faceData.v2.Y].Z;

					colorArray[9 * j + 0] = 1.0f;
					colorArray[9 * j + 1] = 1.0f;
					colorArray[9 * j + 2] = 1.0f;
					colorArray[9 * j + 3] = 1.0f;
					colorArray[9 * j + 4] = 1.0f;
					colorArray[9 * j + 5] = 1.0f;
					colorArray[9 * j + 6] = 1.0f;
					colorArray[9 * j + 7] = 1.0f;
					colorArray[9 * j + 8] = 1.0f;

					texcoordArray[6 * j + 0] = meshes[i].texcoords[meshes[i].texcoords.Count + faceData.v0.Z].X;
					texcoordArray[6 * j + 1] = meshes[i].texcoords[meshes[i].texcoords.Count + faceData.v0.Z].Y;
					texcoordArray[6 * j + 2] = meshes[i].texcoords[meshes[i].texcoords.Count + faceData.v1.Z].X;
					texcoordArray[6 * j + 3] = meshes[i].texcoords[meshes[i].texcoords.Count + faceData.v1.Z].Y;
					texcoordArray[6 * j + 4] = meshes[i].texcoords[meshes[i].texcoords.Count + faceData.v2.Z].X;
					texcoordArray[6 * j + 5] = meshes[i].texcoords[meshes[i].texcoords.Count + faceData.v2.Z].Y;

					indexArray[3 * j + 0] = 3 * j + 0;
					indexArray[3 * j + 1] = 3 * j + 1;
					indexArray[3 * j + 2] = 3 * j + 2;
				}

				Mesh mesh = new Mesh( vertexArray, normalArray, colorArray, texcoordArray, indexArray );
				model.AddMesh( meshes[i].name, mesh );
			}
		}

		private static void LoadFromFile( string file, List<MeshData> meshes ) {
			string[] lines = file.Split( '\n' ); //split at newline

			for ( int i = 0; i < lines.Length; ++i ) {
				string[] data = lines[i].Trim().Split( ' ' );

				if ( data[0] == "g" || data[0] == "o" ) {
					meshes.Add( new MeshData( data[1] ) );
				}
				else if ( data[0] == "v" ) {
					Vector3 vertex = new Vector3( float.Parse( data[1] ), float.Parse( data[2] ), float.Parse( data[3] ) );
					meshes[^1].vertices.Add( vertex );
				}
				else if ( data[0] == "vn" ) {
					Vector3 normal = new Vector3( float.Parse( data[1] ), float.Parse( data[2] ), float.Parse( data[3] ) );
					meshes[^1].normals.Add( normal );
				}
				else if ( data[0] == "vt" ) {
					Vector2 texcoord = new Vector2( float.Parse( data[1] ), float.Parse( data[2] ) );
					meshes[^1].texcoords.Add( texcoord );
				}
				else if ( data[0] == "f" ) {
					string[] v0 = data[1].Split( '/' );
					string[] v1 = data[2].Split( '/' );
					string[] v2 = data[3].Split( '/' );

					FaceData faceData = new FaceData {
						v0 = new Vector3i( int.Parse( v0[0] ), int.Parse( v0[1] ), int.Parse( v0[2] ) ),
						v1 = new Vector3i( int.Parse( v1[0] ), int.Parse( v1[1] ), int.Parse( v1[2] ) ),
						v2 = new Vector3i( int.Parse( v2[0] ), int.Parse( v2[1] ), int.Parse( v2[2] ) )
					};

					meshes[^1].faces.Add( faceData );
				}
			}
		}
	}
}