using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System;

namespace FinalProject.Pages
{
    public class ReadEmailModel : PageModel
    {
        public EmailInfo Email { get; set; }

        public IActionResult OnGet(int emailId)
        {
            Email = GetEmailById(emailId);

            if (Email == null)
            {
                return NotFound(); // Return a 404 NotFound result if the email is not found
            }

            return Page(); // Return the page with the email details
        }

        private EmailInfo GetEmailById(int emailId)
        {
            // Replace this with your logic to fetch email details from the database or any other source
            // This is a placeholder method and should be replaced with your actual implementation
            string connectionString = "Server=tcp:finalprojectggez.database.windows.net,1433;Initial Catalog=FinalProject;Persist Security Info=False;User ID=ggez;Password=Peepam01;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"; // Replace with your database connection string

            EmailInfo email = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT EmailID, EmailSubject, EmailMessage, EmailDate, EmailIsRead, EmailSender, EmailReceiver FROM emails WHERE EmailID = @EmailID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@EmailID", emailId);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                email = new EmailInfo
                                {
                                    EmailID = reader.GetInt32(0).ToString(),
                                    EmailSubject = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                                    EmailMessage = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                                    EmailDate = reader.GetDateTime(3).ToString(),
                                    EmailIsRead = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                                    EmailSender = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                                    EmailReceiver = reader.IsDBNull(6) ? string.Empty : reader.GetString(6)
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return email;
        }
    }

}