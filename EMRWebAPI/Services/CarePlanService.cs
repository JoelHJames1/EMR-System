using EMRDataLayer.Model;
using EMRDataLayer.Repository.IRepository;
using EMRWebAPI.Services.IServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRWebAPI.Services
{
    public class CarePlanService : ICarePlanService
    {
        private readonly ICarePlanRepository _carePlanRepository;
        private readonly IRepository<CarePlanActivity> _activityRepository;
        private readonly ILogger<CarePlanService> _logger;

        public CarePlanService(
            ICarePlanRepository carePlanRepository,
            IRepository<CarePlanActivity> activityRepository,
            ILogger<CarePlanService> logger)
        {
            _carePlanRepository = carePlanRepository;
            _activityRepository = activityRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<CarePlan>> GetPatientCarePlansAsync(int patientId)
        {
            try
            {
                return await _carePlanRepository.GetByPatientIdAsync(patientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving care plans for patient {patientId}");
                throw;
            }
        }

        public async Task<IEnumerable<CarePlan>> GetActiveCarePlansAsync(int patientId)
        {
            try
            {
                return await _carePlanRepository.GetActiveCarePlansAsync(patientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving active care plans for patient {patientId}");
                throw;
            }
        }

        public async Task<CarePlan?> GetCarePlanByIdAsync(int id)
        {
            try
            {
                return await _carePlanRepository.GetByIdWithActivitiesAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving care plan {id}");
                throw;
            }
        }

        public async Task<CarePlan> CreateCarePlanAsync(CarePlan carePlan, string userId)
        {
            try
            {
                carePlan.Status = "Active";
                carePlan.CreatedDate = DateTime.UtcNow;
                carePlan.CreatedBy = userId;

                return await _carePlanRepository.AddAsync(carePlan);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating care plan");
                throw;
            }
        }

        public async Task<CarePlanActivity> AddActivityAsync(int carePlanId, CarePlanActivity activity)
        {
            try
            {
                var carePlan = await _carePlanRepository.GetByIdAsync(carePlanId);
                if (carePlan == null)
                {
                    throw new KeyNotFoundException($"Care plan with ID {carePlanId} not found");
                }

                activity.CarePlanId = carePlanId;
                activity.CreatedDate = DateTime.UtcNow;

                return await _activityRepository.AddAsync(activity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding activity to care plan {carePlanId}");
                throw;
            }
        }

        public async Task<bool> UpdateActivityStatusAsync(int activityId, string status)
        {
            try
            {
                var activity = await _activityRepository.GetByIdAsync(activityId);
                if (activity == null)
                {
                    return false;
                }

                activity.Status = status;

                if (status == "Completed")
                {
                    activity.CompletedDate = DateTime.UtcNow;
                }

                await _activityRepository.UpdateAsync(activity);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating activity status {activityId}");
                throw;
            }
        }

        public async Task<bool> UpdateCarePlanStatusAsync(int id, string status, string userId)
        {
            try
            {
                var carePlan = await _carePlanRepository.GetByIdAsync(id);
                if (carePlan == null)
                {
                    return false;
                }

                carePlan.Status = status;
                carePlan.ModifiedDate = DateTime.UtcNow;
                carePlan.ModifiedBy = userId;

                if (status == "Completed")
                {
                    carePlan.EndDate = DateTime.UtcNow;
                }

                await _carePlanRepository.UpdateAsync(carePlan);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating care plan status {id}");
                throw;
            }
        }
    }
}