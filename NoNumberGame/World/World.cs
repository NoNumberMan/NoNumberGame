using System.Collections.Generic;

namespace NoNumberGame
{
	public class World
	{
		private readonly SortedList<uint, Chunk> _chunks = new SortedList<uint, Chunk>();
		private readonly SortedList<uint, Mesh> _meshes = new SortedList<uint, Mesh>();

		private World() { }

		private static uint GetChunkKey( int cx, int cy ) {
			uint k  = 0;
			uint ix = ( uint ) ( ( long ) int.MaxValue + ( long ) cx );
			uint iy = ( uint ) ( ( long ) int.MaxValue + ( long ) cy );
			k += ( ix & 65535 ) << 16;
			k += ( iy & 65535 );

			return k;
		}

		public void GenerateChunk( int cx, int cy ) {
			Chunk chunk = Chunk.Generate( cx, cy );
			_chunks.Add( GetChunkKey( cx, cy ), chunk );
		}

		public void DeleteChunk( int cx, int cy ) {
			uint key = GetChunkKey( cx, cy );
			_chunks.Remove( key );
			_meshes.Remove( key );
		}

		public MeshModel GenerateMeshModel() {
			MeshModel model = new MeshModel();
			foreach ( Chunk chunk in _chunks.Values ) {
				if ( chunk.IsDirty() ) {
					Mesh newMesh = chunk.GenerateMesh();
					_meshes.Add( GetChunkKey( chunk.X, chunk.Z ), newMesh );
					chunk.RemoveDirty();
				}

				if ( _meshes.TryGetValue( GetChunkKey( chunk.X, chunk.Z ), out Mesh mesh ) )
					model.AddMesh( $"mesh_{chunk.X}_{chunk.Z}", mesh );
			}

			return model;
		}

		public static World Generate( int startingX, int startingY /*TODO seed*/ ) {
			World world = new World();
			for ( int y = 0; y < 5; ++y ) {
				for ( int x = 0; x < 5; ++x ) {
					world.GenerateChunk( startingX + x - 2, startingY + y - 2 );
				}
			}

			return world;
		}
	}
}