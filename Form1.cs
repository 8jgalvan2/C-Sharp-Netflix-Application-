// Netflix Database Application using N-Tier design 
// <Jonathan Galvan>
// U. of Illinois, Chicago 
// CS 341, Fall 2015
// Homework 9

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;


namespace NetflixApp
{
  public partial class Form1 : Form
  {
    //
    // Class members:
    //
    private string m_connectionInfo;

    //
    // Constructor:
    //
    public Form1()
    {
      InitializeComponent();

      string filename, version;

      version = "v11.0";    // for VS 2013:
      //version = "MSSQLLocalDB";  // for VS 2015:
      filename = "netflix.mdf";

      m_connectionInfo = String.Format(@"Data Source=(LocalDB)\{0};AttachDbFilename=|DataDirectory|\{1};Integrated Security=True;", 
        version, 
        filename);
    }


    //
    // Form1_Load:  called just before the form is displayed to the user:
    //
    private void Form1_Load(object sender, EventArgs e)
    {
    }


    private void tbarRating_Scroll(object sender, EventArgs e)
    {
      lblRating.Text = tbarRating.Value.ToString();
    }

    //
    // Add Review:
    //
    private void cmdInsertReview_Click(object sender, EventArgs e)
    {
      //
      // Get the movie name from the list of movies:
      //
      if (this.listBox1.SelectedIndex < 0)
      {
        MessageBox.Show("Please select a movie...");
        return;
      }

      string MovieName = this.listBox1.Text;

      //
      // And the user name from the list of users:
      //
      if (this.listBox2.SelectedIndex < 0)
      {
        MessageBox.Show("Please select a user...");
        return;
      }

      string UserName = this.listBox2.Text;

      //
      // NOTE: since a movie and a user is selected, the movie and user IDs are 
      // available from the associated text boxes:
      //
      int movieid = Convert.ToInt32(this.txtMovieID.Text);
      int userid = Convert.ToInt32(this.txtUserID.Text);
      int rating = Convert.ToInt32(this.lblRating.Text);

      BusinessTier.Business BT = new BusinessTier.Business("netflix.mdf");
      //
      // Insert movie review:
      //

      var insert = BT.AddReview(movieid, userid, rating); 

      //
      // display results:
      //
      if (insert != null) // success!
      {
        MessageBox.Show("Success, review's new id is " + insert.ReviewID);
      }
      else
      {
        MessageBox.Show("**Failure, insert was not added (?) **");
      }
    }


    //
    // All Movies:
    //
    private void cmdAllMovies_Click(object sender, EventArgs e)
    {
        listBox1.Items.Clear();

        BusinessTier.Business BT = new BusinessTier.Business("netflix.mdf");

        var movies = BT.GetAllMovies();

        foreach (BusinessTier.Movie m in movies)
        {
            this.listBox1.Items.Add(m.MovieName);

        }
   
    }
    //
    // When the user selects a movie, display movie id and average rating...
    //
    private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
        string name = this.listBox1.Text;

       
        BusinessTier.Business BT = new BusinessTier.Business("netflix.mdf");



        var movie = BT.GetMovie(name);

        if (movie == null)
        {
            MessageBox.Show("**Internal Error, movie not found ?!");
            return;
        }

        this.txtMovieID.Text = movie.MovieID.ToString();

        
        var detail = BT.GetMovieDetail(movie.MovieID);

        if(detail == null) 
        {
            MessageBox.Show("**Internal Error, detail not found ?!");
            return;
        }

        this.txtAvgRating.Text = detail.AvgRating.ToString();
        

    }


