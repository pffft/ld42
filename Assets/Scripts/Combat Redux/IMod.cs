using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Combat_Redux
{
    public interface IMod
    {
        void OnUpdate();
        void OnDamageTaken()
    }
}
