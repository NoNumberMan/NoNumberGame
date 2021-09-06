using System;
using System.IO;
using OpenTK.Graphics.OpenGL4;

namespace NoNumberGame
{
	public static class ShaderLoader
	{
		private static Shader LoadShader( string shaderLocation, ShaderType type ) {
			int shaderId = GL.CreateShader( type );
			GL.ShaderSource( shaderId, File.ReadAllText( shaderLocation ) );
			GL.CompileShader( shaderId );
			string infoLog = GL.GetShaderInfoLog( shaderId );
			if ( !string.IsNullOrEmpty( infoLog ) ) {
				throw new Exception( infoLog );
			}

			return new Shader( shaderId );
		}

		public static ShaderProgram LoadShaderProgram( string vertexShaderLocation, string fragmentShaderLocation ) {
			int shaderProgramId = GL.CreateProgram();

			Shader vertexShader   = LoadShader( vertexShaderLocation, ShaderType.VertexShader );
			Shader fragmentShader = LoadShader( fragmentShaderLocation, ShaderType.FragmentShader );

			GL.AttachShader( shaderProgramId, vertexShader.id );
			GL.AttachShader( shaderProgramId, fragmentShader.id );
			GL.LinkProgram( shaderProgramId );
			GL.DetachShader( shaderProgramId, vertexShader.id );
			GL.DetachShader( shaderProgramId, fragmentShader.id );
			GL.DeleteShader( vertexShader.id );
			GL.DeleteShader( fragmentShader.id );

			string infoLog = GL.GetProgramInfoLog( shaderProgramId );
			if ( !string.IsNullOrEmpty( infoLog ) ) {
				throw new Exception( infoLog );
			}

			return new ShaderProgram( shaderProgramId );
		}
	}
}