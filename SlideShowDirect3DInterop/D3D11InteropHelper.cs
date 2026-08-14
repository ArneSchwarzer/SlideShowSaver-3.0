using Vortice.Direct3D11;
using Vortice.Mathematics;

namespace SlideShowDirect3DInterop
{
    public static class D3D11InteropHelper
    {
        public static void ClearRenderTargetView(
            ID3D11DeviceContext renderContext,
            ID3D11RenderTargetView renderTargetView,
            float red,
            float green,
            float blue,
            float alpha)
        {
            Color4 clearColor;

            clearColor =
                new Color4(
                    red,
                    green,
                    blue,
                    alpha);

            renderContext.ClearRenderTargetView(
                renderTargetView,
                clearColor);
        }

        public static void UnbindRenderTarget(
            ID3D11DeviceContext renderContext)
        {
            renderContext.OMSetRenderTargets(
                (ID3D11RenderTargetView)null);
        }
    }
}