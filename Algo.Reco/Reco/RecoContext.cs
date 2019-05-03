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
                    sum2 += (movieR1.Value - r2) * (movieR1.Value - r2);
                }
            }
            return atLeastOne ? Math.Sqrt( sum2 ) : double.PositiveInfinity;
        }

        public double Similarity( User u1, User u2 ) => 1.0 / (1.0 + Distance( u1, u2 ));

        public double SimilarityPearson( User u1, User u2 )
        {
            var ratings = new List<(int x, int y)>();
            foreach( var movieR1 in u1.Ratings )
                if( u2.Ratings.TryGetValue( movieR1.Key, out var r2 ) )
                    ratings.Add( (movieR1.Value, r2) );

            return SimilarityPearson(ratings);
        }

        public double SimilarityPearson( IEnumerable<(int x, int y)> values )
        {
            var val = values.ToList();
            var sumX = 0;
            var sumY = 0;
            foreach( var (x, y) in val )
            {
                sumX += x;
                sumY += y;
            }

            var averageX = sumX / val.Count;
            var averageY = sumY / val.Count;

            var dividend = 0;
            foreach ( var (x, y) in val )
                dividend += (x - averageX) * (y - averageY);

            var divisorLeft = 0;
            foreach( var (x, y) in val )
                divisorLeft += (x - averageX) * (x - averageX);

            var divisorRight = 0;
            foreach( var (x, y) in val )
                divisorRight += (y - averageY) * (y - averageY);

            var divisor = Math.Sqrt( divisorLeft ) * Math.Sqrt( divisorRight );

            return dividend / divisor;
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

        public (IEnumerable<KeyValuePair<Movie, int>> UserRatings1, IEnumerable<KeyValuePair<Movie, int>> UserRatings2) GetRatings( User user1, User user2 )
        {
            var UserRatings1 = user1.Ratings.Where( r1 => user2.Ratings.Any( r2 => r1.Key.MovieId.Equals( r2.Key.MovieId ) ) );
            var UserRatings2 = user2.Ratings.Where( r2 => UserRatings1.Any( r1 => r2.Key.MovieId.Equals( r1.Key.MovieId ) ) );

            return (UserRatings1, UserRatings2);
        }
    }
}
