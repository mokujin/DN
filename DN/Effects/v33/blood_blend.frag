#version 330 core

uniform sampler2D tex1;
uniform sampler2D tex2;

in vec4 fcolor; 
in vec2 ftexcoord;

out vec4 color;

void main(void) 
{ 
	vec4 tex2color = texture(tex2, ftexcoord);
	vec4 tex1color = texture(tex1, ftexcoord);
	
	if(tex2color.r > 0 && tex1color.r > 0)
		color = vec4(tex2color.rgb, tex1color.a);
	else
		color = tex1color;
	 
	//color = vec4(ftexcoord, 1, 0.5); //texture(colorTexture, ftexcoord) * 0.1;
}