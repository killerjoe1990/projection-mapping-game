// Space Matricies
shared float4x4 worldMatrix;
shared float4x4 viewMatrix;
shared float4x4 projectionMatrix;
shared float4x4 projectorViewMatrix;
shared float4x4 projectorProjectionMatrix;

// Camera Properties
shared float3 cameraPosition;

// Lighting/Color Properties
shared float3 lightPosition;
shared float3 lightDirection;
shared float4 lightAmbientColor;
shared float4 lightDiffuseColor;
shared float4 lightSpecularColor;

// Material Properties
shared float materialShine;
shared float4 materialEmissiveColor;
shared float4 materialAmbientColor;
shared float4 materialDiffuseColor;
shared float4 materialSpecularColor;