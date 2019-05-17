using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algo.Optim
{
    public abstract class SolutionSpace
    {
        readonly Random _random;
        SolutionInstance _bestEver = null;

        protected SolutionSpace()
        {
            _random = new Random();
        }

        protected SolutionSpace( int randomSeed )
        {
            _random = new Random( randomSeed );
        }

        protected void Initialize( IReadOnlyList<int> dimensions )
        {
            Dimensions = dimensions;
        }

        public IReadOnlyList<int> Dimensions { get; private set; }

        public double Cardinality => Dimensions.Aggregate( 1.0, ( acc, value ) => acc * value );

        public SolutionInstance CreateRandomInstance() {
            var r = new int[Dimensions.Count];

            for( var i = 0; i < r.Length; i++ )
            {
                r[i] = _random.Next( Dimensions[i] );
            }

            return DoCreateInstance( r );
        }

        public SolutionInstance ComputeBestRandom( int count )
        {
            SolutionInstance best = null;
            while ( count-- > 0)
            {
                var r = CreateRandomInstance();
                if( best == null || r.Cost < best.Cost ) best = r;
            }
            return best;
        }

        private SolutionInstance DoCreateInstance( IReadOnlyList<int> coordinates )
        {
            var s = CreateInstance( coordinates );
            if( _bestEver == null || s.Cost < _bestEver.Cost ) _bestEver = s;
            return s;
        }


        protected abstract SolutionInstance CreateInstance( IReadOnlyList<int> coordinates );
    }
}
