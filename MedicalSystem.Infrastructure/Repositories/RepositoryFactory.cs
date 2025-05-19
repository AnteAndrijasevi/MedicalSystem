using MedicalSystem.Infrastructure.Data;
using MedicalSystem.Infrastructure.Repositories;

public class RepositoryFactory : IRepositoryFactory
{
    private readonly MedicalSystemContext _context;
    private readonly Dictionary<Type, object> _repositories;
    private IPatientRepository? _patientRepository;

    public RepositoryFactory(MedicalSystemContext context)
    {
        _context = context;
        _repositories = new Dictionary<Type, object>();
    }

    public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
    {
        var type = typeof(TEntity);

        if (!_repositories.ContainsKey(type))
        {
            var repositoryType = typeof(Repository<>).MakeGenericType(type);
            var repository = Activator.CreateInstance(repositoryType, _context);
            if (repository != null)
            {
                _repositories.Add(type, repository);
            }
        }

        return (IRepository<TEntity>)_repositories[type];
    }

    public IPatientRepository GetPatientRepository()
    {
        if (_patientRepository == null)
        {
            _patientRepository = new PatientRepository(_context);
        }

        return _patientRepository;
    }
}