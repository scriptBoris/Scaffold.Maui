using ScaffoldLib.Maui.Args;
using ScaffoldLib.Maui.Containers.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldLib.Maui.Containers.Material;

public class AgentMaterial : CommonAgent
{
    public AgentMaterial(CreateAgentArgs args) : base(args)
    {
    }

    public override uint PushAnimationTime => 270;
    public override uint PopAnimationTime => 220;
    public override uint ReplaceAnimationTime => 220;
}
