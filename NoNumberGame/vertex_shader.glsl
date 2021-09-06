#version 330

layout(location=0) in vec3 vPosition;
layout(location=1) in vec3 vNormals;
layout(location=2) in vec3 vColors;
layout(location=3) in vec2 vTexcoords;

out vec3 frag_normal;
out vec3 frag_color;
out vec2 frag_texcoord;

uniform mat4 proj	= mat4(1);
uniform mat4 cam	= mat4(1);
uniform mat4 obj	= mat4(1);
uniform mat4 ani	= mat4(1);

void main() {
  frag_normal	= ( cam * obj * ani * vec4( vNormals, 1.0 ) ).xyz;
  frag_color	= vColors;
  frag_texcoord = vTexcoords;
  gl_Position	= proj * cam * obj * ani * vec4( vPosition, 1.0 );
} 