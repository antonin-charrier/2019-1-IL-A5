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

        public bool LoadFrom(string folder)
        {
            string p = Path.Combine(folder, "users.dat");
            if (!File.Exists(p)) return false; 
            Users = User.ReadUsers(p);
            p = Path.Combine(folder, "movies.dat");
            if (!File.Exists(p)) return false;
            Movies = Movie.ReadMovies(p);
            p = Path.Combine(folder, "ratings.dat");
            if (!File.Exists(p)) return false;
            RatingCount = User.ReadRatings(Users, Movies, p);
            return true;
        }

        public double DistanceBetween(User user1, User user2)
        {
            var ratings1 = user1.Ratings.Where( r1 => user2.Ratings.Any( r2 => r1.Key.MovieId.Equals( r2.Key.MovieId ) ) ).ToList();
            if( ratings1.Count == 0 ) return double.NaN;

            var ratings2 = user2.Ratings.Where( r2 => ratings1.Any( r1 => r2.Key.MovieId.Equals( r1.Key.MovieId ) ) ).ToList();
            if( ratings1.Count != ratings2.Count ) throw new Exception();

            double sum = 0;
            for (var i = 0; i < ratings1.Count; i++)
                sum += Math.Pow( ratings1[i].Value - ratings2[i].Value, 2 );

            return Math.Sqrt( sum );
        }
    }
}
