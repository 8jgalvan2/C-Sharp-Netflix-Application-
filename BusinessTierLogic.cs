//  Netflix Database Application Using N-Tier design 
//  <Jonathan Galvan>
//  U. of Illinois, Chicago 
// BusinessTier:  business logic, acting as interface between UI and data store.
//

using System;
using System.Collections.Generic;
using System.Data;


namespace BusinessTier
{

  //
  // Business:
  //
  public class Business
  {
    //
    // Fields:
    //
    private string _DBFile;
    private DataAccessTier.Data dataTier;


    //
    // Constructor:
    //
    public Business(string DatabaseFilename)
    {
      _DBFile = DatabaseFilename;

      dataTier = new DataAccessTier.Data(DatabaseFilename);
    }


    //
    // TestConnection:
    //
    // Returns true if we can establish a connection to the database, false if not.
    //
    public bool TestConnection()
    {
      return dataTier.TestConnection();
    }


    //
    // GetUser:
    //
    // Retrieves User object based on USER ID; returns null if user is not
    // found.
    //
    // NOTE: if the user exists in the Users table, then a name and occupation
    // is returned.  If the user does not exist in the Users table, then the user
    // id is looked up in the Reviews table.  If found, then the user is an
    // "anonymous" user, so a User object with name = "<UserID>" and no occupation
    // ("") is returned.  In other words, name = the user’s id surrounded by < >.
    //
    public User GetUser(int UserID)
    {
      //
      //
      //
      DataAccessTier.Data dataTier = new DataAccessTier.Data("netflix.mdf");

      string sql = string.Format(@"Select Users.UserID, Users.UserName, Users.Occupation From Users
                                     Where UserID = {0}",UserID);      

      object result = dataTier.ExecuteScalarQuery(sql);
      User person;

      // If User exists in the Users Table then we return User Object
      if(result != null) {
        
          DataSet ds = dataTier.ExecuteNonScalarQuery(sql);

          DataTable dt = ds.Tables["TABLE"];
          
          DataRow row = dt.Rows[0];
          
          person = new User(UserID, row["UserName"].ToString(), row["Occupation"].ToString());

          return person;      
      }

      // If not then we check in the Reviews Table
      else
      {
          string sqltwo = string.Format(@"Select Reviews.UserID From Reviews 
                                          Where UserID = {0}", UserID);

          object resultTwo = dataTier.ExecuteScalarQuery(sqltwo);

          if (resultTwo != null)
          {
              string id = Convert.ToString(UserID);
              person = new User(UserID, "<id>", "");

              return person;

          }

          return null;
      }

    }


    //
    // GetNamedUser:
    //
    // Retrieves User object based on USER NAME; returns null if user is not
    // found.
    //
    // NOTE: there are "named" users from the Users table, and anonymous users
    // that only exist in the Reviews table.  This function only looks up "named"
    // users from the Users table.
    //
    public User GetNamedUser(string UserName)
    {
      DataAccessTier.Data dataTier = new DataAccessTier.Data("netflix.mdf");
      User person;

      string sql = string.Format(@"Select Users.UserName, Users.UserID, Users.Occupation From Users
                                       Where UserName = '{0}'", UserName);

      object result = dataTier.ExecuteScalarQuery(sql);

    
      if (result != null)
      {
          DataSet ds = dataTier.ExecuteNonScalarQuery(sql);

          DataTable dt = ds.Tables["TABLE"];

          DataRow row = dt.Rows[0];
          
          int id = Convert.ToInt32(row["UserID"]); 

          person = new User( id, UserName ,row["Occupation"].ToString());

          return person;
      }
        
      return null;

    }


    //
    // GetAllNamedUsers:
    //
    // Returns a list of all the users in the Users table ("named" users), sorted 
    // by user name.
    //
    // NOTE: the database also contains lots of "anonymous" users, which this 
    // function does not return.
    //
    public IReadOnlyList<User> GetAllNamedUsers()
    {
      List<User> users = new List<User>();
      User person;

      DataAccessTier.Data dataTier = new DataAccessTier.Data("netflix.mdf");

      string sql = "Select * From Users Order By UserName ASC";

      DataSet ds = dataTier.ExecuteNonScalarQuery(sql);

      DataTable dt = ds.Tables["TABLE"];

      foreach (DataRow row in dt.Rows)
      {
          int id = Convert.ToInt32(row["UserID"]);
 
          person = new User(id, row["UserName"].ToString(), row["Occupation"].ToString());

          users.Add(person);
      }

      return users;
    }


    //
    // GetMovie:
    //
    // Retrieves Movie object based on MOVIE ID; returns null if movie is not
    // found.
    //
    public Movie GetMovie(int MovieID)
    {
      //
      // 
      //

        DataAccessTier.Data dataTier = new DataAccessTier.Data("netflix.mdf");
        Movie mine;

        string sql = string.Format(@"Select Movies.MovieID From Movies
                                     Where Movies.MovieID = {0}", MovieID);

        object result = dataTier.ExecuteScalarQuery(sql);

        if(result != null) {

            string sqlTwo = string.Format(@"Select Movies.MovieName From Movies 
                                         Where MovieID = {0}", MovieID);

            DataSet ds = dataTier.ExecuteNonScalarQuery(sqlTwo);

            DataTable dt = ds.Tables["TABLE"];
            DataRow row = dt.Rows[0];

            mine = new Movie(MovieID, row["MovieName"].ToString());

            return mine;
        }


      return null;     
    }


    //
    // GetMovie:
    //
    // Retrieves Movie object based on MOVIE NAME; returns null if movie is not
    // found.
    //
    public Movie GetMovie(string MovieName)
    {
      //
      // 
      //
        DataAccessTier.Data dataTier = new DataAccessTier.Data("netflix.mdf");
        Movie mine;
        
        string checker = MovieName;
        checker = checker.Replace("'","''");


        string sql = string.Format(@"Select Movies.MovieName From Movies
                                     Where MovieName = '{0}'", checker);

        object result = dataTier.ExecuteScalarQuery(sql);

        if (result != null)
        {

            string sqlTwo = string.Format(@"Select Movies.MovieID From Movies 
                                         Where MovieName = '{0}'", checker);

            DataSet ds = dataTier.ExecuteNonScalarQuery(sqlTwo);

            DataTable dt = ds.Tables["TABLE"];
            DataRow row = dt.Rows[0];

            int id = Convert.ToInt32(row["MovieID"]);
            mine = new Movie(id,checker);

            return mine;
        }      

        return null;
    }


    //
    // AddReview:
    //
    // Adds review based on MOVIE ID, returning a Review object containing
    // the review, review's id, etc.  If the add failed, null is returned.
    //
    public Review AddReview(int MovieID, int UserID, int Rating)
    {
      //
      // 
      //

            
      DataAccessTier.Data dataTier = new DataAccessTier.Data("netflix.mdf");
      Review review;
      int revID;
      
        
      string sql = string.Format(@"Insert into Reviews(MovieID, UserID, Rating) 
                                     Values ({0},{1},{2})
                                    Select ReviewID = SCOPE_IDENTITY()", MovieID, UserID, Rating);

      object result = dataTier.ExecuteScalarQuery(sql);


      if(result == DBNull.Value) 
      {
          return null;
      }

      else
      {
          revID = Convert.ToInt32(result);
         
          review = new Review(revID, MovieID, UserID, Rating);
          return review;
      }
         
    }


    //
    // GetMovieDetail:
    //
    // Given a MOVIE ID, returns detailed information about this movie --- all
    // the reviews, the total number of reviews, average rating, etc.  If the 
    // movie cannot be found, null is returned.
    //
    public MovieDetail GetMovieDetail(int MovieID)
    {
      //
      // 
      //

        DataAccessTier.Data dataTier = new DataAccessTier.Data("netflix.mdf");

        List<Review> reviews = new List<Review>();
        MovieDetail fleek;
        Movie m;
        Review r;
        double avgRating;
        int reviewCount;

        string sql = string.Format("Select Movies.MovieID From Movies Where MovieID = {0}", MovieID);

        object result = dataTier.ExecuteScalarQuery(sql);

        if(result != null) {

            string sqlTwo = string.Format(@"Select ROUND(AVG(Cast(Rating as float)),4) From Reviews
                                              Inner Join
                                              (
                                                Select Movies.MovieID From Movies
	                                                Where MovieID = {0}
                                              ) As Temp
                                              On Temp.MovieID = Reviews.MovieID", MovieID);

            string sqlThree = string.Format(@"Select * From Movies Where MovieID = {0}", MovieID);

            string sqlFour = string.Format(@"Select COUNT(Reviews.ReviewID) From Reviews
                                             Where MovieID = {0}", MovieID);

            string sqlFive = string.Format(@"Select * From Reviews 
                                               Where MovieID = {0}
                                               ORDER BY Rating DESC, UserID ASC", MovieID);


            // Gets average information 
            object One = dataTier.ExecuteScalarQuery(sqlTwo);
           
            if(One == DBNull.Value) {
                avgRating = 0.0;
            }

            else
            {
                avgRating = Convert.ToDouble(One);
            }

            // Gets Movie information
            DataSet dsTwo = dataTier.ExecuteNonScalarQuery(sqlThree);
            DataTable dtTwo = dsTwo.Tables["TABLE"];
            DataRow rowTwo = dtTwo.Rows[0];

            // Gets number of total reviews in a movie 
            object Three = dataTier.ExecuteScalarQuery(sqlFour);
            
            if (Three == DBNull.Value)
            {
                //return null;
                reviewCount = 0;
            }

            else
            {
                reviewCount = Convert.ToInt32(Three);
            }

            m = new Movie(MovieID, rowTwo["MovieName"].ToString());

            if(One != DBNull.Value && Three != DBNull.Value) {

                // Puts the ratings into a list 
                DataSet dsFour = dataTier.ExecuteNonScalarQuery(sqlFive);
                DataTable dtFour = dsFour.Tables["TABLE"];

                foreach(DataRow rowFour in dtFour.Rows) {
                    int reviewID = Convert.ToInt32(rowFour["ReviewID"]);
                    int userID = Convert.ToInt32(rowFour["UserID"]);
                    int rating = Convert.ToInt32(rowFour["Rating"]);
      
                    r = new Review(reviewID, MovieID, userID, rating);

                    reviews.Add(r);
                }

                fleek = new MovieDetail(m,avgRating, reviewCount, reviews);
            }
            
            else {
                fleek = new MovieDetail(m,avgRating, reviewCount, null);
            }
 
            return fleek;
        }

        return null;
    }


    //
    // GetUserDetail:
    //
    // Given a USER ID, returns detailed information about this user --- all
    // the reviews submitted by this user, the total number of reviews, average 
    // rating given, etc.  If the user cannot be found, null is returned.
    //
    public UserDetail GetUserDetail(int UserID)
    {
      //
      // 
      //
    
      DataAccessTier.Data dataTier = new DataAccessTier.Data("netflix.mdf");
      UserDetail user;
      List<Review> reviews = new List<Review>();
      User person;
      Review eachPerson;
      double avgRating;
      int reviewCount;

      string sqlAvgRating, sqlNumberReviews, sqlReviews;
      object result, resultOne, resultTwo;

      string sql = string.Format(@"Select UserID, UserName, Occupation From Users
                                     Where UserID = {0}", UserID);

      result = dataTier.ExecuteScalarQuery(sql);

      // UserId exist in the Users Table
      if(result != null) {

          DataSet ds = dataTier.ExecuteNonScalarQuery(sql);
          DataTable dt = ds.Tables["TABLE"];
          DataRow row = dt.Rows[0];

          person = new User(UserID, row["UserName"].ToString(), row["Occupation"].ToString());

          sqlAvgRating = string.Format(@"Select ROUND(AVG(Cast(Rating as float)),4) From Reviews
                                                  Inner Join
                                                  (
                                                    Select UserID From Users
	                                                  Where UserID = {0}
                                                  ) As Temp
                                                  On Temp.UserID = Reviews.UserID",UserID);

          sqlNumberReviews = string.Format(@"Select Count(ReviewID) From Reviews 
                                                      Where userID = {0}", UserID);

          sqlReviews = string.Format(@"Select ReviewID, MovieID, UserID, Rating From Reviews
                                                  where UserID = {0}
                                                  Order BY Rating DESC", UserID);
          // Gets the avgrating for user 
          resultOne = dataTier.ExecuteScalarQuery(sqlAvgRating);

          if(resultOne == DBNull.Value) {
              return null;
          }

          else
          {
              avgRating = Convert.ToDouble(resultOne);
          }


          // gets the total number of reviews by users  
          resultTwo = dataTier.ExecuteScalarQuery(sqlNumberReviews); 

          if(resultTwo == DBNull.Value) {
              return null;
          }

          else
          {
              reviewCount = Convert.ToInt32(resultTwo);
          }

          // Retrieves the ratings as a dataset 
          DataSet dsTwo = dataTier.ExecuteNonScalarQuery(sqlReviews);
          DataTable dtTwo = dsTwo.Tables["TABLE"];

          foreach(DataRow rowTwo in dtTwo.Rows) {
              int reviewNumber = Convert.ToInt32(rowTwo["ReviewID"]);
              int movieNumber = Convert.ToInt32(rowTwo["MovieID"]);
              int ratingNumber = Convert.ToInt32(rowTwo["Rating"]);

              eachPerson = new Review(reviewNumber, movieNumber, UserID, ratingNumber);

              reviews.Add(eachPerson);
          }

          //return userdetail
          user = new UserDetail(person, avgRating, reviewCount, reviews);
          return user;
      }

      else
      {
          // Check to see if the UserID exists in the reviews table 
          string sqlTwo = string.Format(@"Select Reviews.UserID From Reviews
                                            Where UserID = {0}", UserID);
          
          object resultAnonymous= dataTier.ExecuteScalarQuery(sqlTwo);

          // UserID exists in Reviews
          if(resultAnonymous != null) {
          
              // Add information for userDetail for being anonymous
              string userName = string.Format(@"<{0}>",UserID);
              string occupation = "";

              person = new User(UserID, userName, occupation);


              sqlAvgRating = string.Format(@"Select ROUND(AVG(Cast(Rating as float)),4) From Reviews
                                                  Inner Join
                                                  (
                                                    Select UserID From Users
	                                                  Where UserID = {0}
                                                  ) As Temp
                                                  On Temp.UserID = Reviews.UserID", UserID);

              sqlNumberReviews = string.Format(@"Select Count(ReviewID) From Reviews 
                                                      Where userID = {0}", UserID);

              sqlReviews = string.Format(@"Select ReviewID, MovieID, UserID, Rating From Reviews
                                                  where UserID = {0}
                                                  Order BY Rating DESC", UserID);


              resultOne = dataTier.ExecuteScalarQuery(sqlAvgRating);

              if (resultOne == DBNull.Value)
              {
                  avgRating = 0.0;
              }

              else
              {
                  avgRating = Convert.ToDouble(resultOne);
              }


              // gets the total number of reviews by users  
              resultTwo = dataTier.ExecuteScalarQuery(sqlNumberReviews);

              if (resultTwo == DBNull.Value)
              {
                  reviewCount = 0;
              }

              else
              {
                  reviewCount = Convert.ToInt32(resultTwo);
              }

              if(resultOne != DBNull.Value && resultTwo != DBNull.Value) {
                  // Retrieves the ratings as a dataset 
                  DataSet dsFive = dataTier.ExecuteNonScalarQuery(sqlReviews);
                  DataTable dtFive = dsFive.Tables["TABLE"];

                  foreach (DataRow rowFive in dtFive.Rows)
                  {
                      int reviewNumber = Convert.ToInt32(rowFive["ReviewID"]);
                      int movieNumber = Convert.ToInt32(rowFive["MovieID"]);
                      int ratingNumber = Convert.ToInt32(rowFive["Rating"]);

                      eachPerson = new Review(reviewNumber, movieNumber, UserID, ratingNumber);

                      reviews.Add(eachPerson);
                  }
                  
                  user = new UserDetail(person, avgRating, reviewCount, reviews);
                  return user;
              }
          
              else 
              {
                  string name = string.Format(@"<{0>",UserID);
                  User personTwo = new User(UserID, name, "");

                  user = new UserDetail(personTwo, avgRating, reviewCount, null);
                  return user;
              } 
          }

          return null;
      }

    }


    //
    // GetTopMoviesByAvgRating:
    //
    // Returns the top N movies in descending order by average rating.  If two
    // movies have the same rating, the movies are presented in ascending order
    // by name.  If N < 1, an EMPTY LIST is returned.
    //
    public IReadOnlyList<Movie> GetTopMoviesByAvgRating(int N)
    {
      List<Movie> movies = new List<Movie>();
      Movie MovieInfo;

      DataAccessTier.Data dataTier = new DataAccessTier.Data("netflix.mdf");
      
      //
      // 
      //

      string sql = string.Format(@"Select TOP {0} Movies.MovieName, Temp.AVGRating, Temp.MovieID From Movies
                                      Inner Join
                                      (
                                        Select Reviews.MovieID,
		                                       Round(AVG(Cast(Reviews.Rating As Float )),4) as AVGRating
                                        From Reviews
                                        Group By Reviews.MovieID
                                      ) As Temp
                                    On Temp.MovieID = Movies.MovieID
                                    Order By Temp.AVGRating DESC, Movies.MovieName ASC;", N);

      DataSet ds = dataTier.ExecuteNonScalarQuery(sql);
      DataTable dt = ds.Tables["TABLE"];
      
      foreach(DataRow row in dt.Rows) {

          int movieID = Convert.ToInt32(row["MovieID"]);
          MovieInfo = new Movie(movieID, row["MovieName"].ToString());

          movies.Add(MovieInfo);
      }

      return movies;
    }


    //
    // GetTopMoviesByNumReviews
    //
    // Returns the top N movies in descending order by number of reviews.  If two
    // movies have the same number of reviews, the movies are presented in ascending
    // order by name.  If N < 1, an EMPTY LIST is returned.
    //
    public IReadOnlyList<Movie> GetTopMoviesByNumReviews(int N)
    {
      List<Movie> movies = new List<Movie>();

      Movie MovieInfo;

      DataAccessTier.Data dataTier = new DataAccessTier.Data("netflix.mdf");
      //
      // 
      //

      string sql = string.Format(@"Select TOP {0} Movies.MovieName, Temp.number, Temp.MovieID From Movies
                                      Inner Join
                                      (
                                        Select Reviews.MovieID,
		                                       Count(Rating) as number
                                        From Reviews
                                        Group By Reviews.MovieID
                                      ) As Temp
                                    On Temp.MovieID = Movies.MovieID
                                    Order By Temp.number DESC, Movies.MovieName ASC",N);

      DataSet ds = dataTier.ExecuteNonScalarQuery(sql);
      DataTable dt = ds.Tables["TABLE"];

      foreach (DataRow row in dt.Rows)
      {

          int movieID = Convert.ToInt32(row["MovieID"]);
          MovieInfo = new Movie(movieID, row["MovieName"].ToString());

          movies.Add(MovieInfo);
      }

      return movies;
    }


    //
    // GetTopUsersByNumReviews
    //
    // Returns the top N users in descending order by number of reviews.  If two
    // users have the same number of reviews, the users are presented in ascending
    // order by user id.  If N < 1, an EMPTY LIST is returned.
    //
    // NOTE: not all user ids map to users in the Users table.  User ids that don't
    // map over are considered "anonymous" users, and returned with their name =
    // to their userid ("<UserID>") and no occupation ("").
    //
    public IReadOnlyList<User> GetTopUsersByNumReviews(int N)
    {
      List<User> users = new List<User>();
      User indivudal;
      DataAccessTier.Data dataTier = new DataAccessTier.Data("netflix.mdf");

      //
      // execute query to rank users:
      //
      // NOTE: some reviews are anonymous, i.e. we don't have a username.  So we
      // use a "RIGHT JOIN" to capture those as well.
      //
      string sql = string.Format(@"SELECT TOP {0} Temp.UserID, Users.UserName, Users.Occupation
            FROM Users
            RIGHT JOIN
            (
              SELECT UserID, COUNT(*) AS RatingCount
              FROM Reviews
              GROUP BY UserID
            ) AS Temp
            On Temp.UserID = Users.UserID
            ORDER BY Temp.RatingCount DESC, Users.UserName ASC;",
        N);
        
      //
      // Now execute this query...  In the resulting dataset, the anonymous users will
      // have a UserName of "" because the result of the join was NULL.  So when you
      // come across a user with "" as their name, create a new based on their user id,
      // i.e. string.Format(""<{0}>", userid);
      //
      
      
      //
      // 
      //

      DataSet ds = dataTier.ExecuteNonScalarQuery(sql);
      DataTable dt = ds.Tables["TABLE"];

      foreach (DataRow row in dt.Rows)
      {
          int userID = Convert.ToInt32(row["UserID"]);

          indivudal = new User(userID, row["UserName"].ToString(), row["Occupation"].ToString());
          users.Add(indivudal);
      }

      return users;
    }



      //
      //GetAllMovies:
      //
      // Returns alist ofall the movies in the database, sorted by movie name.
    public IReadOnlyList<Movie> GetAllMovies()
    {
        List<Movie> movie = new List<Movie>();
        Movie m;

        DataAccessTier.Data dataTier = new DataAccessTier.Data("netflix.mdf");

        string sql = string.Format(@"Select * From Movies 
                                       ORDER BY MovieName ASC");

        DataSet ds = dataTier.ExecuteNonScalarQuery(sql);
        DataTable dt = ds.Tables["TABLE"];

        foreach (DataRow row in dt.Rows)
        {
            int mID = Convert.ToInt32(row["MovieID"]);
            m = new Movie(mID, row["MovieName"].ToString());

            movie.Add(m);
        }

        return movie;
    }


  }//class
}//namespace
