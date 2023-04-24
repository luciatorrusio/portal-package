#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

float4 _ColorTint;
// Textures
TEXTURE2D(_ColorMap); //RGB = albedo, A = alpha
SAMPLER(sampler_ColorMap);
float4 _ColorMap_ST; // This is automatically set by unity. Used in TRANSFORM_TEX to apply UV tiling

// World space normal of slice, anything along this direction from centre will be invisible
float3 _portalNormal;
// World space centre of slice
float3 _portalCenter;
// float _transitioning;


// This attributes struct recieves data about the mesh were currently rendering
// Data is automatically placed in fields according to their semantic
struct Attributes
{
    float3 position : POSITION; //Position in object space(local space) "POSITION" is the semantics and its the important part
    float2 uv : TEXCOORD0; // Material texture UVs
};

struct Interpolators
{
    // This value should contain the position in the clip space (which is similar to a position on screen)
    // when output from the vertex function. It will be transformed into pixel position of the current
    // fragment on the screen when read from the fragment function
    float4 positionCS : SV_POSITION;

    // The following variables will retain their values from the vertex stage, except the
    // resterizer will interpolate between vertices
    float2 uv : TEXCOORD0;  // Material texture UVs
    float3 positionWS : TEXCOORD2; 
};

// compute where vertex appears on screen
Interpolators Vertex(const Attributes input)
{
    Interpolators output;
    // These helper functions, found in URP/ShaderLib/ShaderVariablesFunctions.hlsl
    // transform object space values into world and clip space
    const VertexPositionInputs position_inputs = GetVertexPositionInputs(input.position);

    // Pass position and orientation data to the fragment function
    const float4 position_clip_space = position_inputs.positionCS;
    const float3 position_world_space = position_inputs.positionWS;
    output.positionCS = position_clip_space;
    output.positionWS = position_world_space;
    output.uv = TRANSFORM_TEX(input.uv, _ColorMap);
    return  output;
    
}

// The fragment function. This runs once per fragment, which you can think of as a pixel on the screen
// It must output the final color of this pixel
float4 Fragment(Interpolators input) : SV_TARGET
{
    float3 vectorToCenter = _portalCenter - input.positionWS;
    if( dot(vectorToCenter, _portalNormal) > 0.1)
        discard;
    
    float2 uv = input.uv;
    float4 colorSample = SAMPLE_TEXTURE2D(_ColorMap, sampler_ColorMap, uv);
    return  colorSample * _ColorTint;
}
