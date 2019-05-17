using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algo.Optim
{
    public abstract class SolutionInstance
    {
        double _cost;

        protected SolutionInstance( SolutionSpace space, IReadOnlyList<int> coordinates)
        {
            _cost = -1.0;
            Space = space;
            Coordinates = coordinates;
            Debug.Assert( CheckCoordinates() );
        }

        public bool CheckCoordinates()
        {
            if( Coordinates.Count == Space.Dimensions.Count )
            {
                for( var i = 0; i < Coordinates.Count; i++ )
                {
                    if( Coordinates[i] < 0 || Coordinates[i] > Space.Dimensions[i] )
                    {
                        return false;
                    }
                }
                return true;
            }
            else return false;
        }

        public SolutionSpace Space { get; }

        public IReadOnlyList<int> Coordinates { get; }

        public double Cost {
            get
            {
                if( _cost >= 0.0 ) return _cost;
                return _cost = ComputeCost();
            }
        }

        protected abstract double ComputeCost();
    }
}
