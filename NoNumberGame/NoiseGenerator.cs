using System;
using System.Linq;

namespace NoNumberGame
{
	public static class NoiseGenerator
	{
		private static uint Randomize( uint input ) {
			input += 1073676287;
			input ^= input << 13;
			input ^= input >> 17;
			input ^= input << 5;
			return input;
		}

		public static float[] GenerateNoise2D( uint xOffset, uint yOffset, int width, int height, int layers, int[] octaves, float[] amplitudes ) {
			double[] map  = new double[width * height];
			double   norm = ( double ) amplitudes.Sum();

			for ( int layer = 0; layer < layers; ++layer ) {
				int octave = octaves[layer];
				for ( uint y = 0; y < height / octave; ++y )
				for ( uint x = 0; x < width / octave; ++x ) {
					uint rx   = Randomize( 97 * x );
					uint ry   = Randomize( 89 * y );
					uint rx1  = Randomize( 97 * ( x + 1 ) );
					uint ry1  = Randomize( 89 * ( y + 1 ) );
					uint x0y0 = rx  * ry;
					uint x1y0 = rx1 * ry;
					uint x0y1 = rx  * ry1;
					uint x1y1 = rx1 * ry1;
					for ( uint xi = 0; xi < octave; ++xi )
					for ( uint yi = 0; yi < octave; ++yi ) {
						double d0 = 1.0 / ( xi + yi + 1.0 );
						double d1 = 1.0 / ( ( octave - xi ) + yi + 1.0 );
						double d2 = 1.0 / ( xi + ( octave - yi ) + 1.0 );
						double d3 = 1.0 / ( ( octave - xi ) + ( octave - yi ) + 1.0 );
						double h  = ( x0y0 * d0 * d0 + x1y0 * d1 * d1 + x0y1 * d2 * d2 + x1y1 * d3 * d3 ) / ( d0 * d0 + d1 * d1 + d2 * d2 + d3 * d3 );

						map[( octave * x + xi ) + width * ( octave * y + yi )] += ( ( double ) amplitudes[layer] / norm ) * h;
					}
				}
			}


			float[] result = new float[width * height];
			for ( int i = 0; i < width * height; ++i )
				result[i] = ( float ) ( map[i] / ( double ) uint.MaxValue );

			return result;
		}
	}
}