namespace EAMI.WebApi.Models
{
    /// <summary>
    /// TokenParameters
    /// </summary>
    public class TokenParameters
    {
        /// <summary>
        /// Gets or sets the type of the grant.
        /// </summary>
        /// <value>
        /// The type of the grant.
        /// </value>
        public string Grant_type { get; set; }
        /// <summary>
        /// Gets or sets the refresh token.
        /// </summary>
        /// <value>
        /// The refresh token.
        /// </value>
        public string Refresh_token { get; set; }
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string User_name { get; set; }
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public long User_id { get; set; }
        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password { get; set; }
        /// <summary>
        /// Gets or sets the company identifier.
        /// </summary>
        /// <value>
        /// The company identifier.
        /// </value>
        public string Company_id { get; set; }
        /// <summary>
        /// Gets or sets the user type identifier.
        /// </summary>
        /// <value>
        /// The user type identifier.
        /// </value>
        public string User_type_id { get; set; }
        /// <summary>
        /// Gets or sets the provider type identifier.
        /// </summary>
        /// <value>
        /// The provider type identifier.
        /// </value>
        public string Program_Choice_Id { get; set; }
    }
}
