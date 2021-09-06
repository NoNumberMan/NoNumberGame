using System;
using System.Linq;

namespace NoNumberGame
{
	public static class NoiseGenerator
	{
		private const int M2 = 2 ^ 30;


		private static uint Randomize( uint input ) {
			input += 1073676287;
			input ^= input << 13;
			input ^= input >> 17;
			input ^= input << 5;
			return input;
		}

		public static float[] GenerateNoise2D( int xOffset, int zOffset, uint width, uint depth, uint layers, uint[] octaves, float[] amplitudes ) {
			double[] map  = new double[width * depth];
			double   norm = ( double ) amplitudes.Sum();

			for ( int layer = 0; layer < layers; ++layer ) {
				uint octave = octaves[layer];
				for ( int z = 0; z < depth / octave; ++z )
				for ( int x = 0; x < width / octave; ++x ) {
					uint ix = ( uint ) ( M2 + xOffset + x * octave ); //ix+0 = c + [0, 8, 16, ... 256-8] + 0 -> c + 256 + 0 -> 256
					uint iz = ( uint ) ( M2 + zOffset + z * octave ); //ix+1 = c + [0, 8, 16, ...] + 255 -> c + 0 + 256 -> 256

					uint rx   = Randomize( ix );
					uint rz   = Randomize( iz );
					uint rx1  = Randomize( ( ix + octave ) );
					uint rz1  = Randomize( ( iz + octave ) );
					uint x0y0 = rx  * rz;
					uint x1y0 = rx1 * rz;
					uint x0y1 = rx  * rz1;
					uint x1y1 = rx1 * rz1;
					for ( uint xi = 0; xi < octave; ++xi )
					for ( uint zi = 0; zi < octave; ++zi ) {
						double d0 = 1.0 / ( xi + zi + 1.0 );
						double d1 = 1.0 / ( ( octave - xi ) + zi + 1.0 );
						double d2 = 1.0 / ( xi + ( octave - zi ) + 1.0 );
						double d3 = 1.0 / ( ( octave - xi ) + ( octave - zi ) + 1.0 );
						double h  = ( x0y0 * d0 * d0 + x1y0 * d1 * d1 + x0y1 * d2 * d2 + x1y1 * d3 * d3 ) / ( d0 * d0 + d1 * d1 + d2 * d2 + d3 * d3 );

						map[( octave * x + xi ) + width * ( octave * z + zi )] += ( ( double ) amplitudes[layer] / norm ) * h;
					}
				}
			}


			float[] result = new float[width * depth];
			for ( int i = 0; i < width * depth; ++i )
				result[i] = ( float ) ( map[i] / ( double ) uint.MaxValue );

			return result;
		}
	}
}