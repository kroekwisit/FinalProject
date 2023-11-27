using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace FinalProject.Pages
{
    public class ReplyModel : PageModel
    {
        private readonly string _connectionString = "Server=tcp:finalprojectggez.database.windows.net,1433;Initial Catalog=FinalProject;Persist Security Info=False;User ID=ggez;Password=Peepam01;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        [BindProperty(SupportsGet = true)]
        public string Email { get; set; } // Bind property to receive the sender's email address

        [BindProperty]
        public EmailModel ReplyEmail { get; set; } // Model for replying to the email

        public IActionResult OnGet()
        {
            // Set the "To" field with the sender's email address received from the query parameter
            ReplyEmail = new EmailModel { ToUserName = Email };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                string senderName = User.FindFirstValue(ClaimTypes.Name); // Get sender's name from claims

                bool userExists = await CheckIfUserExists(ReplyEmail.ToUserName);

                if (!userExists)
                {
                    ModelState.AddModelError("ReplyEmail.ToUserName", "User does not exist.");
                    return Page();
                }

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string insertQuery = "INSERT INTO emails (emailsubject, emailmessage, emaildate, emailisread, emailsender, emailreceiver) " +
                                         "VALUES (@EmailSubject, @EmailMessage, @EmailDate, @EmailIsRead, @EmailSender, @EmailReceiver)";

                    using (SqlCommand command = new SqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@EmailSubject", ReplyEmail.EmailSubject);
                        command.Parameters.AddWithValue("@EmailMessage", ReplyEmail.EmailMessage);
                        command.Parameters.AddWithValue("@EmailDate", DateTime.UtcNow);
                        command.Parameters.AddWithValue("@EmailIsRead", false);
                        command.Parameters.AddWithValue("@EmailSender", senderName); // Use the retrieved sender name
                        command.Parameters.AddWithValue("@EmailReceiver", ReplyEmail.ToUserName);

                        await command.ExecuteNonQueryAsync();
                    }
                }

                return RedirectToPage("/Success"); // Redirect to a success page after sending the reply
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while processing your request. Please try again.");
                return Page(); // Return to the same page to display the error message
            }
        }

        private async Task<bool> CheckIfUserExists(string username)
        {
            // Perform a query to check if the user exists in your user database table
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT COUNT(*) FROM AspNetUsers WHERE UserName = @UserName";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserName", username);

                    int userCount = (int)await command.ExecuteScalarAsync();
                    return userCount > 0;
                }
            }
        }
    }

}

