using PagosGranChapur.Data.Infrastructure;
using PagosGranChapur.Entities;
using PagosGranChapur.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PagosGranChapur.Services
{
    public interface IStoreService {
        Task<List<Store>> GetAll();
        Task<bool> Add(Store entidad);
    }

    public class StoreService : IStoreService
    {
        private readonly IUnitOfWork _unitOfWork;
        public readonly IStoreRepository _storeRepository;

        public StoreService(IUnitOfWork unitOfWork,
            IStoreRepository storeRepository) {

            this._storeRepository = storeRepository;
            this._unitOfWork      = unitOfWork;
        }

        /// <summary>
        /// MÉTODO QUE PERMITE AGREGAR UNA NUEVA TIENDA A LA BASE DE DATOS
        /// </summary>
        /// <param name="entidad"></param>
        /// <returns></returns>
        public async Task<bool> Add(Store entidad)
        {
            try
            {
                await this._storeRepository.AddAsync(entidad);
                await this._unitOfWork.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// MÉTODO QUE PERMITE OBTENER EL LISTADO DE TODAS LAS TIENDAS
        /// </summary>
        /// <returns></returns>
        public async Task<List<Store>> GetAll()
        {
            try
            {
                return (await this._storeRepository.GetAllAsync())?.ToList();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
