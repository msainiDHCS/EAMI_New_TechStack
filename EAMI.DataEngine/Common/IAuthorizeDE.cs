namespace EAMI.DataEngine
{
    public interface IAuthorizeDE
    {
        int RoleId { get; set; }
        int UserId { get; set; }
        int ProgramId { get; set; }
    }
}
