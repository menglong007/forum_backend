public class UserSignUpRequest
{
    public string Email { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}

public class UserLoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class UserDetailResponse
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }

    public class UserUpdateRequest
    {
        public string Username { get; set; }
    }
}