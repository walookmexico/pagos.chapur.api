using PagosGranChapur.Data.Infrastructure;
using PagosGranChapur.Entities;
using PagosGranChapur.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PagosGranChapur.Services
{
    public interface IPlatformService    {
        Task<List<Platform>> GetAll();
        Task<bool> Add(Platform entidad);
    }

    public class PlatformService : IPlatformService
    {
        private readonly IUnitOfWork _unitOfWork;
        public readonly IPlatformRepository _platformRepository;

        public PlatformService(IUnitOfWork unitOfWork,
           IPlatformRepository platformRepository) {

            this._platformRepository = platformRepository;
            this._unitOfWork         = unitOfWork;
        }

        /// <summary>
        /// AGREGA UNA NUEVA PLATAFORMA AL CATÁLOGO
        /// </summary>
        /// <param name="entidad"></param>
        /// <returns></returns>
        public async Task<bool> Add(Platform entidad)
        {
            try
            {
                await this._platformRepository.AddAsync(entidad);
                await this._unitOfWork.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// OBTIENE TODAS LAS PLATAFORMAS REGISTRADAS
        /// </summary>
        /// <returns></returns>
        public async Task<List<Platform>> GetAll()
        {
            try
            {
                return (await this._platformRepository.GetAllAsync())?.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