    //
    // Reviews for selected movie:
    //
    private void cmdMovieReviews_Click(object sender, EventArgs e)
    {
      string name;

      if (this.listBox1.SelectedIndex < 0)
      {
        MessageBox.Show("Please select a movie...");
        return;
      }

      name = this.listBox1.Text;

      BusinessTier.Business BT = new BusinessTier.Business("netflix.mdf");

      //
      // NOTE: since a movie is selected, the movie id is in the associated textbox:
      //

      int movieid = Convert.ToInt32(this.txtMovieID.Text);

     
      // Return the avgRating as an int to check if the movie has any reviews  
      double rating = Convert.ToDouble(this.txtAvgRating.Text);


      // Return an instance of a movie detail object
      var movie = BT.GetMovieDetail(movieid);

      // Test Case to see if the movie exists 

      if (movie == null)
      {
          MessageBox.Show("**Internal Error, movie could not be found !?");
          return;
      } 
      
      // Creates and displays information into a subform    

      SubForm frm = new SubForm();

      frm.lblHeader.Text = string.Format("Reviews for \"{0}\"", name);

      frm.listBox1.Items.Add(name);
      frm.listBox1.Items.Add("");

      if (rating == 0)
      {
          frm.listBox1.Items.Add("No reviews available");
      }

      else
      {
 
          //frm.listBox1.Items.Add(movie.Reviews); // Erase not necessary

          foreach (BusinessTier.Review r in movie.Reviews)
          {
              frm.listBox1.Items.Add(r.UserID + ": " + r.Rating);
          }
      }

      
      frm.ShowDialog();
    }


    //
    // Summary of reviews (by each rating) for selected movie:
    //
    private void cmdReviewsSummary_Click(object sender, EventArgs e)
    {
      string name;
  
      int countOne = 0, countTwo = 0, countThree = 0, countFour = 0, countFive = 0, totalCount = 0; // Keep count of review counts 


      if (this.listBox1.SelectedIndex < 0)
      {
        MessageBox.Show("Please select a movie...");
        return;
      }

      name = this.listBox1.Text;

      BusinessTier.Business BT = new BusinessTier.Business("netflix.mdf");


      //
      // NOTE: since a movie is selected, the movie id is in the associated textbox:
      //

      int movieid = Convert.ToInt32(this.txtMovieID.Text);

      // returns average rating as an int 
      double rating = Convert.ToDouble(this.txtAvgRating.Text);  
        
      // Return an instance of movieDetail object
      var movie = BT.GetMovieDetail(movieid);

      // If Program crashes add the test case if movie == null then return. CHeck to see if the movie exists

      if(movie == null)
      {
          MessageBox.Show("**Internal Error, movie could not be found!?");
          return;
      }

      // Display information into a sub form  
      SubForm frm = new SubForm();

      frm.lblHeader.Text = string.Format("Reviews for \"{0}\"", name);
 
      frm.listBox1.Items.Add(name);
      frm.listBox1.Items.Add("");



      if (rating == 0)
      {
          frm.listBox1.Items.Add("5: " + countFive);
          frm.listBox1.Items.Add("4: " + countFour);
          frm.listBox1.Items.Add("3: " + countThree);
          frm.listBox1.Items.Add("2: " + countTwo);
          frm.listBox1.Items.Add("1: " + countOne);
          frm.listBox1.Items.Add("");

          frm.listBox1.Items.Add("Total Count: " + totalCount);
      }

      else
      {
         
          foreach(BusinessTier.Review r in movie.Reviews) 
          {
              if(r.Rating == 5) {
                  countFive++;
              }

              else if(r.Rating == 4) {
                  countFour++;
              }

              else if(r.Rating == 3) {
                  countThree++;
              }

              else if(r.Rating == 2) {
                  countTwo++;
              }

              else if(r.Rating == 1) {
                  countOne++;
              }

              totalCount++;
          }

          // Print out to the subform 
          frm.listBox1.Items.Add("5: " + countFive);
          frm.listBox1.Items.Add("4: " + countFour);
          frm.listBox1.Items.Add("3: " + countThree);
          frm.listBox1.Items.Add("2: " + countTwo);
          frm.listBox1.Items.Add("1: " + countOne);
          frm.listBox1.Items.Add("");

          frm.listBox1.Items.Add("Total Count: " + totalCount);
      }

      frm.ShowDialog();
    }


