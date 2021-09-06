using System.Collections.Generic;

namespace NoNumberGame
{
	public class MeshModel
	{
		private readonly Dictionary<string, Mesh> _model = new Dictionary<string, Mesh>();


		public void AddMesh( string name, Mesh mesh ) {
			_model.Add( name, mesh );
		}

		public Mesh ExtractMesh( string name ) {
			_model.Remove( name, out Mesh mesh );
			return mesh;
		}

		public VaoModel ToVaoModel( Texture texture ) {
			VaoModel vaoModel = new VaoModel( texture );
			foreach ( string name in _model.Keys )
				vaoModel.AddVao( name, _model[name].GenerateVao() );

			return vaoModel;
		}
	}
}