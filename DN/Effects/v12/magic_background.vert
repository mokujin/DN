#version 120

attribute vec2 vposition; 
attribute vec4 vcolor; 
attribute vec2 vtexcoord;

varying vec4 fcolor; 
varying vec2 ftexcoord;

void main(void) 
{
	fcolor = vcolor;
	ftexcoord = vtexcoord;
	gl_Position = vec4(vposition, 0, 1); 
}