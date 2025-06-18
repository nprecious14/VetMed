using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.TwiML.Voice;
using Twilio.Types;
using DotNetEnv;

namespace VetMed
{
    public partial class Form1 : Form
    {
        // Declare global variables and connection string to the database
        string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""C:\Users\natha\Downloads\VetMedCurrent 5\VetMedCurrent\VetMed\VetMED_db.mdf"";Integrated Security=True;Connect Timeout=30;Integrated Security=True";
        private string twilioAccountSid;
        private string twilioAuthToken;
        private string twilioPhoneNumber;
        private string usernameFor2FA;
        private string phoneNumberFor2FA;
        private string generated2FACode;
        private string currentPetName;
        private int currentLoginID;
        private int petProfileID;
        public Form1()
        {
            InitializeComponent();

            // Load .env variables
            Env.Load();
            twilioAccountSid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
            twilioAuthToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");
            twilioPhoneNumber = Environment.GetEnvironmentVariable("TWILIO_PHONE_NUMBER");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            tabControl1.Appearance = TabAppearance.FlatButtons;
            tabControl1.ItemSize = new Size(0, 1);
            tabControl1.SizeMode = TabSizeMode.Fixed;

            foreach (TabPage tab in tabControl1.TabPages)
            {
                tab.Text = "";
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }


        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = UserProfile;
            string email = "";
            string phoneNum = "";
            string address = "";
            string state = "";
            string zip = "";


            // Open connection to the database
            using (SqlConnection connection = new SqlConnection(connectionString))
            { 
                string query = "SELECT email, Number, Address, State, Zip FROM login_table WHERE username_id = @Username";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", usernameFor2FA);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {   // Populate the profile page
                        email = reader["email"].ToString();
                        mtbProfileEmail.Text = email;
                        phoneNum = reader["Number"].ToString();
                        mtbProfilePhone.Text = phoneNum;
                        address = reader["Address"].ToString();
                        mtbProfileAddress.Text = address;
                        state = reader["State"].ToString();
                        mtbProfileState.Text = state;
                        zip = reader["Zip"].ToString();
                        mtbProfileZip.Text = zip;
                    }

                    reader.Close();
                }
            }

