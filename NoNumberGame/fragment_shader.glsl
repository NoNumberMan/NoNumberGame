#version 330

in vec3 frag_normal;
in vec3 frag_color;
in vec2 frag_texcoord;

uniform sampler2D tex;

out vec4 color_out;

void main() { //TODO add phong shading
	vec4 tex_color = texture(tex, frag_texcoord);

	color_out = vec4( vec3( frag_color.r, frag_color.g, frag_color.b ) * tex_color.rgb, 1.0 );
}