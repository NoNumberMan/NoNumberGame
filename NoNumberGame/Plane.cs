using System;
using OpenTK.Mathematics;

namespace NoNumberGame
{
	public class Plane //TODO probably rename this to player
	{
		private Vector3 _pos;
		private Vector3 _vel;

		private Quaternion _ang;
		private Vector3 _angvel;

		private int _lifetime = 0;


		public Plane( float x, float y, float z, float pitch, float yaw, float roll ) {
			_pos    = new Vector3( x, y, z );
			_vel    = Vector3.Zero;
			_angvel = Vector3.Zero;
			_ang = Quaternion.FromEulerAngles( pitch, yaw, roll );
		}

		public Plane() : this( 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f ) { }



		internal Matrix4 GetMatrix() {
			return Matrix4.CreateFromQuaternion( _ang ) * Matrix4.CreateTranslation( _pos );
		}

		internal Matrix4 GetCamMatrix() {
			Vector3 offsetBack = -16.0f * ( Vector4.UnitZ * Matrix4.CreateFromQuaternion( _ang ) ).Xyz;
			Vector3 offsetUp   = 6.0f  * ( Vector4.UnitY * Matrix4.CreateFromQuaternion( _ang ) ).Xyz;
			return ( Matrix4.CreateRotationY( ( float ) Math.PI )
				* Matrix4.CreateFromQuaternion( _ang )
				* Matrix4.CreateTranslation( _pos )
				* Matrix4.CreateTranslation( ( offsetBack + offsetUp ) ) ).Inverted();
		}

		public void Update() {
			_pos += 0.024f * _vel;

			Vector3 ax = ( Vector4.UnitX * Matrix4.CreateFromQuaternion( _ang ) ).Xyz;
			Vector3 ay = ( Vector4.UnitY * Matrix4.CreateFromQuaternion( _ang ) ).Xyz;
			Vector3 az = ( Vector4.UnitZ * Matrix4.CreateFromQuaternion( _ang ) ).Xyz;
			_ang = Quaternion.FromAxisAngle( ax, 0.016f * _angvel.X ) * _ang;
			_ang = Quaternion.FromAxisAngle( ay, 0.016f * _angvel.Y ) * _ang;
			_ang = Quaternion.FromAxisAngle( az, 0.016f * _angvel.Z ) * _ang;

			float v = Vector3.Dot( _vel, ( Vector4.UnitZ * Matrix4.CreateFromQuaternion( _ang ) ).Xyz );
			float lift = 0.003f * v * v;

			_vel    += new Vector3( 0.0f, -0.2f, 0.0f );
			_vel    += lift * ( Vector4.UnitY * Matrix4.CreateFromQuaternion( _ang ) ).Xyz;
			_vel    *= 0.98f;
			_angvel *= 0.98f;

			_lifetime++;
		}

		public int GetLifetime() {
			return _lifetime;
		}

		public void SetPosition( float x, float y, float z ) {
			_pos = new Vector3( x, y, z );
		}

		public void SetVelocity( float dx, float dy, float dz ) {
			_vel = new Vector3( dx, dy, dz );
		}

		public void SetAngle( float pitch, float yaw, float roll ) {
			_ang = Quaternion.FromEulerAngles( pitch, yaw, roll );
		}

		public void SetAngularVelocity( float dpitch, float dyaw, float droll ) {
			_angvel = new Vector3( dpitch, dyaw, droll );
		}

		public void AccForwards( float mul ) {
			_vel += mul * ( Vector4.UnitZ * Matrix4.CreateFromQuaternion( _ang ) ).Xyz;
		}

		public void AccSideways( float mul ) {
			_vel += mul * ( Vector4.UnitX * Matrix4.CreateFromQuaternion( _ang ) ).Xyz;
		}

		public void AccUpwards( float mul ) {
			_vel += mul * ( Vector4.UnitY * Matrix4.CreateFromQuaternion( _ang ) ).Xyz;
		}

		public void AccAngle( float ddpitch, float ddyaw, float ddroll ) {
			_angvel += new Vector3( ddpitch, ddyaw, ddroll );
		}
	}
}