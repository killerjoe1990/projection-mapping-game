// Include all shared types
#include "Includes.inc"

//
// Vertex Structure
//

   struct PhongVertIn {
		float4	position	:	POSITION;
		float3	normal	:	NORMAL;
		float2   texCoord	:	TEXCOORD0;
	};
	
	struct PhongVertOut {
		float4  position		   :	POSITION;
		float2  texCoord	      :  TEXCOORD0;
		float4  pos_w			   :	TEXCOORD1;
		float3  N				   :	TEXCOORD2;
	};

//
// Vertex Shader
//

   PhongVertOut PhongVertexShaderFunction(PhongVertIn input)
	{
		PhongVertOut output;
		
		// Create world, view, and projection matrix
		float4x4 worldViewProjMatrix = mul(mul(worldMatrix, viewMatrix), projectionMatrix);

		// Calculate and store output for pixel shader vertex, normal and position
		output.pos_w = mul(input.position, worldMatrix);
		output.N = mul(input.normal, worldMatrix);
		output.position = mul(input.position, worldViewProjMatrix);
		output.texCoord = input.texCoord;

		return output;
	}

   float4 PhongPixelShaderFunction(PhongVertOut input) : COLOR0
   {
      // Normalize normal
      input.N = normalize(input.N);

      // Compute light direction
      float3 L = normalize(lightPosition - input.pos_w);

		// Compute diffuse contribution
      float diffuseIntensity = max(dot(L, input.N), 0.0f);
		float4 diffuseColor = materialDiffuseColor * lightDiffuseColor * diffuseIntensity;

		return materialEmissiveColor + diffuseColor;
}

technique PhongDiffuseOnly
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 PhongVertexShaderFunction();
        PixelShader = compile ps_2_0 PhongPixelShaderFunction();
    }
}
