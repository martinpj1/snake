using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
    //TODO eventually include objects as well
    public interface ICollisionCheck
    {
        Dictionary<Type, Action> CollissionEffects { get; set; }
        Dictionary<Type, Action> CollisionResponses { get; set; }
        Action NoCollisionAction { get; set; }
    }
    
}
