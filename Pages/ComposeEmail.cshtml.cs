using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace FinalProject.Pages
{
    public class ComposeEmailModel : PageModel
    {
        private readonly ILogger<ComposeEmailModel> _logger;
        private readonly string connectionString = "Server=tcp:finalprojectggez.database.windows.net,1433;Initial Catalog=FinalProject;Persist Security Info=False;User ID=ggez;Password=Peepam01;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"; // Replace with your connection string

        public ComposeEmailModel(ILogger<ComposeEmailModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            // Optional: Add logic for OnGet if needed
        }

        public IActionResult OnPost(string ToEmail, string EmailSubject, string EmailMessage)
        {
            try
            {
                string senderEmail = User.Identity.Name; // Replace this with actual sender email or get it from your authentication system

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Check if the recipient's email exists in the database
                    if (!EmailExists(connection, ToEmail))
                    {
                        TempData["WarningMessage"] = "The specified recipient email does not exist.";
                        return Page();
                    }

                    // Insert the composed email into the database
                    string insertQuery = @"INSERT INTO emails (emailsubject, emailmessage, emaildate, emailisread, emailsender, emailreceiver)
                                           VALUES (@EmailSubject, @EmailMessage, @EmailDate, @EmailIsRead, @EmailSender, @EmailReceiver)";

                    using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@EmailSubject", EmailSubject);
                        insertCommand.Parameters.AddWithValue("@EmailMessage", EmailMessage);
                        insertCommand.Parameters.AddWithValue("@EmailDate", DateTime.Now);
                        insertCommand.Parameters.AddWithValue("@EmailIsRead", 0); // Assuming newly composed emails are unread
                        insertCommand.Parameters.AddWithValue("@EmailSender", senderEmail);
                        insertCommand.Parameters.AddWithValue("@EmailReceiver", ToEmail);

                        insertCommand.ExecuteNonQuery();
                    }
                }

                return RedirectToPage("/Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email");
                // Handle the error gracefully - display an error message on the page
                throw;
            }
        }

        private bool EmailExists(SqlConnection connection, string email)
        {
            string query = "SELECT COUNT(*) FROM emails WHERE emailreceiver = @Email";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Email", email); // Bind the parameter using AddWithValue

                object result = null;
                command.CommandType = CommandType.Text;

                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                try
                {
                    result = command.ExecuteScalar();
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }

                if (result != null && result != DBNull.Value)
                {
                    int count;
                    if (int.TryParse(result.ToString(), out count))
                    {
                        return count > 0;
                    }
                }

                return false;
            }
        }




    }
}
