using System;

namespace NoNumberGame
{
	public static class TerrainGenerator
	{
		private static readonly float[][] _colorMap = new float[][] {
			new float[] { 0.0f, 1.0f, 0.0f },    //grass
			new float[] { 0.5f, 0.5f, 0.1f },    //sand
			new float[] { 0.4f, 0.4f, 0.4f },    //rock
			new float[] { 0.95f, 0.95f, 0.95f }, //snow
		};



		public static MeshModel GenerateTerrain( /*TODO chunk_x, chunk_y*/ ) {
			float[] heightmap = NoiseGenerator.GenerateNoise2D( 0, 0, 256, 256, 4,
				new uint[] { 2, 8, 32, 128 }, new[] { 0.04f, 0.1f, 1.0f, 0.4f } );

			float[] vertexArray = new float[256 * 256 * 3];
			float[] normalArray = new float[256 * 256 * 3]; //TODO fill this
			float[] colorArray  = new float[256 * 256 * 3];
			float[] texcoordArray = new float[256 * 256 * 3];
			int[]   indexArray  = new int[255 * 255 * 6];

			Random rand = new Random();
			for ( int z = 0; z < 256; ++z )
			for ( int x = 0; x < 256; ++x ) {
				float y = heightmap[x + 256 * z];

				vertexArray[3 * ( x + 256 * z ) + 0] = x - 0.0f;
				vertexArray[3 * ( x + 256 * z ) + 1] = y * 64.0f;
				vertexArray[3 * ( x + 256 * z ) + 2] = z - 0.0f;

				int   colorIndex = 0;
				float yc         = y + 0.1f * ( float ) rand.NextDouble();

				if ( yc      < 0.3f ) colorIndex = 1;
				else if ( yc < 0.5f ) colorIndex = 0;
				else if ( yc < 0.7f ) colorIndex = 2;
				else colorIndex                  = 3;
				colorArray[3 * ( x + 256 * z ) + 0] = _colorMap[colorIndex][0];
				colorArray[3 * ( x + 256 * z ) + 1] = _colorMap[colorIndex][1];
				colorArray[3 * ( x + 256 * z ) + 2] = _colorMap[colorIndex][2];

				if ( x < 255 && z < 255 ) {
					indexArray[6 * ( x + 255 * z ) + 0] = x         + 256 * z;
					indexArray[6 * ( x + 255 * z ) + 1] = x         + 256 * ( z + 1 );
					indexArray[6 * ( x + 255 * z ) + 2] = ( x + 1 ) + 256 * z;
					indexArray[6 * ( x + 255 * z ) + 3] = ( x + 1 ) + 256 * z;
					indexArray[6 * ( x + 255 * z ) + 4] = x         + 256 * ( z + 1 );
					indexArray[6 * ( x + 255 * z ) + 5] = ( x + 1 ) + 256 * ( z + 1 );
				}
			}

			for ( int i = 0; i < 255 * 255 * 2; ++i ) {
				float dc = 0.90f + 0.2f * ( float ) rand.NextDouble();
				colorArray[3 * indexArray[3 * i + 0] + 0] *= dc;
				colorArray[3 * indexArray[3 * i + 0] + 1] *= dc;
				colorArray[3 * indexArray[3 * i + 0] + 2] *= dc;
				colorArray[3 * indexArray[3 * i + 1] + 0] *= dc;
				colorArray[3 * indexArray[3 * i + 1] + 1] *= dc;
				colorArray[3 * indexArray[3 * i + 1] + 2] *= dc;
				colorArray[3 * indexArray[3 * i + 2] + 0] *= dc;
				colorArray[3 * indexArray[3 * i + 2] + 1] *= dc;
				colorArray[3 * indexArray[3 * i + 2] + 2] *= dc;
			}

			MeshModel model = new MeshModel();
			model.AddMesh( "terrain", new Mesh( vertexArray, normalArray, colorArray, texcoordArray, indexArray ) );
			return model;
		}
	}
}