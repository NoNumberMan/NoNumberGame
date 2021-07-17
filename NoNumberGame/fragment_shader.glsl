#version 330

in vec3 color_in;

out vec4 color_out;

uniform float saturation = 1.0;

void main() {
  color_out = vec4( saturation * color_in.r, saturation * color_in.g, saturation * color_in.b, 1.0 );
}