    //
    // All Users:
    //
    private void cmdAllUsers_Click(object sender, EventArgs e)
    {
      listBox2.Items.Clear();

      BusinessTier.Business BT = new BusinessTier.Business("netflix.mdf");

      var users= BT.GetAllNamedUsers();

      if(users == null) {
          MessageBox.Show("**Internal Error, user not found");
          return;
      }  

      
      foreach(BusinessTier.User u in users) 
      {
          this.listBox2.Items.Add(u.UserName);
      }


    }


    //
    // User has selected a user in the list:
    //
    private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
    {
      string name;
  
      name = this.listBox2.Text;
      name = name.Replace("'", "''");

      BusinessTier.Business BT = new BusinessTier.Business("netflix.mdf");

      var nameID = BT.GetNamedUser(name);   

      if(nameID == null) 
      {
        MessageBox.Show("**Internal Error, nameID not found");
        return;
      }

      this.txtUserID.Text = nameID.UserID.ToString();
      this.txtOccupation.Text = nameID.Occupation.ToString();

    }


    //
    // Reviews for selected user:
    //
    private void cmdUserReviews_Click(object sender, EventArgs e)
    {
      string name;

      if (this.listBox2.SelectedIndex < 0)
      {
        MessageBox.Show("Please select a user...");
        return;
      }

      name = this.listBox2.Text;

      BusinessTier.Business BT = new BusinessTier.Business("netflix.mdf");

      //
      // NOTE: since a user is selected, the user id is in the associated textbox:
      //

      int userid = Convert.ToInt32(this.txtUserID.Text);

      //
      // Get all the reviews by this user:
      //

      var userInfo = BT.GetUserDetail(userid);

      if(userInfo == null) {
          MessageBox.Show("**Internal Error, user could not be found!?");
          return;
      }


      // 
      // Display the results in a subform:
      //
      SubForm frm = new SubForm();

      frm.lblHeader.Text = string.Format("Reviews by \"{0}\"", name);

      frm.listBox1.Items.Add(name);
      frm.listBox1.Items.Add("");

      if(userInfo.AvgRating == 0) {
          frm.listBox1.Items.Add("No reviews to display");
      }

      else {
          
          foreach(BusinessTier.Review r in userInfo.Reviews) {

              var retrieveMovieName = BT.GetMovie(r.MovieID);
              
              frm.listBox1.Items.Add(retrieveMovieName.MovieName + " -> "+ r.Rating);
          }
          

      }


      frm.ShowDialog();
    }


    //
    // File >> Test Connection:
    //
    private void testConnectionToolStripMenuItem_Click(object sender, EventArgs e)
    {
      BusinessTier.Business BT = new BusinessTier.Business("netflix.mdf");
      var result = BT.TestConnection();
      try
      {
        
      
          if(result == true) {
              MessageBox.Show("Connection Open");

          }

        MessageBox.Show("Connection Closed");
      }

      catch
      {
        MessageBox.Show("**Error: database file not found?!");
      }
    }


