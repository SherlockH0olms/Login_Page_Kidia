using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace $safeprojectname$
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        private string strcon;

        protected void Page_Load(object sender, EventArgs e)
        {
            strcon = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
        }
        protected TextBox TextBox1;
        protected TextBox TextBox2;
        protected TextBox TextBox3;


        //  (Sign Up)
        protected void Button1_Click(object sender, EventArgs e)
        {
            string fullName = TextBox1.Text.Trim();
            string email = TextBox2.Text.Trim();
            string password = TextBox3.Text.Trim();

            // Email format yoxla
            if (string.IsNullOrWhiteSpace(email) || !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                Response.Write("<script>alert('Invalid email format.');</script>");
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    con.Open();

                    // Bu Email daha əvvəldən giris edib?
                    using (SqlCommand checkUser = new SqlCommand("SELECT COUNT(*) FROM member_master_tbl WHERE email=@Email", con))
                    {
                        checkUser.Parameters.Add("@Email", SqlDbType.NVarChar).Value = email;

                        int count = Convert.ToInt32(checkUser.ExecuteScalar());
                        if (count > 0)
                        {
                            Response.Write("<script>alert('This email is already registered!');</script>");
                            return;
                        }
                    }

                    // İstifadəcini database əlavə et
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO member_master_tbl(full_name,email,password) VALUES(@FullName,@Email,@Password)", con))
                    {
                        cmd.Parameters.Add("@FullName", SqlDbType.NVarChar).Value = fullName;
                        cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = email;
                        cmd.Parameters.Add("@Password", SqlDbType.NVarChar).Value = password;

                        cmd.ExecuteNonQuery();
                    }

                    Response.Write("<script>alert('Sign Up Successful. Please log in.');</script>");
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Error: " + ex.Message + "');</script>");
            }
        }

        //  (Login)
        protected void Button2_Click(object sender, EventArgs e)
        {
            string email = TextBox4.Text.Trim();
            string password = TextBox5.Text.Trim();

            try
            {
                using (SqlConnection con = new SqlConnection(strcon))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM member_master_tbl WHERE email=@Email AND password=@Password", con))
                    {
                        cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = email;
                        cmd.Parameters.Add("@Password", SqlDbType.NVarChar).Value = password;

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.HasRows)
                            {
                                Response.Write("<script>alert('Login Successful');</script>");
                                // Response.Redirect("dashboard.aspx");
                            }
                            else
                            {
                                Response.Write("<script>alert('Invalid Credentials');</script>");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Error: " + ex.Message + "');</script>");
            }
        }
    }
}
