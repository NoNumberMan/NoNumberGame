namespace NoNumberGame
{
	public readonly struct Texture
	{
		public static readonly Texture Zero = new Texture( 0 );

		public readonly int id;

		public Texture( int id ) {
			this.id = id;
		}
	}
}