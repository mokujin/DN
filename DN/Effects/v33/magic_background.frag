#version 330 core

in vec4 fcolor; 
in vec2 ftexcoord;

out vec4 color;

void main(void) 
{ 
	color = vec4(ftexcoord, 1, 0.5); //texture(colorTexture, ftexcoord) * 0.1;
}