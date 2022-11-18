﻿#version 410

in vec3 positionIn;
in vec3 normalIn;
in vec2 texCoordIn;

uniform mat4 model;
uniform mat4 cameraView;
uniform mat4 projection;

out vec3 fragPos;
out vec3 normal;
out vec2 texCoord;

void main(void)
{
	vec4 fragPos4 = vec4(positionIn, 1.0) * model;
	fragPos = vec3(fragPos4);
	//gl_Position = fragPos4 * cameraView * projection;
	// texture
    gl_Position = vec4(positionIn, 1.0) * model * cameraView * projection;
    texCoord = texCoordIn;
	
	normal = normalIn * mat3(model);
}