            return;
        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = ChoosePetProfile;
            string petName1 = "";
            string petName2 = "";
            string petName3 = "";
            string petName4 = "";
            string petName5 = "";
            string petName6 = "";
            // Open connection to the database
            using (SqlConnection connection = new SqlConnection(connectionString))
            { // SQL command to select the top 6 entries in Pet_Profile_Table
                string sqlQuery = "SELECT TOP 6 PetName FROM Pet_Profile_Table WHERE User_ID = @currentLoginID";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
                {
                    connection.Open();
                    cmd.Parameters.AddWithValue("@currentLoginID", currentLoginID);
                    SqlDataReader reader = cmd.ExecuteReader();

                    int count = 0;
                    while (reader.Read())
                    {   // Populate the pets page with the first 6 pets in the database
                        switch (count)
                        {
                            case 0:
                                petName1 = reader.GetString(0);
                                btnPet1.Text = petName1;
                                break;
                            case 1:
                                petName2 = reader.GetString(0);
                                btnPet2.Text = petName2;
                                break;
                            case 2:
                                petName3 = reader.GetString(0);
                                btnPet3.Text = petName3;
                                break;
                            case 3:
                                petName4 = reader.GetString(0);
                                btnPet4.Text = petName4;
                                break;
                            case 4:
                                petName5 = reader.GetString(0);
                                btnPet5.Text = petName5;
                                break;
                            case 5:
                                petName6 = reader.GetString(0);
                                btnPet6.Text = petName6;
                                break;
                        }
                        count++;
                    }

                    reader.Close();
                }
            }
            return;
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }
        private void btnSubmit1_Click(object sender, EventArgs e)
        {
            // Get values from text boxes
            string username = mtbUser2.Text;
            string password = mtbPass2.Text;
            string reenteredPassword = mtbReenter.Text;
            string firstName = mtbFirstName.Text;
            string lastName = mtbLastName.Text;
            string email = mtbEmail.Text;
            string number = mtbNumber.Text;
            string address = mtbAddress.Text;
            string state = mtbState.Text;
            string zip = mtbZip.Text;

            // Validate input fields
            if (username.Length > 15)
            {
                MessageBox.Show("Username must be under 15 characters.");
                return;
            }

            if (password.Length < 6 || password.Length > 50)
            {
                MessageBox.Show("Password must be between 6 to 50 characters.");
                return;
            }

            if (password != reenteredPassword)
            {
                MessageBox.Show("Passwords do not match. Please re-enter.");
                return;
            }
            
            if (firstName.Length < 1)
            {
                MessageBox.Show("Enter a First Name");
                return;
            }

            if (lastName.Length < 1)
            {
                MessageBox.Show("Enter a Last Name");
                return;
            }

            if (!email.Contains("@"))
            {
                MessageBox.Show("Invalid email address.");
                return;
            }

            if (number.Length != 10)
            {
                MessageBox.Show("Phone number must be exactly 10 characters long.");
                return;
            }

            if (zip.Length != 5)
            {
                MessageBox.Show("Zip code must be exactly 5 digits.");
                return;
            }

            try
            {

                // Open connection to the database
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string checkUsernameQuery = "SELECT COUNT(*) FROM Login_table WHERE Username_id = @username";
                    using (SqlCommand checkUsernameCmd = new SqlCommand(checkUsernameQuery, connection))
                    {
                        checkUsernameCmd.Parameters.AddWithValue("@username", username);
                        int usernameCount = (int)checkUsernameCmd.ExecuteScalar();
                        if (usernameCount > 0)
                        {
                            MessageBox.Show("Username already exists. Please choose a different one.");
                            return;
                        }
                    }
                        // SQL command to insert data into the login_table
                        string sqlInsert = "INSERT INTO Login_table (Username_id, Password_id, Email, Number, Address, State, Zip, FirstName, LastName) " +
                                       "VALUES (@username, @password, @email, @number, @address, @state, @zip, @firstName, @lastName)";

                    // Create SQL command with parameters
                    using (SqlCommand cmd = new SqlCommand(sqlInsert, connection))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@number", number);
                        cmd.Parameters.AddWithValue("@address", address);
                        cmd.Parameters.AddWithValue("@state", state);
                        cmd.Parameters.AddWithValue("@zip", zip);
                        cmd.Parameters.AddWithValue("@firstName", firstName);
                        cmd.Parameters.AddWithValue("@lastName", lastName);

                        // Execute the command
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Data inserted successfully!");
                    tabControl1.SelectedTab = Login;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        private void btnLogin_Click(object sender, EventArgs e)
        {
            string enteredUsername = mtbUser1.Text;
            string enteredPassword = mtbPass1.Text;

            try
            {
                                            // Open connection to the database
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    // SQL command to find match username/password with typed information
                    string sqlQuery = "SELECT Login_Id FROM login_table WHERE username_id = @username AND password_id = @password";

                    using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@username", enteredUsername);
                        cmd.Parameters.AddWithValue("@password", enteredPassword);

                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            currentLoginID = reader.GetInt32(0); // Store Login_Id for later use
                        }
                        else
                        {
                            MessageBox.Show("Incorrect username or password. Please try again.");
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            try
            { // Open connection to the database
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    // SQL command to find match username/password with typed information
                    string sqlQuery = "SELECT COUNT(*) FROM login_table WHERE username_id = @username AND password_id = @password";

                    using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@username", enteredUsername);
                        cmd.Parameters.AddWithValue("@password", enteredPassword);

                        int count = (int)cmd.ExecuteScalar();

                        if (count > 0)
                        {
                            // User authenticated successfully
                            usernameFor2FA = enteredUsername;
                        }
                        else
                        {
                            MessageBox.Show("Incorrect username or password. Please try again.");
                            return;
                        }
                    }
                }
                try
                { // Open connection to the database
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        // SQL command to find the phone number to use for 2FA
                        string sqlQuery = "SELECT Number FROM login_table WHERE username_id = @username";

                        using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
                        {
                            cmd.Parameters.AddWithValue("@username", usernameFor2FA);

                            phoneNumberFor2FA = "+1" + (string)cmd.ExecuteScalar();
                        }
                    }
                    // Generate a random 6-digit 2FA code
                    Random rand = new Random();
                    generated2FACode = rand.Next(100000, 999999).ToString();

                    // Send 2FA code via Twilio
                    TwilioClient.Init(twilioAccountSid, twilioAuthToken);

                    var message = MessageResource.Create(
                        body: $"Your verification code is: {generated2FACode}",
                        from: new PhoneNumber(twilioPhoneNumber),
                        to: new PhoneNumber(phoneNumberFor2FA)
                    );

                    MessageBox.Show("A verification code has been sent to your phone number. (Workaround: Code is " + generated2FACode + ")");
                    tabControl1.SelectedTab = TwoFactor; // Switch to TwoFactor tab
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        private void btnResendCode_Click(object sender, EventArgs e)
        {
            try
            {
                Random rand = new Random();
                generated2FACode = rand.Next(100000, 999999).ToString();

                // Send 2FA code via Twilio
                TwilioClient.Init(twilioAccountSid, twilioAuthToken);

                var message = MessageResource.Create(
                    body: $"Your verification code is: {generated2FACode}",
                    from: new PhoneNumber(twilioPhoneNumber),
                    to: new PhoneNumber(phoneNumberFor2FA)
                );

                MessageBox.Show("A verification code has been sent to your phone number. (Workaround: Code is " + generated2FACode + ")");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        private void btnEnter_Click(object sender, EventArgs e)
        {
            string enteredCode = mtbCode.Text;

            if (enteredCode == generated2FACode)
            {
                // Code matched, move to Welcome tab
                tabControl1.SelectedTab = Welcome;
            }
            else
            {
                MessageBox.Show("Incorrect verification code. Please try again.");
            }
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {

        }

        private void label16_Click(object sender, EventArgs e)
        {

        }

        private void maskedTextBox6_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            // Get values from text boxes
            string PetName = mtbPetName.Text;
            string Animal = mtbAnimal.Text;
            string Breed = mtbBreed.Text;
            string KnownComplications = mtbKnownComplications.Text;
            string Age = mtbAge.Text;
            string Weight = mtbWeight.Text;
            string Height = mtbHeight.Text;
            string Prescriptions = mtbPrescriptions.Text;

            // Validate input fields
            if (PetName.Length > 30)
            {
                MessageBox.Show("Username must be under 30 characters.");
                return;
            }

            if (Animal.Length > 50)
            {
                MessageBox.Show("Animal name must shorter than 50 characters.");
                return;
            }

            try
            {
                // Open connection to the database
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // SQL command to insert data into the Pet_Profile_Table
                    string sqlInsert = "INSERT INTO Pet_Profile_Table (PetName, Animal, Breed, KnownComplications, Age, Weight, Height, Prescriptions, User_Id) " +
                                       "VALUES (@petName, @animal, @breed, @knownComplications, @age, @weight, @height, @prescriptions, @userid)";

                    // Create SQL command with parameters
                    using (SqlCommand cmd = new SqlCommand(sqlInsert, connection))
                    {
                        cmd.Parameters.AddWithValue("@petName", PetName);
                        cmd.Parameters.AddWithValue("@animal", Animal);
                        cmd.Parameters.AddWithValue("@breed", Breed);
                        cmd.Parameters.AddWithValue("@knownComplications", KnownComplications);
                        cmd.Parameters.AddWithValue("@age", Age);
                        cmd.Parameters.AddWithValue("@weight", Weight);
                        cmd.Parameters.AddWithValue("@height", Height);
                        cmd.Parameters.AddWithValue("@prescriptions", Prescriptions);
                        cmd.Parameters.AddWithValue("@userid", currentLoginID);

                        // Execute the command
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Data inserted successfully!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            tabControl1.SelectedTab = ThankYouNewProfile;

        }

        private void maskedTextBox8_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void label30_Click(object sender, EventArgs e)
        {

        }

        private void label29_Click(object sender, EventArgs e)
        {

        }

        private void label28_Click(object sender, EventArgs e)
        {

        }

        private void maskedTextBox17_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

        private void maskedTextBox27_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

        private void label38_Click(object sender, EventArgs e)
        {

        }

        private void maskedTextBox30_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

        private void maskedTextBox29_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

        private void textBox15_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox19_TextChanged(object sender, EventArgs e)
        {

        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            tabControl1.SelectedTab = CreateNewPass;
        }

        private void button5_Click(object sender, EventArgs e)
        { // Open connection to the database
            using (SqlConnection connection = new SqlConnection(connectionString))
            { // SQL command to edit profile
                string sqlUpdate = @"UPDATE Login_Table
                         SET FirstName = @firstName,
                             LastName = @lastName,
                             Email = @email,
                             Number = @number,
                             Address = @address,
                             State = @state,
                             Zip = @zip
                         WHERE Login_id = @login";

                using (SqlCommand cmd = new SqlCommand(sqlUpdate, connection))
                {
                    cmd.Parameters.AddWithValue("@firstName", mtbEditFirst.Text);
                    cmd.Parameters.AddWithValue("@lastName", mtbEditLast.Text);
                    cmd.Parameters.AddWithValue("@email", mtbEmail3.Text);
                    cmd.Parameters.AddWithValue("@number", mtbPhone2.Text);
                    cmd.Parameters.AddWithValue("@address", mtbAddress2.Text);
                    cmd.Parameters.AddWithValue("@state", mtbState2.Text);
                    cmd.Parameters.AddWithValue("@zip", mtbZip2.Text);
                    cmd.Parameters.AddWithValue("@login", currentLoginID);

                    connection.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Profile updated successfully.");
                        tabControl1.SelectedTab = Welcome;
                    }
                    else
                    {
                        MessageBox.Show("Profile was not updated.");
                    }
                }
            }
        }

        private void maskedTextBox35_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

        private void maskedTextBox18_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

        private void pictureBox35_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox12_Click(object sender, EventArgs e)
        {

        }

        private void btnEditUserProfile_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = EditProfile;
        }

        private void btnEditPetProfile_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = EditPetProfile;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = Welcome;
        }

        private void btnEditProfile_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = EditProfile;
        }

        private void btnReturnWelcomePage_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = Welcome;
        }

        private void btnReturnToHome_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = Welcome;
        }

        private void btnBackPetProfile_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = ChoosePetProfile;
        }

        private void btnBackWelcome1_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = Welcome;
        }

        private void btnCreateNewUser_Click(object sender, EventArgs e)
        {
            if (mtbNewUser.Text == mtbReenterUser.Text)
            {
                string newUser = mtbNewUser.Text;
                // Open connection to the database
                using (SqlConnection connection = new SqlConnection(connectionString))
                { // SQL command to find the username to change
                    string query = "UPDATE login_table SET Username_Id = @NewUsername WHERE Username_Id = @CurrentUsername";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@NewUsername", newUser);
                        command.Parameters.AddWithValue("@CurrentUsername", usernameFor2FA);
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Username updated successfully.");
                            tabControl1.SelectedTab = Login;
                        }
                        else
                        {
                            MessageBox.Show("Failed to update username.");
                        }
                    }
                }
            }
            else
            {
                mtbNewUser = null;
                mtbReenterUser = null;
                MessageBox.Show("Username Must match Re-Entered Username");
            }
        }
        private void btnNewPass_Click(object sender, EventArgs e)
        {
            if (mtbNewPass.Text == mtbReenterPass.Text)
            {
                string newPassword = mtbNewPass.Text;
                // Open connection to the database
                using (SqlConnection connection = new SqlConnection(connectionString))
                { // SQL command to update password
                    string query = "UPDATE login_table SET Password_Id = @NewPassword WHERE Username_Id = @CurrentUsername";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@NewPassword", newPassword);
                        command.Parameters.AddWithValue("@CurrentUsername", usernameFor2FA);
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Password updated successfully.");
                        }
                        else
                        {
                            MessageBox.Show("Failed to update Password.");
                        }
                    }
                }
            }
            else
            {
                mtbNewPass = null;
                mtbReenterPass = null;
                MessageBox.Show("Password Must match Re-Entered Password");
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = Welcome;
        }

        private void pictureBox30_Click(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            tabControl1.SelectedTab = CreateAccount;
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = ChoosePetProfile;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = Login;
        }

        private void maskedTextBox32_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }
        private void maskedTextBox32_Click(object sender, MaskInputRejectedEventArgs e)
        {
            tabControl1.SelectedTab = EditPetProfile;
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = EditPetProfile;
        }

        private void btnCreatePet_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = CreatePet;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = ChoosePetProfile;
        }

        private void label41_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            currentPetName = btnPet4.Text;
            PopulateTextBoxes(currentPetName);
            petProfileID = GetPetProfileID(currentPetName, currentLoginID);
            tabControl1.SelectedTab = PetProfile;
        }

        private void btnPet1_Click(object sender, EventArgs e)
        {
            currentPetName = btnPet1.Text;
            PopulateTextBoxes(currentPetName);
            petProfileID = GetPetProfileID(currentPetName, currentLoginID);
            tabControl1.SelectedTab = PetProfile;
        }

        private void btnPet2_Click(object sender, EventArgs e)
        {
            currentPetName = btnPet2.Text;
            PopulateTextBoxes(currentPetName);
            petProfileID = GetPetProfileID(currentPetName, currentLoginID);
            tabControl1.SelectedTab = PetProfile;
        }

        private void btnPet3_Click(object sender, EventArgs e)
        {
            currentPetName = btnPet3.Text;
            PopulateTextBoxes(currentPetName);
            petProfileID = GetPetProfileID(currentPetName, currentLoginID);
            tabControl1.SelectedTab = PetProfile;
        }

        private void btnPet5_Click(object sender, EventArgs e)
        {
            currentPetName = btnPet5.Text;
            PopulateTextBoxes(currentPetName);
            petProfileID = GetPetProfileID(currentPetName, currentLoginID);
            tabControl1.SelectedTab = PetProfile;
        }

        private void btnPet6_Click(object sender, EventArgs e)
        {
            currentPetName = btnPet6.Text;
            PopulateTextBoxes(currentPetName);
            petProfileID = GetPetProfileID(currentPetName, currentLoginID);
            tabControl1.SelectedTab = PetProfile;
        }

        private void btnEditPet_Click(object sender, EventArgs e)
        { // Open connection to the database
            using (SqlConnection connection = new SqlConnection(connectionString))
            { // SQL command to Update the Pet Profile 
                string sqlUpdate = @"UPDATE Pet_Profile_Table
                         SET PetName = @Name,
                             Animal = @Animal,
                             Breed = @Breed,
                             KnownComplications = @KnownComplications,
                             Age = @Age,
                             Weight = @Weight,
                             Height = @Height,
                             Prescriptions = @Prescriptions,
                             User_Id = @UserID
                         WHERE PetProfile_Id = @PetID";

                using (SqlCommand cmd = new SqlCommand(sqlUpdate, connection))
                {
                    cmd.Parameters.AddWithValue("@Name", mtbName2.Text);
                    cmd.Parameters.AddWithValue("@Animal", mtbAnimal2.Text);
                    cmd.Parameters.AddWithValue("@Breed", mtbBreed2.Text);
                    cmd.Parameters.AddWithValue("@KnownComplications", mtbKnownComplications2.Text);
                    cmd.Parameters.AddWithValue("@Age", mtbAge2.Text);
                    cmd.Parameters.AddWithValue("@Weight", mtbWeight2.Text);
                    cmd.Parameters.AddWithValue("@Height", mtbHeight2.Text);
                    cmd.Parameters.AddWithValue("@Prescriptions", mtbPrescriptions2.Text);
                    cmd.Parameters.AddWithValue("@PetName", currentPetName);
                    cmd.Parameters.AddWithValue("@UserID", currentLoginID);
                    cmd.Parameters.AddWithValue("@PetID", petProfileID);

                    connection.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Pet profile updated successfully.");
                        tabControl1.SelectedTab = Welcome;
                    }
                    else
                    {
                        MessageBox.Show("No pet profile was updated. Pet ID may not exist.");
                    }
                }
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = ChoosePetProfile;
        }
        private void PopulateTextBoxes(string petName)
        { // Open connection to the database
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = @"SELECT PetName, Animal, Breed, KnownComplications, Age, Weight, Height, Prescriptions
                           FROM Pet_Profile_Table
                           WHERE PetName = @PetName";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@PetName", petName);

                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        mtbName1.Text = reader["PetName"].ToString();
                        mtbAnimal1.Text = reader["Animal"].ToString();
                        mtbBreed1.Text = reader["Breed"].ToString();
                        mtbKnownComplications1.Text = reader["KnownComplications"].ToString();
                        mtbAge1.Text = reader["Age"].ToString();
                        mtbWeight1.Text = reader["Weight"].ToString();
                        mtbHeight1.Text = reader["Height"].ToString();
                        mtbPrescriptions1.Text = reader["Prescriptions"].ToString();
                    }
                    else
                    {
                        // PetID not found in the database
                        MessageBox.Show("Pet profile not found.");
                    }

                    reader.Close();
                }
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = Welcome;
        }
        private int GetPetProfileID(string petName, int userID)
        {
            petProfileID = -1; // Default value if no matching record is found

            try
            {
                // Open connection to the database
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    // SQL command to retrieve the PetProfile_ID for the given petName and userID
                    string sqlQuery = "SELECT PetProfile_ID FROM Pet_Profile_Table WHERE PetName = @petName AND User_Id = @userID";

                    using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@petName", currentPetName);
                        cmd.Parameters.AddWithValue("@userID", currentLoginID);

                        // Execute the command to get the PetProfile_ID
                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            petProfileID = Convert.ToInt32(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            return petProfileID;
        }
    }
}

