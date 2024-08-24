using ImageProcessing.Models;
namespace ImageProcessing.Interfaces
{
    public interface IRenderer
    {
        void Render(byte[] frame);
    }
}
