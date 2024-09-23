using ImageProcessing.Models;
namespace ImageProcessing.Interfaces
{
    public interface IRenderer
    {
        bool IsFrameAvailable(int index, out byte[] stream);
        void Rendering();
    }
}
