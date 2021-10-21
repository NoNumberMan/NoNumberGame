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
			//xOffset = CW * CN;
			//zOffset = ...
			//width = CW, depth = ...
			
			
			double[] map  = new double[width * depth];
			double   norm = ( double ) amplitudes.Sum();

			for ( int layer = 0; layer < layers; ++layer ) {
				uint octave = octaves[layer];
				for ( int z = 0; z < depth / octave; ++z )
				for ( int x = 0; x < width / octave; ++x ) {
					uint ix = ( uint ) ( M2 + xOffset + x * octave ); 
					uint iz = ( uint ) ( M2 + zOffset + z * octave );

					uint rx   = Randomize( ix ); //ix+0 = c + 0 + 255 / ix+1 = c + 256 + 0
					uint rz   = Randomize( iz );
					uint rx1  = Randomize( ( ix + octave ) ); //ix+0 = c + 256
					uint rz1  = Randomize( ( iz + octave ) );
					uint x0y0 = rx  * rz;
					uint x1y0 = rx1 * rz;
					uint x0y1 = rx  * rz1;
					uint x1y1 = rx1 * rz1;

					if ( (x + 1) * octave == width ) { //special 'squeezed' interpolation
						for ( uint xi = 0; xi < octave; ++xi )
						for ( uint zi = 0; zi < octave; ++zi ) {
							double c = ( double )( octave * octave ) / ( ( octave - 1 ) * ( octave - 1 ) );

							double d0 = ( double ) ( ( octave - xi - 1 ) * ( octave - zi - 1 ) ) * c / ( octave * octave );
							double d1 = ( double ) ( xi * ( octave - zi - 1 ) ) * c / ( octave * octave );
							double d2 = ( double ) ( ( octave - xi -1 ) * zi ) * c / ( octave * octave );
							double d3 = ( double ) ( xi * zi ) * c / ( octave * octave );
							double h = ( x0y0 * d0 * d0 + x1y0 * d1 * d1 + x0y1 * d2 * d2 + x1y1 * d3 * d3 ) / ( d0 * d0 + d1 * d1 + d2 * d2 + d3 * d3 );

							map[( octave * x + xi ) + width * ( octave * z + zi )] += ( ( double ) amplitudes[layer] / norm ) * h;
							if ( octave == 8 ) {
								int a = 0;
							}
						}
					}
					else { //normal interpolation
						for ( uint xi = 0; xi < octave; ++xi )
						for ( uint zi = 0; zi < octave; ++zi ) {
							double c = ( double ) ( octave * octave ) / ( ( octave - 1 ) * ( octave - 1 ) );

							double d0 = ( double ) ( ( octave - xi - 1 ) * ( octave - zi - 1 ) )      * c / ( octave * octave );
							double d1 = ( double ) ( xi                  * ( octave - zi - 1 ) )      * c / ( octave * octave );
							double d2 = ( double ) ( ( octave - xi                       - 1 ) * zi ) * c / ( octave * octave );
							double d3 = ( double ) ( xi                                        * zi ) * c / ( octave * octave );

							double h = ( x0y0 * d0 * d0 + x1y0 * d1 * d1 + x0y1 * d2 * d2 + x1y1 * d3 * d3 ) / ( d0 * d0 + d1 * d1 + d2 * d2 + d3 * d3 );

							map[( octave * x + xi ) + width * ( octave * z + zi )] += ( ( double ) amplitudes[layer] / norm ) * h;
						}
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