    //
    // File >> Exit:
    //
    private void exitToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.Close();
    }


    //
    // File >> Top Movies by Avg Rating:
    //
    private void topMoviesByRatingToolStripMenuItem_Click(object sender, EventArgs e)
    {

      BusinessTier.Business BT = new BusinessTier.Business("netflix.mdf");
      
      //
      // Group all the reviews for each movie, compute averages, and take top N:
      //

      //string N = txtTopN.Text;
      int N = Convert.ToInt32(this.txtTopN.Text);

      var data = BT.GetTopMoviesByAvgRating(N);  
            
        SubForm frm = new SubForm();

        frm.lblHeader.Text = "Top Movies by Average Rating";

        foreach (BusinessTier.Movie r in data)
        {
          var rating = BT.GetMovieDetail(r.MovieID);
          frm.listBox1.Items.Add(r.MovieName + ": "+ rating.AvgRating);
        }

        frm.ShowDialog();
      
    }


    //
    // File >> Top Movies by Num Reviews:
    //
    private void topMoviesByNumReviewsToolStripMenuItem_Click(object sender, EventArgs e)
    {
      BusinessTier.Business BT = new BusinessTier.Business("netflix.mdf");
      
      //
      // Group all the reviews for each movie, compute averages, and take top N:
      //
      //string N = txtTopN.Text;
      int N = Convert.ToInt32(this.txtTopN.Text);
     
      var data = BT.GetTopMoviesByNumReviews(N);

        SubForm frm = new SubForm();

        frm.lblHeader.Text = "Top Movies by Number of Reviews";

        foreach (BusinessTier.Movie r in data)
        {

          var count = BT.GetMovieDetail(r.MovieID);
          frm.listBox1.Items.Add(r.MovieName + ": " + count.NumReviews);
        }

        frm.ShowDialog();
      
    }


    //
    // File >> Top Users by New Reviews:
    //
    private void topUsersByNumReviewsToolStripMenuItem_Click(object sender, EventArgs e)
    {
      BusinessTier.Business BT = new BusinessTier.Business("netflix.mdf");
       
      //
      // Group all the reivews by user, count, and take top N:
      //
      //string N = txtTopN.Text;
      int N = Convert.ToInt32(this.txtTopN.Text);

      var data = BT.GetTopUsersByNumReviews(N);      


      //
      // display results:
      //
      
        SubForm frm = new SubForm();

        frm.lblHeader.Text = "Top Users by Number of Reviews";

        foreach (BusinessTier.User r in data)
        {

          var rating = BT.GetUserDetail(r.UserID);
 
          frm.listBox1.Items.Add(r.UserName + ": " + rating.NumReviews);
        }

        frm.ShowDialog();
      
    }

    private void txtMovieID_TextChanged(object sender, EventArgs e)
    {

    }

    private void txtAvgRating_TextChanged(object sender, EventArgs e)
    {

    }

    private void txtUserID_TextChanged(object sender, EventArgs e)
    {

    }

    private void txtTopN_TextChanged(object sender, EventArgs e)
    {

    }

    private void txtOccupation_TextChanged(object sender, EventArgs e)
    {

    }

    private void label4_Click(object sender, EventArgs e)
    {

    }

    private void label5_Click(object sender, EventArgs e)
    {

    }

    private void label3_Click(object sender, EventArgs e)
    {

    }

    private void lblRating_Click(object sender, EventArgs e)
    {

    }

    private void button1_Click(object sender, EventArgs e)
    {
        if (this.textBox1.Text.Length == 0)
        {
            MessageBox.Show("Enter a movie id");
            return;
        }

        int movieIDnum = Convert.ToInt32(this.textBox1.Text);

        BusinessTier.Business BT = new BusinessTier.Business("netflix.mdf");

        var movie = BT.GetMovie(movieIDnum);

        if(movie != null) {
            MessageBox.Show("The name of the movie is " + movie.MovieName);
        }

        else {
            MessageBox.Show("**Internal error, the movie does not exist");
        }


    }

    private void textBox1_TextChanged(object sender, EventArgs e)
    {

    }

    private void label6_Click(object sender, EventArgs e)
    {

    }

    private void button2_Click(object sender, EventArgs e)
    {
        
        if(this.textBox2.Text.Length == 0) {
            MessageBox.Show("Enter a user id");
            return;
        }


        int userID = Convert.ToInt32(this.textBox2.Text);

        BusinessTier.Business BT = new BusinessTier.Business("netflix.mdf");

        var userInfo = BT.GetUser(userID);

        if(userInfo == null) {
            MessageBox.Show("**Internal Error, user does not exist");
            return;
        }

        else {

            if(userInfo.UserName.Substring(0,1).StartsWith("<"))  {
                MessageBox.Show("User is anonymous");
            }

            else {
                MessageBox.Show("User is " + userInfo.UserName);
            }

        }
    }

    private void textBox2_TextChanged(object sender, EventArgs e)
    {

    }

  }//class
}//namespace
