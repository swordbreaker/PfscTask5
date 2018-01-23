#version 400 core

in vec3 pos;
in vec4 v_color;

uniform mat4 p;
uniform mat4 m;
out vec4 f_color;

void main()
{
    vec4 f_pos = m * vec4(pos,1);
    gl_Position = p * f_pos;
    f_color = v_color;
}