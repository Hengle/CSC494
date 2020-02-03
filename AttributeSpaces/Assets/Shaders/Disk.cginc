// Pcx - Point cloud importer & renderer for Unity
// https://github.com/keijiro/Pcx

#include "UnityCG.cginc"
#include "Common.cginc"

// Uniforms
half _PointSize;
float4x4 _Transform;

StructuredBuffer<float4> _PositionBuffer;
StructuredBuffer<float4> _ColorBuffer;

// Vertex input attributes
struct Attributes
{
    uint vertexID : SV_VertexID;
};

// Fragment varyings
struct Varyings
{
    float4 position : SV_POSITION;
    float4 color : COLOR;
    float scale : PSIZE;
    UNITY_FOG_COORDS(0)
};

// Vertex phase
Varyings Vertex(Attributes input)
{
    // Retrieve vertex attributes.
    float4 pt = _PositionBuffer[input.vertexID];
    float4 pos = mul(_Transform, float4(pt.xyz, 1));
    float4 col = _ColorBuffer[input.vertexID];
    float scale = pt.w;

    // Color space convertion & applying tint
    // col *= _Tint.rgb * 2;

    // Set vertex output.
    Varyings o;
    o.position = UnityObjectToClipPos(pos);
    o.color = col;
    o.scale = pt.w;
    UNITY_TRANSFER_FOG(o, o.position);
    return o;
}

// Geometry phase
[maxvertexcount(36)]
void Geometry(point Varyings input[1], inout TriangleStream<Varyings> outStream)
{
    float4 origin = input[0].position;
    float2 extent = abs(UNITY_MATRIX_P._11_22 * _PointSize * input[0].scale);

    // Copy the basic information.
    Varyings o = input[0];

    // Determine the number of slices based on the radius of the
    // point on the screen.
    float radius = extent.y / origin.w * _ScreenParams.y;
    uint slices = min((radius + 1) / 5, 4) + 2;

    // Slightly enlarge quad points to compensate area reduction.
    // Hopefully this line would be complied without branch.
    if (slices == 2) extent *= 1.2;

    // Top vertex
    o.position.y = origin.y + extent.y;
    o.position.xzw = origin.xzw;
    outStream.Append(o);

    UNITY_LOOP for (uint i = 1; i < slices; i++)
    {
        float sn, cs;
        sincos(UNITY_PI / slices * i, sn, cs);

        // Right side vertex
        o.position.xy = origin.xy + extent * float2(sn, cs);
        outStream.Append(o);

        // Left side vertex
        o.position.x = origin.x - extent.x * sn;
        outStream.Append(o);
    }

    // Bottom vertex
    o.position.x = origin.x;
    o.position.y = origin.y - extent.y;
    outStream.Append(o);

    outStream.RestartStrip();
}

half4 Fragment(Varyings input) : SV_Target
{
    float4 c = input.color;
    UNITY_APPLY_FOG(input.fogCoord, c);
    return c;
}

