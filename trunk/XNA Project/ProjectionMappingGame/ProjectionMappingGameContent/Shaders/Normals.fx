// Include all shared types
#include "Includes.inc"

//
// Vertex Structure
//

   struct NormalsVertIn {
		float4	position	:	POSITION;
		float3	normal	:	NORMAL;
	};
	
	struct NormalsVertOut {
		float4  position		   :	POSITION;
		float3  N				   :	TEXCOORD0;
	};

//
// Vertex Shader
//

   NormalsVertOut NormalsVertexShaderFunction(NormalsVertIn input)
	{
		NormalsVertOut output;
		
		// Create world, view, and projection matrix
		float4x4 worldViewProjMatrix = mul(mul(worldMatrix, viewMatrix), projectionMatrix);

		// Calculate and store output for pixel shader vertex, normal and position
		output.N = input.normal;
		output.position = mul(input.position, worldViewProjMatrix);

		return output;
	}

//
// Pixel Shader
//

   float4 NormalsPixelShaderFunction(NormalsVertOut input) : COLOR0
   {
      // Normalize normal
      input.N = normalize(input.N);
      input.N = (input.N * 0.5f) + 0.5f;
		return float4(input.N.r, input.N.g, input.N.b, 1);
}

technique NormalsOnly
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 NormalsVertexShaderFunction();
        PixelShader = compile ps_2_0 NormalsPixelShaderFunction();
    }
}
