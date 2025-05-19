using System;
using System.Collections.Generic;
using MedicalSystem.Infrastructure.Data;

namespace MedicalSystem.Infrastructure.Repositories
{
    public interface IRepositoryFactory
    {
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
        IPatientRepository GetPatientRepository();
    }

    
}