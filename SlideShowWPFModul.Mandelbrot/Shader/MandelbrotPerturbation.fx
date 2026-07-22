// ============================================================
// MandelbrotPerturbation.fx
//
// Temporärer Platzhalter während der Entwicklung des
// Multipass-Proof-of-Concepts.
//
// Der produktive Perturbationsshader wird später auf Basis
// der funktionierenden Multipass-Architektur neu aufgebaut.
//
// Dieser Shader enthält bewusst:
// - keine Iterationsschleife
// - keine Orbitberechnung
// - keine Perturbationsmathematik
//
// Er reicht lediglich die Gradiententextur unverändert durch.
// ============================================================


// ------------------------------------------------------------
// Sampler
// ------------------------------------------------------------

sampler2D GradientSampler
    : register(s0);


// ------------------------------------------------------------
// Hauptshader
// ------------------------------------------------------------

float4 main(
    float2 uv : TEXCOORD) : COLOR
{
    return
        tex2D(
            GradientSampler,
            uv);
}