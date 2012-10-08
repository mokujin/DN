#version 120

varying vec4 fcolor; 
varying vec2 ftexcoord;

void main(void) 
{ 
	gl_FragColor = vec4(ftexcoord, 1, 0.5); //texture(colorTexture, ftexcoord) * 0.1;
}