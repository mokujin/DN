#version 330 core

in vec4 fcolor; 
in vec2 ftexcoord;

uniform float time;

out vec4 color;

void main(void) 
{ 
	color = vec4(ftexcoord.x * (1.0-time), ftexcoord.y * time, time, 0.5); //texture(colorTexture, ftexcoord) * 0.1;
}