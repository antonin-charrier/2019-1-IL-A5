using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Algo
{
    public class RecoContext
    {
        public IReadOnlyList<User> Users { get; private set; }
        public IReadOnlyList<Movie> Movies { get; private set; }
        public int RatingCount { get; private set; }

        public double Distance( User u1, User u2 )
        {
            bool atLeastOne = false;
            int sum2 = 0;
            foreach( var movieR1 in u1.Ratings )
            {
                if( u2.Ratings.TryGetValue( movieR1.Key, out var r2 ) )
                {
                    atLeastOne = true;
                    sum2 += (movieR1.Value - r2) ^ 2;
                }
            }
            return atLeastOne ? Math.Sqrt( sum2 ) : double.PositiveInfinity;
        }

        public double Similarity( User u1, User u2 ) => 1.0 / (1.0 + Distance( u1, u2 ));

        public double SimilarityPearson( User u1, User u2 )
        {
            // This should call the "real" one below.
            throw new NotImplementedException();
        }

        public double SimilarityPearson( IEnumerable<(int x, int y)> values )
        {
            throw new NotImplementedException();
        }

        public bool LoadFrom( string folder )
        {
            string p = Path.Combine( folder, "users.dat" );
            if( !File.Exists( p ) ) return false;
            Users = User.ReadUsers( p );
            p = Path.Combine( folder, "movies.dat" );
            if( !File.Exists( p ) ) return false;
            Movies = Movie.ReadMovies( p );
            p = Path.Combine( folder, "ratings.dat" );
            if( !File.Exists( p ) ) return false;
            RatingCount = User.ReadRatings( Users, Movies, p );
            return true;
        }

        public double DistanceBetween(User user1, User user2)
        {
            var ratings1 = user1.Ratings.Where( r1 => user2.Ratings.Any( r2 => r1.Key.MovieId.Equals( r2.Key.MovieId ) ) ).ToList();
            if( ratings1.Count == 0 ) return double.PositiveInfinity;

            var ratings2 = user2.Ratings.Where( r2 => ratings1.Any( r1 => r2.Key.MovieId.Equals( r1.Key.MovieId ) ) ).ToList();
            if( ratings1.Count != ratings2.Count ) throw new Exception();

            var sum = 0;
            for( var i = 0; i < ratings1.Count; i++ )
                sum += (ratings1[i].Value - ratings2[i].Value) ^ 2;

            return Math.Sqrt( sum );
        }

        public double SimilarityBetween( User user1, User user2 )
        {
            return 1.0 / (1.0 + DistanceBetween( user1, user2 ));
        }

        public double PearsonSimilarityBetween(User user1, User user2)
        {
            var ratings1 = user1.Ratings.Where( r1 => user2.Ratings.Any( r2 => r1.Key.MovieId.Equals( r2.Key.MovieId ) ) ).ToList();
            if( ratings1.Count == 0 ) return double.PositiveInfinity;

            var ratings2 = user2.Ratings.Where( r2 => ratings1.Any( r1 => r2.Key.MovieId.Equals( r1.Key.MovieId ) ) ).ToList();
            if( ratings1.Count != ratings2.Count ) throw new Exception();

            var sum1 = 0;
            foreach (var rating in ratings1)
                sum1 += rating.Value;
            var mean1 = sum1 / ratings1.Count;

            var sum2 = 0;
            foreach( var rating in ratings2 )
                sum2 += rating.Value;
            var mean2 = sum2 / ratings2.Count;

            var dividend = 0;
            for( var i = 0; i < ratings1.Count; i++ )
                dividend += (ratings1[i].Value - mean1) * (ratings2[i].Value - mean2);

            var divisorLeft = 0;
            for( var i = 0; i < ratings1.Count; i++ )
                divisorLeft += (ratings1[i].Value - mean1) ^ 2;

            var divisorRight = 0;
            for( var i = 0; i < ratings2.Count; i++ )
                divisorRight += (ratings2[i].Value - mean2) ^ 2;

            var divisor = Math.Sqrt( divisorLeft ) * Math.Sqrt( divisorRight );

            return dividend / divisor;
        }
    }
}
