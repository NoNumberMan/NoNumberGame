using OpenTK.Mathematics;

namespace NoNumberGame
{
	public class Camera
	{
		private float _x;
		private float _y;
		private float _z;

		private float _pitch;
		private float _yaw;
		private float _roll;

		public Camera( float x, float y, float z, float pitch, float yaw, float roll ) {
			_x     = x;
			_y     = y;
			_z     = z;
			_pitch = pitch;
			_yaw   = yaw;
			_roll  = roll;
		}

		public Camera() : this( 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f ) { }



		internal Matrix4 GetMatrix() {
			return Matrix4.CreateTranslation( _x, _y, _z ) * Matrix4.CreateFromQuaternion( Quaternion.FromEulerAngles( _pitch, _yaw, _roll ) );
		}

		public void Translate( float dx, float dy, float dz ) {
			_x += dx;
			_y += dy;
			_z += dz;
		}

		public void SetPosition( float x, float y, float z ) {
			_x = x;
			_y = y;
			_z = z;
		}

		public void MoveForwards( float distance ) {
			( float x, float y, float z, _ ) =  Vector4.UnitZ * Matrix4.CreateFromQuaternion( Quaternion.FromEulerAngles( _pitch, _yaw, _roll ) ).Inverted();
			_x                               += distance      * x;
			_y                               += distance      * y;
			_z                               += distance      * z;
		}

		public void MoveSideways( float distance ) {
			( float x, float y, float z, _ ) =  Vector4.UnitX * Matrix4.CreateFromQuaternion( Quaternion.FromEulerAngles( _pitch, _yaw, _roll ) ).Inverted();
			_x                               += distance      * x;
			_y                               += distance      * y;
			_z                               += distance      * z;
		}

		public void MoveUpwards( float distance ) {
			( float x, float y, float z, _ ) =  Vector4.UnitY * Matrix4.CreateFromQuaternion( Quaternion.FromEulerAngles( _pitch, _yaw, _roll ) ).Inverted();
			_x                               += distance      * x;
			_y                               += distance      * y;
			_z                               += distance      * z;
		}

		public void Rotate( float dpitch, float dyaw, float droll ) {
			_pitch += dpitch;
			_yaw   += dyaw;
			_roll  += droll;
		}

		public void SetRotation( float pitch, float yaw, float roll ) {
			_pitch = pitch;
			_yaw   = yaw;
			_roll  = roll;
		}
	}
}