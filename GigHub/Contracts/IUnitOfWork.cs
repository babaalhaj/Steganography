namespace GigHub.Contracts
{
    public interface IUnitOfWork
    {
        IGenresRepository Genres { get; }
        IGigsRepository Gigs { get; }
        IAttendancesRepository Attendances { get; }
        //Task<int> AsyncComplete();
        int Complete();
    }
}
