using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace FinalProject.Pages
{
    public class IndexModel : PageModel
    {
        public List<EmailInfo> listEmails = new List<EmailInfo>();

        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }


        public IActionResult OnGetDeleteEmail(int emailid)
        {
            try
            {
                String connectionString = "Server=tcp:finalprojectggez.database.windows.net,1433;Initial Catalog=FinalProject;Persist Security Info=False;User ID=ggez;Password=Peepam01;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"; // Update with your connection string
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    String sql = "DELETE FROM emails WHERE EmailID = @EmailID";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@EmailID", emailid);
                        command.ExecuteNonQuery();
                    }
                }

                return RedirectToPage("/Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return Page(); // You might want to handle errors more gracefully
            }
        }

        public void OnGet()
        {
            try
            {
                String connectionString = "Server=tcp:finalprojectggez.database.windows.net,1433;Initial Catalog=FinalProject;Persist Security Info=False;User ID=ggez;Password=Peepam01;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"; // Update this connection string
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string username = User.Identity.Name ?? "";

                    String sql = "SELECT * FROM emails WHERE emailreceiver = @Username";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                EmailInfo emailInfo = new EmailInfo();
                                emailInfo.EmailID = reader.GetInt32(0).ToString();
                                emailInfo.EmailSubject = reader.GetString(1);
                                emailInfo.EmailMessage = reader.GetString(2);
                                emailInfo.EmailDate = reader.GetDateTime(3).ToString();
                                emailInfo.EmailIsRead = reader.GetString(4);
                                emailInfo.EmailSender = reader.GetString(5);
                                emailInfo.EmailReceiver = reader.GetString(6);

                                listEmails.Add(emailInfo);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }

    public class EmailInfo
    {
        public String EmailID { get; set; }
        public String EmailSubject { get; set; }
        public String EmailMessage { get; set; }
        public String EmailDate { get; set; }
        public String EmailIsRead { get; set; }
        public String EmailSender { get; set; }
        public String EmailReceiver { get; set; }
    }
}
