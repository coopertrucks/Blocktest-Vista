using Blocktest.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blocktest.Code.Rendering;

public sealed class RenderableContainer
{
    public readonly HashSet<Renderable> Players = [];
    public readonly HashSet<Renderable> Objects = [];
    public readonly HashSet<Renderable> ForegroundBlocks = [];
    public readonly HashSet<Renderable> BackgroundBlocks = [];
    public readonly HashSet<Renderable> ForegroundParallax = [];
    public readonly HashSet<Renderable> BackgroundParallax = [];

    public RenderableContainer()
    {
        Debug.WriteLine(Players);
        Debug.WriteLine(Objects);
        Debug.WriteLine(ForegroundBlocks);
        Debug.WriteLine(BackgroundBlocks);
        Debug.WriteLine(ForegroundParallax);
        Debug.WriteLine(BackgroundParallax);
        Debug.WriteLine("RenderableContainer initialized!");
    }
}
