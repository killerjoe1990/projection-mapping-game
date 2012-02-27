
static const float PI = 3.14159265f;
static const float HUE_STEP = (PI * 2.0f) / 6.0f;
static const float CHORD_LENGTH = (2.0f * 0.7f * sin((PI * 2.0f / 3.0f) / 2.0f)) - 0.2f;
static float3 HUES[7] = {
   float3(1.0f, 0.0f, 0.0f),
   float3(1.0f, 1.0f, 0.0f),
   float3(0.0f, 1.0f, 0.0f),
   float3(0.0f, 1.0f, 1.0f),
   float3(0.0f, 0.0f, 1.0f),
   float3(1.0f, 0.0f, 1.0f),
   float3(1.0f, 0.0f, 0.0f)
};

float2 m_HuePoint;
float2 m_WhitePoint;
float2 m_BlackPoint;
float m_Hue;

//
// Utility 
//

float crossZ(float2 l, float2 r)
{
   return (r.y * l.x) - (r.x * l.y);
}

float getHue(float2 pos)
{
   float2 unitX = float2(1.0f, 0.0f);
   float2 posNorm = normalize(pos);
   float dp = clamp(dot(posNorm, unitX), -1, 1);
   float theta = acos(dp);
   float cp = crossZ(posNorm, unitX);
   if (cp < 0)
      theta = ((2 * PI) - theta);
   return theta / HUE_STEP;
}

float3 getColorFromHue(float h)
{
   int hue = floor(h);
   float t = h - hue;
   return HUES[hue] * (1 - t) + HUES[hue + 1] * t;
}

//
// Vertex Shader
//

struct VertexShaderInput
{
    float4 Position : POSITION0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

VertexShaderOutput ColorPickerVertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
    output.Position = input.Position;
    output.TexCoord = input.Position;
    return output;
}

//
// Pixel Shaders
//

float4 HueWheelPixelShaderFunction(VertexShaderOutput input) : COLOR0
{
   float4 color = float4(0, 0, 0, 0);
   float len = length(input.TexCoord);
   if (len > 0.69f && len <= 0.91f)
   {
      color.rgb = getColorFromHue(getHue(input.TexCoord));
      color.a = 1.0f;

      if (len <= 0.7)
         color *= (1.0f - ((0.7f - len) * 100.0f));
      else if (len > 0.9f)
         color *= ((0.91 - len) * 100.0f);
   }
   return color;
}

float4 SaturationTrianglePixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float4 color = float4(0, 0, 0, 1.0f);
    float3 hueColor = getColorFromHue(m_Hue);
    
    float hue_t = length(m_HuePoint - input.TexCoord) / CHORD_LENGTH;
    hue_t -= 0.1f;
    hue_t = clamp(hue_t, 0, 1);
    
    float white_t = length(m_WhitePoint - input.TexCoord) / CHORD_LENGTH;
    white_t -= 0.1f;
    white_t = clamp(white_t, 0, 1);
    
    color.rgb = (hueColor * (1.0f - hue_t));
    color.rgb += float3(1, 1, 1) * (1.0f - white_t);
    return color;
}

//
// Techniques
//

technique DrawHueWheel
{
   pass Pass1
   {
      BlendOp = Add;
      SrcBlend = One;
      DestBlend = Zero;
      ZEnable = false;
      ZWriteEnable = false;
      ZFunc = LessEqual;

      VertexShader = compile vs_3_0 ColorPickerVertexShaderFunction();
      PixelShader = compile ps_3_0 HueWheelPixelShaderFunction();
   }
}

technique DrawSaturationTriangle
{
   pass Pass1
   {
		BlendOp = Add;
		SrcBlend = One;
		DestBlend = Zero;
		ZEnable = false;
		ZWriteEnable = false;
		ZFunc = LessEqual;

      VertexShader = compile vs_3_0 ColorPickerVertexShaderFunction();
      PixelShader = compile ps_3_0 SaturationTrianglePixelShaderFunction();
   }
}
